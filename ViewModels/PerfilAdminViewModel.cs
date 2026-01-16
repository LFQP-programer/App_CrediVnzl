using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class PerfilAdminViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private readonly DatabaseService _databaseService;
        
        private string _nombreCompleto = string.Empty;
        private string _usuario = string.Empty;
        private string _email = string.Empty;
        private string _telefono = string.Empty;
        private DateTime _fechaCreacion = DateTime.Now;
        private bool _isEditing;
        private bool _isLoading;
        
        // Estad�sticas
        private int _totalClientes;
        private int _totalPrestamos;
        private decimal _capitalTotal;
        private decimal _gananciasTotal;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string NombreCompleto
        {
            get => _nombreCompleto;
            set { _nombreCompleto = value; OnPropertyChanged(); }
        }

        public string Usuario
        {
            get => _usuario;
            set { _usuario = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Telefono
        {
            get => _telefono;
            set { _telefono = value; OnPropertyChanged(); }
        }

        public DateTime FechaCreacion
        {
            get => _fechaCreacion;
            set { _fechaCreacion = value; OnPropertyChanged(); }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNotEditing)); }
        }

        public bool IsNotEditing => !IsEditing;

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        // Propiedades para estad�sticas
        public int TotalClientes
        {
            get => _totalClientes;
            set { _totalClientes = value; OnPropertyChanged(); }
        }

        public int TotalPrestamos
        {
            get => _totalPrestamos;
            set { _totalPrestamos = value; OnPropertyChanged(); }
        }

        public string CapitalTotalFormateado => FormatearMoneda(_capitalTotal);
        
        public string GananciasTotalFormateado => FormatearMoneda(_gananciasTotal);

        public ICommand EditarCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }

        public PerfilAdminViewModel(AuthService authService, DatabaseService databaseService)
        {
            _authService = authService;
            _databaseService = databaseService;
            
            EditarCommand = new Command(OnEditar);
            GuardarCommand = new Command(async () => await OnGuardarAsync());
            CancelarCommand = new Command(OnCancelar);
            
            _ = CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            try
            {
                IsLoading = true;
                
                // Cargar datos del administrador
                NombreCompleto = "Administrador CrediVnzl";
                Usuario = "admin";
                Email = "admin@credivnzl.com";
                Telefono = "+51 999 999 999";
                FechaCreacion = DateTime.Now.AddMonths(-6);
                
                // Cargar estad�sticas desde la base de datos
                await CargarEstadisticasAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando datos: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CargarEstadisticasAsync()
        {
            try
            {
                await _databaseService.InitializeAsync();
                
                // Total de clientes
                var clientes = await _databaseService.GetClientesAsync();
                TotalClientes = clientes.Count;
                
                // Total de pr�stamos activos
                var prestamos = await _databaseService.GetPrestamosAsync();
                TotalPrestamos = prestamos.Count(p => p.Estado == "Activo");
                
                // Capital total (suma de capital pendiente en pr�stamos activos)
                _capitalTotal = prestamos.Where(p => p.Estado == "Activo").Sum(p => p.CapitalPendiente);
                OnPropertyChanged(nameof(CapitalTotalFormateado));
                
                // Ganancias totales (suma de intereses acumulados en todos los pr�stamos)
                _gananciasTotal = prestamos.Sum(p => p.InteresAcumulado);
                OnPropertyChanged(nameof(GananciasTotalFormateado));
                
                System.Diagnostics.Debug.WriteLine($"Estad�sticas cargadas - Clientes: {TotalClientes}, Pr�stamos: {TotalPrestamos}, Capital: {_capitalTotal}, Ganancias: {_gananciasTotal}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando estad�sticas: {ex.Message}");
                
                // Valores por defecto en caso de error
                TotalClientes = 0;
                TotalPrestamos = 0;
                _capitalTotal = 0;
                _gananciasTotal = 0;
                OnPropertyChanged(nameof(CapitalTotalFormateado));
                OnPropertyChanged(nameof(GananciasTotalFormateado));
            }
        }

        private string FormatearMoneda(decimal monto)
        {
            if (monto >= 1000000)
            {
                return $"{(monto / 1000000):0.#}M";
            }
            else if (monto >= 1000)
            {
                return $"{(monto / 1000):0.#}K";
            }
            else
            {
                return $"{monto:0}";
            }
        }

        private void OnEditar()
        {
            IsEditing = true;
        }

        private async Task OnGuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreCompleto))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "El nombre completo es obligatorio",
                    "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "El email es obligatorio",
                    "OK");
                return;
            }

            try
            {
                IsLoading = true;

                // TODO: Aqu� guardar�as los datos en la base de datos o configuraci�n
                await Task.Delay(500); // Simulaci�n

                await Application.Current.MainPage.DisplayAlert(
                    "? �xito",
                    "Perfil actualizado correctamente",
                    "OK");

                IsEditing = false;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"No se pudo guardar el perfil: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnCancelar()
        {
            IsEditing = false;
            _ = CargarDatosAsync(); // Recargar datos originales
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
