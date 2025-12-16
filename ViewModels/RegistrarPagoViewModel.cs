using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    [QueryProperty(nameof(PrestamoId), "prestamoId")]
    public class RegistrarPagoViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private Prestamo _prestamo;
        private int _prestamoId;
        private string _montoPago = string.Empty;
        private string _notaPago = string.Empty;

        public Prestamo Prestamo
        {
            get => _prestamo;
            set
            {
                _prestamo = value;
                OnPropertyChanged();
            }
        }

        public int PrestamoId
        {
            get => _prestamoId;
            set
            {
                _prestamoId = value;
                OnPropertyChanged();
            }
        }

        public string MontoPago
        {
            get => _montoPago;
            set
            {
                _montoPago = value;
                OnPropertyChanged();
            }
        }

        public string NotaPago
        {
            get => _notaPago;
            set
            {
                _notaPago = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegistrarPagoCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand PagarInteresCommand { get; }
        public ICommand PagarTotalCommand { get; }

        public RegistrarPagoViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _prestamo = new Prestamo();
            
            RegistrarPagoCommand = new Command(async () => await RegistrarPago());
            CancelarCommand = new Command(async () => await Cancelar());
            PagarInteresCommand = new Command(() => MontoPago = Prestamo.InteresAcumulado.ToString("F2"));
            PagarTotalCommand = new Command(() => MontoPago = Prestamo.TotalAdeudado.ToString("F2"));
        }

        public async Task LoadDataAsync()
        {
            try
            {
                var prestamo = await _databaseService.GetPrestamoAsync(PrestamoId);
                if (prestamo != null)
                {
                    // Actualizar interes acumulado antes de mostrar
                    ActualizarInteresAcumulado(prestamo);
                    Prestamo = prestamo;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar los datos: {ex.Message}", "OK");
            }
        }

        private void ActualizarInteresAcumulado(Prestamo prestamo)
        {
            // Calcular semanas desde el ultimo pago o desde el inicio
            var fechaReferencia = prestamo.FechaUltimoPago ?? prestamo.FechaInicio;
            var dias = (DateTime.Now - fechaReferencia).Days;
            var semanasTranscurridas = Math.Max(0, dias / 7);
            
            // Calcular interes acumulado basado en el capital pendiente
            // El interes se calcula por semana sobre el capital pendiente
            var interesPorSemana = prestamo.CapitalPendiente * (prestamo.TasaInteresSemanal / 100);
            
            // Si hay semanas transcurridas desde el ultimo pago, agregar el nuevo interes
            if (semanasTranscurridas > 0)
            {
                var nuevoInteres = interesPorSemana * semanasTranscurridas;
                prestamo.InteresAcumulado += nuevoInteres;
            }
            
            // Actualizar total adeudado
            prestamo.TotalAdeudado = prestamo.CapitalPendiente + prestamo.InteresAcumulado;
        }

        private async Task RegistrarPago()
        {
            if (!ValidarFormulario())
                return;

            try
            {
                if (!decimal.TryParse(MontoPago, out decimal monto))
                {
                    await Shell.Current.DisplayAlert("Error", "El monto debe ser un numero valido", "OK");
                    return;
                }

                // Aplicar el pago segun el sistema: primero interes, luego capital
                decimal montoRestante = monto;
                decimal pagoInteres = 0;
                decimal pagoCapital = 0;

                // 1. Aplicar al interes acumulado
                if (Prestamo.InteresAcumulado > 0)
                {
                    pagoInteres = Math.Min(montoRestante, Prestamo.InteresAcumulado);
                    Prestamo.InteresAcumulado -= pagoInteres;
                    montoRestante -= pagoInteres;
                }

                // 2. El excedente va al capital
                if (montoRestante > 0)
                {
                    pagoCapital = Math.Min(montoRestante, Prestamo.CapitalPendiente);
                    Prestamo.CapitalPendiente -= pagoCapital;
                    montoRestante -= pagoCapital;
                }

                // Actualizar totales
                Prestamo.MontoPagado += monto;
                Prestamo.FechaUltimoPago = DateTime.Now;
                Prestamo.TotalAdeudado = Prestamo.CapitalPendiente + Prestamo.InteresAcumulado;

                // Verificar si el prestamo esta completado
                if (Prestamo.CapitalPendiente <= 0)
                {
                    Prestamo.Estado = "Completado";
                    Prestamo.CapitalPendiente = 0;
                    Prestamo.InteresAcumulado = 0;
                    Prestamo.TotalAdeudado = 0;
                }

                // Actualizar notas
                var fechaPago = DateTime.Now;
                var notaPagoCompleta = $"Pago registrado: S/{monto:N2} ({fechaPago:dd/MM/yyyy HH:mm})";
                if (pagoInteres > 0)
                    notaPagoCompleta += $" - Interes: S/{pagoInteres:N2}";
                if (pagoCapital > 0)
                    notaPagoCompleta += $" - Capital: S/{pagoCapital:N2}";
                if (!string.IsNullOrWhiteSpace(NotaPago))
                    notaPagoCompleta += $" - {NotaPago}";
                
                Prestamo.Notas += "\n" + notaPagoCompleta;

                // Guardar en base de datos
                await _databaseService.SavePrestamoAsync(Prestamo);

                // Actualizar cliente
                var cliente = await _databaseService.GetClienteAsync(Prestamo.ClienteId);
                if (cliente != null)
                {
                    // Recalcular deuda pendiente del cliente
                    var prestamosActivos = await _databaseService.GetPrestamosByClienteAsync(cliente.Id);
                    cliente.DeudaPendiente = prestamosActivos
                        .Where(p => p.Estado == "Activo")
                        .Sum(p => p.TotalAdeudado);
                    
                    // Actualizar contador de prestamos activos
                    cliente.PrestamosActivos = prestamosActivos.Count(p => p.Estado == "Activo");
                    
                    await _databaseService.SaveClienteAsync(cliente);
                }

                // Mostrar mensaje de exito
                var mensaje = $"Pago registrado exitosamente\n\n";
                if (pagoInteres > 0)
                    mensaje += $"Interes pagado: S/{pagoInteres:N2}\n";
                if (pagoCapital > 0)
                    mensaje += $"Capital pagado: S/{pagoCapital:N2}\n";
                if (montoRestante > 0)
                    mensaje += $"\nSobra: S/{montoRestante:N2}";
                
                if (Prestamo.Estado == "Completado")
                    mensaje += "\n\n¡Prestamo completado!";

                await Shell.Current.DisplayAlert("Exito", mensaje, "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo registrar el pago: {ex.Message}", "OK");
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(MontoPago))
            {
                Shell.Current.DisplayAlert("Validacion", "Debe ingresar el monto del pago", "OK");
                return false;
            }

            if (!decimal.TryParse(MontoPago, out decimal monto) || monto <= 0)
            {
                Shell.Current.DisplayAlert("Validacion", "El monto debe ser un numero mayor a cero", "OK");
                return false;
            }

            return true;
        }

        private async Task Cancelar()
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Cancelar", 
                "¿Esta seguro que desea cancelar?", 
                "Si", 
                "No");

            if (confirmar)
            {
                await Shell.Current.GoToAsync("..");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
