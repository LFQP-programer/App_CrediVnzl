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
        private bool _sistemaPagoExpandido = false;

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

        public bool SistemaPagoExpandido
        {
            get => _sistemaPagoExpandido;
            set
            {
                _sistemaPagoExpandido = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegistrarPagoCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand PagarInteresCommand { get; }
        public ICommand PagarTotalCommand { get; }
        public ICommand ToggleSistemaPagoCommand { get; }

        public RegistrarPagoViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _prestamo = new Prestamo();
            
            RegistrarPagoCommand = new Command(async () => await RegistrarPago());
            CancelarCommand = new Command(async () => await Cancelar());
            PagarInteresCommand = new Command(() => MontoPago = Prestamo.InteresAcumulado.ToString("F2"));
            PagarTotalCommand = new Command(() => MontoPago = Prestamo.TotalAdeudado.ToString("F2"));
            ToggleSistemaPagoCommand = new Command(() => SistemaPagoExpandido = !SistemaPagoExpandido);
        }

        public async Task LoadDataAsync()
        {
            try
            {
                var prestamo = await _databaseService.GetPrestamoAsync(PrestamoId);
                if (prestamo != null)
                {
                    // Calcular y actualizar interes acumulado solo si han pasado semanas
                    await ActualizarInteresAcumuladoEnBD(prestamo);
                    
                    // Recargar prestamo actualizado
                    prestamo = await _databaseService.GetPrestamoAsync(PrestamoId);
                    Prestamo = prestamo!;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar los datos: {ex.Message}", "OK");
            }
        }

        private async Task ActualizarInteresAcumuladoEnBD(Prestamo prestamo)
        {
            // Calcular semanas desde el ultimo pago o desde el inicio
            var fechaReferencia = prestamo.FechaUltimoPago ?? prestamo.FechaInicio;
            var dias = (DateTime.Now - fechaReferencia).Days;
            var semanasTranscurridas = Math.Max(0, dias / 7);
            
            // Solo actualizar si han pasado al menos una semana completa DESDE el inicio o ultimo pago
            // Si FechaUltimoPago es null y han pasado menos de 7 dias desde FechaInicio, no agregar mas interes
            if (semanasTranscurridas > 0)
            {
                // Si no hay FechaUltimoPago y es la primera semana, restar 1 porque ya tiene el interes inicial
                var semanasAAgregar = semanasTranscurridas;
                if (prestamo.FechaUltimoPago == null && semanasTranscurridas == 1)
                {
                    // Ya tiene el interes de la primera semana, no agregar nada
                    return;
                }
                
                var interesPorSemana = prestamo.CapitalPendiente * (prestamo.TasaInteresSemanal / 100);
                var nuevoInteres = interesPorSemana * semanasAAgregar;
                
                prestamo.InteresAcumulado += nuevoInteres;
                prestamo.TotalAdeudado = prestamo.CapitalPendiente + prestamo.InteresAcumulado;
                prestamo.FechaUltimoPago = DateTime.Now;
                
                await _databaseService.SavePrestamoAsync(prestamo);
                await _databaseService.ActualizarDeudaClienteAsync(prestamo.ClienteId);
            }
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

                // Validar si el monto excede el total adeudado
                if (monto > Prestamo.TotalAdeudado)
                {
                    var excedente = monto - Prestamo.TotalAdeudado;
                    bool continuar = await Shell.Current.DisplayAlert(
                        "Monto Excedente",
                        $"El monto ingresado (S/{monto:N2}) excede el total adeudado (S/{Prestamo.TotalAdeudado:N2}).\n\n" +
                        $"Excedente: S/{excedente:N2}\n\n" +
                        $"¿Desea registrar el pago por el total adeudado?",
                        "Si",
                        "No");
                    
                    if (!continuar)
                        return;
                    
                    // Ajustar al total adeudado
                    monto = Prestamo.TotalAdeudado;
                    MontoPago = monto.ToString("F2");
                }

                // Guardar estado anterior para historial
                var capitalAntes = Prestamo.CapitalPendiente;
                var interesAntes = Prestamo.InteresAcumulado;

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

                // Guardar en historial de pagos
                var historial = new HistorialPago
                {
                    PrestamoId = Prestamo.Id,
                    ClienteId = Prestamo.ClienteId,
                    MontoTotal = monto,
                    MontoInteres = pagoInteres,
                    MontoCapital = pagoCapital,
                    CapitalPendienteAntes = capitalAntes,
                    CapitalPendienteDespues = Prestamo.CapitalPendiente,
                    InteresAcumuladoAntes = interesAntes,
                    InteresAcumuladoDespues = Prestamo.InteresAcumulado,
                    FechaPago = fechaPago,
                    Nota = NotaPago
                };
                await _databaseService.SaveHistorialPagoAsync(historial);

                // Actualizar ganancia total si se pago interes
                if (pagoInteres > 0)
                {
                    await _databaseService.AgregarGananciaAsync(pagoInteres);
                }

                // Actualizar cliente
                await _databaseService.ActualizarDeudaClienteAsync(Prestamo.ClienteId);

                // Mostrar mensaje de exito
                var mensaje = $"Pago registrado exitosamente\n\n";
                mensaje += $"Total pagado: S/{monto:N2}\n";
                if (pagoInteres > 0)
                    mensaje += $"  - Interes: S/{pagoInteres:N2}\n";
                if (pagoCapital > 0)
                    mensaje += $"  - Capital: S/{pagoCapital:N2}\n";
                
                mensaje += $"\nCapital pendiente: S/{Prestamo.CapitalPendiente:N2}";
                if (Prestamo.InteresAcumulado > 0)
                    mensaje += $"\nInteres acumulado: S/{Prestamo.InteresAcumulado:N2}";
                
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
