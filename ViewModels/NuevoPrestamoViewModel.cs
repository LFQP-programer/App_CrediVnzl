using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    [QueryProperty(nameof(ClienteId), "clienteId")]
    public class NuevoPrestamoViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private Cliente? _clienteSeleccionado;
        private string _montoInicial = string.Empty;
        private DateTime _fechaInicio = DateTime.Today;
        private int _clienteId;
        private bool _condicionesExpandidas = false;
        private const decimal TASA_INTERES_SEMANAL = 5.0m;

        public ObservableCollection<Cliente> Clientes { get; set; } = new();

        public Cliente? ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set
            {
                _clienteSeleccionado = value;
                OnPropertyChanged();
            }
        }

        public string MontoInicial
        {
            get => _montoInicial;
            set
            {
                _montoInicial = value;
                OnPropertyChanged();
            }
        }

        public DateTime FechaInicio
        {
            get => _fechaInicio;
            set
            {
                _fechaInicio = value;
                OnPropertyChanged();
            }
        }

        public int ClienteId
        {
            get => _clienteId;
            set
            {
                _clienteId = value;
                OnPropertyChanged();
                _ = PreseleccionarCliente();
            }
        }

        public bool CondicionesExpandidas
        {
            get => _condicionesExpandidas;
            set
            {
                _condicionesExpandidas = value;
                OnPropertyChanged();
            }
        }

        public ICommand CrearPrestamoCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand ToggleCondicionesCommand { get; }

        public NuevoPrestamoViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            CrearPrestamoCommand = new Command(async () => await CrearPrestamo());
            CancelarCommand = new Command(async () => await Cancelar());
            ToggleCondicionesCommand = new Command(() => CondicionesExpandidas = !CondicionesExpandidas);
        }

        public async Task LoadClientesAsync()
        {
            try
            {
                var clientes = await _databaseService.GetClientesAsync();
                Clientes.Clear();
                
                foreach (var cliente in clientes)
                {
                    Clientes.Add(cliente);
                }

                if (ClienteId > 0)
                {
                    await PreseleccionarCliente();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar los clientes: {ex.Message}", "OK");
            }
        }

        private async Task PreseleccionarCliente()
        {
            if (ClienteId > 0 && Clientes.Count > 0)
            {
                ClienteSeleccionado = Clientes.FirstOrDefault(c => c.Id == ClienteId);
            }
        }

        private async Task CrearPrestamo()
        {
            if (!await ValidarFormulario())
                return;

            try
            {
                if (!decimal.TryParse(MontoInicial, out decimal monto))
                {
                    await Shell.Current.DisplayAlert("Error", "El monto debe ser un número válido", "OK");
                    return;
                }

                // Calcular el interes de la primera semana inmediatamente
                var interesPrimeraSemana = monto * (TASA_INTERES_SEMANAL / 100);

                var prestamo = new Prestamo
                {
                    ClienteId = ClienteSeleccionado!.Id,
                    MontoInicial = monto,
                    TasaInteresSemanal = TASA_INTERES_SEMANAL,
                    DuracionSemanas = 0, // Sin duracion fija, pago flexible
                    FechaInicio = FechaInicio,
                    Estado = "Activo",
                    CapitalPendiente = monto,
                    InteresAcumulado = interesPrimeraSemana, // ? Interes inicial
                    TotalAdeudado = monto + interesPrimeraSemana, // ? Total con interes
                    MontoPagado = 0,
                    Notas = $"Préstamo creado el {DateTime.Now:dd/MM/yyyy HH:mm}\n" +
                            $"Capital inicial: S/{monto:N2}\n" +
                            $"Interes primera semana: S/{interesPrimeraSemana:N2}\n" +
                            $"Total adeudado inicial: S/{(monto + interesPrimeraSemana):N2}"
                };

                await _databaseService.SavePrestamoAsync(prestamo);

                // Actualizar contador de prestamos activos del cliente
                ClienteSeleccionado.PrestamosActivos++;
                ClienteSeleccionado.DeudaPendiente += monto + interesPrimeraSemana; // ? Incluir interes
                await _databaseService.SaveClienteAsync(ClienteSeleccionado);

                // Mostrar mensaje con detalle
                var mensaje = $"Préstamo creado exitosamente\n\n" +
                             $"Capital: S/{monto:N2}\n" +
                             $"Interes semanal: S/{interesPrimeraSemana:N2}\n" +
                             $"Total a pagar: S/{(monto + interesPrimeraSemana):N2}\n\n" +
                             $"El cliente ya tiene S/{interesPrimeraSemana:N2} de interes acumulado desde hoy.";

                await Shell.Current.DisplayAlert("Éxito", mensaje, "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo crear el préstamo: {ex.Message}", "OK");
            }
        }

        private async Task<bool> ValidarFormulario()
        {
            if (ClienteSeleccionado == null)
            {
                await Shell.Current.DisplayAlert("Validación", "Debe seleccionar un cliente", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(MontoInicial))
            {
                await Shell.Current.DisplayAlert("Validación", "Debe ingresar el monto del préstamo", "OK");
                return false;
            }

            if (!decimal.TryParse(MontoInicial, out decimal monto) || monto <= 0)
            {
                await Shell.Current.DisplayAlert("Validación", "El monto debe ser un número mayor a cero", "OK");
                return false;
            }

            if (monto > 500)
            {
                bool continuar = await Shell.Current.DisplayAlert(
                    "Advertencia", 
                    "El monto excede el máximo recomendado de S/500. ¿Está seguro?", 
                    "Continuar", 
                    "Cancelar");
                
                if (!continuar)
                    return false;
            }

            return true;
        }

        private async Task Cancelar()
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Cancelar", 
                "¿Está seguro que desea cancelar? Se perderán los datos ingresados.", 
                "Sí", 
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
