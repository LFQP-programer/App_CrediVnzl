using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class DashboardClienteViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        private readonly int _clienteId;
        private string _nombreCliente = string.Empty;
        private int _prestamosActivos;
        private decimal _deudaPendiente;
        private decimal _totalPagado;
        private decimal _proximoPago;
        private DateTime? _fechaProximoPago;

        public string NombreCliente
        {
            get => _nombreCliente;
            set { _nombreCliente = value; OnPropertyChanged(); }
        }

        public int PrestamosActivos
        {
            get => _prestamosActivos;
            set { _prestamosActivos = value; OnPropertyChanged(); }
        }

        public decimal DeudaPendiente
        {
            get => _deudaPendiente;
            set { _deudaPendiente = value; OnPropertyChanged(); }
        }

        public decimal TotalPagado
        {
            get => _totalPagado;
            set { _totalPagado = value; OnPropertyChanged(); }
        }

        public decimal ProximoPago
        {
            get => _proximoPago;
            set { _proximoPago = value; OnPropertyChanged(); }
        }

        public DateTime? FechaProximoPago
        {
            get => _fechaProximoPago;
            set { _fechaProximoPago = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Prestamo> MisPrestamos { get; set; } = new();
        public ObservableCollection<Pago> ProximosPagos { get; set; } = new();
        public ObservableCollection<HistorialPago> UltimosPagos { get; set; } = new();

        public ICommand CerrarSesionCommand { get; }
        public ICommand VerDetallePrestamoCommand { get; }
        public ICommand RealizarPagoCommand { get; }
        public ICommand RefrescarCommand { get; }

        public DashboardClienteViewModel(DatabaseService databaseService, AuthService authService)
        {
            _databaseService = databaseService;
            _authService = authService;
            _clienteId = authService.UsuarioActual?.ClienteId ?? 0;

            CerrarSesionCommand = new Command(OnCerrarSesion);
            VerDetallePrestamoCommand = new Command<Prestamo>(async (prestamo) => await OnVerDetallePrestamo(prestamo));
            RealizarPagoCommand = new Command(async () => await OnRealizarPago());
            RefrescarCommand = new Command(async () => await LoadDataAsync());
        }

        public async Task LoadDataAsync()
        {
            try
            {
                // Cargar información del cliente
                var cliente = await _databaseService.GetClienteAsync(_clienteId);
                if (cliente != null)
                {
                    NombreCliente = cliente.NombreCompleto;
                    DeudaPendiente = cliente.DeudaPendiente;
                }

                // Actualizar intereses antes de cargar datos
                await _databaseService.ActualizarInteresesPrestamosActivosAsync();

                // Cargar préstamos del cliente
                var prestamos = await _databaseService.GetPrestamosByClienteAsync(_clienteId);
                MisPrestamos.Clear();
                PrestamosActivos = 0;
                TotalPagado = 0;

                foreach (var prestamo in prestamos.Where(p => p.Estado == "Activo"))
                {
                    MisPrestamos.Add(prestamo);
                    PrestamosActivos++;
                    TotalPagado += prestamo.MontoPagado;
                }

                // Cargar próximos pagos
                var todosPagos = await _databaseService.GetPagosAsync();
                var pagosPendientes = todosPagos
                    .Where(p => p.ClienteId == _clienteId && p.Estado == "Pendiente")
                    .OrderBy(p => p.FechaProgramada)
                    .Take(5)
                    .ToList();

                ProximosPagos.Clear();
                foreach (var pago in pagosPendientes)
                {
                    ProximosPagos.Add(pago);
                }

                if (pagosPendientes.Any())
                {
                    ProximoPago = pagosPendientes.First().MontoPago;
                    FechaProximoPago = pagosPendientes.First().FechaProgramada;
                }

                // Cargar últimos pagos realizados
                var historial = await _databaseService.GetHistorialPagosByClienteAsync(_clienteId);
                UltimosPagos.Clear();
                foreach (var pago in historial.Take(5))
                {
                    UltimosPagos.Add(pago);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando datos del cliente: {ex.Message}");
            }
        }

        private void OnCerrarSesion()
        {
            _authService.Logout();
            Shell.Current.GoToAsync("//login");
        }

        private async Task OnVerDetallePrestamo(Prestamo prestamo)
        {
            if (prestamo != null)
            {
                await Shell.Current.GoToAsync($"detallecliente?clienteId={_clienteId}");
            }
        }

        private async Task OnRealizarPago()
        {
            // Esta función se puede implementar más adelante
            await Application.Current!.MainPage!.DisplayAlert("Próximamente", 
                "La funcionalidad de pago en línea estará disponible próximamente.\nPor favor, contacte al administrador para realizar pagos.", 
                "OK");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
