using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    [QueryProperty(nameof(ClienteId), "clienteId")]
    public class DetalleClienteViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private Cliente _cliente;
        private int _clienteId;
        private int _prestamosCompletados;
        private decimal _capitalPendienteTotal;
        private decimal _totalAdeudadoHoy;

        public ObservableCollection<Prestamo> PrestamosActivos { get; set; } = new();

        public Cliente Cliente
        {
            get => _cliente;
            set
            {
                _cliente = value;
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
            }
        }

        public int PrestamosCompletados
        {
            get => _prestamosCompletados;
            set
            {
                _prestamosCompletados = value;
                OnPropertyChanged();
            }
        }

        public decimal CapitalPendienteTotal
        {
            get => _capitalPendienteTotal;
            set
            {
                _capitalPendienteTotal = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalAdeudadoHoy
        {
            get => _totalAdeudadoHoy;
            set
            {
                _totalAdeudadoHoy = value;
                OnPropertyChanged();
            }
        }

        public ICommand NuevoPrestamoCommand { get; }
        public ICommand RegistrarPagoCommand { get; }

        public DetalleClienteViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _cliente = new Cliente();
            
            NuevoPrestamoCommand = new Command(async () => await NuevoPrestamo());
            RegistrarPagoCommand = new Command<Prestamo>(async (prestamo) => await RegistrarPago(prestamo));
        }

        public async Task LoadDataAsync()
        {
            try
            {
                // Actualizar intereses de prestamos activos primero
                await _databaseService.ActualizarInteresesPrestamosActivosAsync();
                
                var cliente = await _databaseService.GetClienteAsync(ClienteId);
                if (cliente != null)
                {
                    Cliente = cliente;
                }

                var prestamos = await _databaseService.GetPrestamosByClienteAsync(ClienteId);
                
                PrestamosActivos.Clear();
                foreach (var prestamo in prestamos.Where(p => p.Estado == "Activo"))
                {
                    PrestamosActivos.Add(prestamo);
                }

                PrestamosCompletados = prestamos.Count(p => p.Estado == "Completado");
                CapitalPendienteTotal = prestamos.Where(p => p.Estado == "Activo").Sum(p => p.CapitalPendiente);
                TotalAdeudadoHoy = prestamos.Where(p => p.Estado == "Activo").Sum(p => p.TotalAdeudado);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar los datos: {ex.Message}", "OK");
            }
        }

        private async Task NuevoPrestamo()
        {
            await Shell.Current.GoToAsync($"nuevoprestamo?clienteId={ClienteId}");
        }

        private async Task RegistrarPago(Prestamo prestamo)
        {
            if (prestamo != null)
            {
                await Shell.Current.GoToAsync($"registrarpago?prestamoId={prestamo.Id}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
