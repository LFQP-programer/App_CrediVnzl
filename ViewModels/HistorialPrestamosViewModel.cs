using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    [QueryProperty(nameof(ClienteId), "clienteId")]
    public class HistorialPrestamosViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private int _clienteId;
        private string _clienteNombre = string.Empty;
        private int _totalPrestamos;
        private int _prestamosActivos;
        private int _prestamosCompletados;
        private decimal _totalPrestadoHistorico;
        private decimal _totalInteresGenerado;
        private string _filtroSeleccionado = "Todos";

        public ObservableCollection<Prestamo> TodosPrestamos { get; set; } = new();
        public ObservableCollection<Prestamo> PrestamosFiltrados { get; set; } = new();

        public int ClienteId
        {
            get => _clienteId;
            set
            {
                _clienteId = value;
                OnPropertyChanged();
            }
        }

        public string ClienteNombre
        {
            get => _clienteNombre;
            set
            {
                _clienteNombre = value;
                OnPropertyChanged();
            }
        }

        public int TotalPrestamos
        {
            get => _totalPrestamos;
            set
            {
                _totalPrestamos = value;
                OnPropertyChanged();
            }
        }

        public int PrestamosActivos
        {
            get => _prestamosActivos;
            set
            {
                _prestamosActivos = value;
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

        public decimal TotalPrestadoHistorico
        {
            get => _totalPrestadoHistorico;
            set
            {
                _totalPrestadoHistorico = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalInteresGenerado
        {
            get => _totalInteresGenerado;
            set
            {
                _totalInteresGenerado = value;
                OnPropertyChanged();
            }
        }

        public string FiltroSeleccionado
        {
            get => _filtroSeleccionado;
            set
            {
                _filtroSeleccionado = value;
                OnPropertyChanged();
            }
        }

        public ICommand FiltrarCommand { get; }
        public ICommand VerDetallePrestamoCommand { get; }
        public ICommand ToggleExpandirCommand { get; }

        public HistorialPrestamosViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            FiltrarCommand = new Command<string>(async (filtro) => await FiltrarPrestamos(filtro));
            VerDetallePrestamoCommand = new Command<Prestamo>(async (prestamo) => await VerDetallePrestamo(prestamo));
            ToggleExpandirCommand = new Command<Prestamo>(ToggleExpandir);
        }

        public async Task LoadDataAsync()
        {
            try
            {
                var cliente = await _databaseService.GetClienteAsync(ClienteId);
                if (cliente != null)
                {
                    ClienteNombre = cliente.NombreCompleto;
                }

                var prestamos = await _databaseService.GetPrestamosByClienteAsync(ClienteId);
                
                TodosPrestamos.Clear();
                foreach (var prestamo in prestamos)
                {
                    TodosPrestamos.Add(prestamo);
                }

                // Calcular estadisticas
                TotalPrestamos = prestamos.Count;
                PrestamosActivos = prestamos.Count(p => p.Estado == "Activo");
                PrestamosCompletados = prestamos.Count(p => p.Estado == "Completado");
                TotalPrestadoHistorico = prestamos.Sum(p => p.MontoInicial);

                // Calcular interes generado de prestamos completados y activos
                var historialPagos = await _databaseService.GetHistorialPagosByClienteAsync(ClienteId);
                TotalInteresGenerado = historialPagos.Sum(h => h.MontoInteres);

                // Aplicar filtro inicial
                await FiltrarPrestamos(FiltroSeleccionado);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar los datos: {ex.Message}", "OK");
            }
        }

        private async Task FiltrarPrestamos(string filtro)
        {
            FiltroSeleccionado = filtro;

            PrestamosFiltrados.Clear();

            var prestamos = filtro switch
            {
                "Activo" => TodosPrestamos.Where(p => p.Estado == "Activo"),
                "Completado" => TodosPrestamos.Where(p => p.Estado == "Completado"),
                _ => TodosPrestamos
            };

            foreach (var prestamo in prestamos)
            {
                PrestamosFiltrados.Add(prestamo);
            }

            await Task.CompletedTask;
        }

        private async Task VerDetallePrestamo(Prestamo prestamo)
        {
            if (prestamo != null)
            {
                await Shell.Current.GoToAsync($"detalleprestamo?prestamoId={prestamo.Id}");
            }
        }

        private void ToggleExpandir(Prestamo prestamo)
        {
            if (prestamo != null)
            {
                prestamo.Expandido = !prestamo.Expandido;
                
                // Refrescar la colección para actualizar la UI
                var index = PrestamosFiltrados.IndexOf(prestamo);
                if (index >= 0)
                {
                    PrestamosFiltrados.RemoveAt(index);
                    PrestamosFiltrados.Insert(index, prestamo);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
