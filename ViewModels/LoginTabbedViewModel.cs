using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class LoginTabbedViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        
        // Admin fields
        private string _adminNombreUsuario = string.Empty;
        private string _adminPassword = string.Empty;
        private bool _adminRecordarMe;
        
        // Cliente fields
        private string _clienteNombreUsuario = string.Empty;
        private string _clientePassword = string.Empty;
        
        // Common fields
        private bool _estaOcupado;
        private bool _isAdminTab = false; // Start with Cliente mode by default

        // Admin Properties
        public string AdminNombreUsuario
        {
            get => _adminNombreUsuario;
            set { _adminNombreUsuario = value; OnPropertyChanged(); }
        }

        public string AdminPassword
        {
            get => _adminPassword;
            set { _adminPassword = value; OnPropertyChanged(); }
        }

        public bool AdminRecordarMe
        {
            get => _adminRecordarMe;
            set { _adminRecordarMe = value; OnPropertyChanged(); }
        }

        // Cliente Properties
        public string ClienteNombreUsuario
        {
            get => _clienteNombreUsuario;
            set { _clienteNombreUsuario = value; OnPropertyChanged(); }
        }

        public string ClientePassword
        {
            get => _clientePassword;
            set { _clientePassword = value; OnPropertyChanged(); }
        }

        // Common Properties
        public bool EstaOcupado
        {
            get => _estaOcupado;
            set { _estaOcupado = value; OnPropertyChanged(); }
        }

        public bool IsAdminTab
        {
            get => _isAdminTab;
            set { _isAdminTab = value; OnPropertyChanged(); }
        }

        public ICommand AdminLoginCommand { get; }
        public ICommand ClienteLoginCommand { get; }

        public LoginTabbedViewModel(AuthService authService)
        {
            _authService = authService;
            AdminLoginCommand = new Command(async () => await OnAdminLoginAsync());
            ClienteLoginCommand = new Command(async () => await OnClienteLoginAsync());
        }

        public void SetInitialMode(bool isAdminMode)
        {
            IsAdminTab = isAdminMode;
        }

        private async Task OnAdminLoginAsync()
        {
            if (string.IsNullOrWhiteSpace(AdminNombreUsuario) || string.IsNullOrWhiteSpace(AdminPassword))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Ingrese usuario y contraseña", "OK");
                return;
            }

            EstaOcupado = true;

            try
            {
                var (exito, mensaje, usuario) = await _authService.LoginAsync(AdminNombreUsuario, AdminPassword);

                if (exito && usuario != null)
                {
                    System.Diagnostics.Debug.WriteLine($"*** Login Admin exitoso para: {usuario.NombreUsuario} (Rol: {usuario.Rol}) ***");
                    
                    // Guardar o limpiar credenciales según el estado del CheckBox
                    if (AdminRecordarMe)
                    {
                        System.Diagnostics.Debug.WriteLine("*** Guardando credenciales Admin en Preferences ***");
                        Preferences.Set("recordar_usuario", true);
                        Preferences.Set("ultimo_usuario", AdminNombreUsuario);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("*** Limpiando credenciales de Preferences ***");
                        Preferences.Remove("recordar_usuario");
                        Preferences.Remove("ultimo_usuario");
                    }

                    await NavigateAfterLogin(usuario);
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en admin login: {ex.Message} ***");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error al iniciar sesión: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnClienteLoginAsync()
        {
            if (string.IsNullOrWhiteSpace(ClienteNombreUsuario) || string.IsNullOrWhiteSpace(ClientePassword))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Ingrese DNI y contraseña", "OK");
                return;
            }

            EstaOcupado = true;

            try
            {
                var (exito, mensaje, usuario) = await _authService.LoginAsync(ClienteNombreUsuario, ClientePassword);

                if (exito && usuario != null)
                {
                    System.Diagnostics.Debug.WriteLine($"*** Login Cliente exitoso para: {usuario.NombreUsuario} (Rol: {usuario.Rol}) ***");
                    
                    await NavigateAfterLogin(usuario);
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en cliente login: {ex.Message} ***");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error al iniciar sesión: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task NavigateAfterLogin(dynamic usuario)
        {
            // Si ya estamos en un Shell, usarlo; si no, crear uno nuevo
            AppShell shellToUse;
            
            if (Shell.Current is AppShell currentShell)
            {
                System.Diagnostics.Debug.WriteLine("*** Usando AppShell actual ***");
                shellToUse = currentShell;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("*** Creando nuevo AppShell después del login ***");
                shellToUse = new AppShell();
                Application.Current!.MainPage = shellToUse;
            }
            
            // Mostrar menú del Flyout después del login
            shellToUse.MostrarMenuDespuesDelLogin();
            
            // Navegar según el rol
            if (usuario.EsAdmin)
            {
                System.Diagnostics.Debug.WriteLine("*** Navegando a dashboard (Admin) ***");
                await shellToUse.GoToAsync("//dashboard");
            }
            else if (usuario.EsCliente)
            {
                System.Diagnostics.Debug.WriteLine("*** Navegando a dashboardcliente (Cliente) ***");
                await shellToUse.GoToAsync("//dashboardcliente");
            }
            
            System.Diagnostics.Debug.WriteLine("*** Navegación post-login completada ***");
        }

        public void CargarCredencialesGuardadas()
        {
            // Cargar estado de "Recordar mis datos" solo para Admin
            bool recordarUsuario = Preferences.Get("recordar_usuario", false);
            
            System.Diagnostics.Debug.WriteLine("*** CARGANDO CREDENCIALES GUARDADAS (Tabbed) ***");
            System.Diagnostics.Debug.WriteLine($"Recordar usuario: {recordarUsuario}");
            
            if (recordarUsuario)
            {
                string ultimoUsuario = Preferences.Get("ultimo_usuario", string.Empty);
                System.Diagnostics.Debug.WriteLine($"Último usuario guardado: '{ultimoUsuario}'");
                
                if (!string.IsNullOrWhiteSpace(ultimoUsuario))
                {
                    AdminNombreUsuario = ultimoUsuario;
                    AdminRecordarMe = true;
                    System.Diagnostics.Debug.WriteLine("Usuario Admin y CheckBox cargados correctamente");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Usuario vacío, limpiando preferences");
                    Preferences.Remove("recordar_usuario");
                    Preferences.Remove("ultimo_usuario");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No hay credenciales guardadas");
                AdminRecordarMe = false;
                AdminNombreUsuario = string.Empty;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}