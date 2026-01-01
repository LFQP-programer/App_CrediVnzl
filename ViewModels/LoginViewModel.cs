using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _nombreUsuario = string.Empty;
        private string _password = string.Empty;
        private bool _recordarme;
        private bool _estaOcupado;

        public string NombreUsuario
        {
            get => _nombreUsuario;
            set { _nombreUsuario = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public bool RecordarMe
        {
            get => _recordarme;
            set { _recordarme = value; OnPropertyChanged(); }
        }

        public bool EstaOcupado
        {
            get => _estaOcupado;
            set { _estaOcupado = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegresarCommand { get; }

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await OnLoginAsync());
            RegresarCommand = new Command(async () => await OnRegresarAsync());
        }

        private async Task OnLoginAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreUsuario) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Ingrese usuario y contraseña", "OK");
                return;
            }

            EstaOcupado = true;

            try
            {
                var (exito, mensaje, usuario) = await _authService.LoginAsync(NombreUsuario, Password);

                if (exito && usuario != null)
                {
                    System.Diagnostics.Debug.WriteLine($"*** Login exitoso para: {usuario.NombreUsuario} (Rol: {usuario.Rol}) ***");
                    System.Diagnostics.Debug.WriteLine($"*** RecordarMe está: {RecordarMe} ***");
                    
                    // Guardar o limpiar credenciales según el estado del CheckBox
                    if (RecordarMe)
                    {
                        System.Diagnostics.Debug.WriteLine("*** Guardando credenciales en Preferences ***");
                        Preferences.Set("recordar_usuario", true);
                        Preferences.Set("ultimo_usuario", NombreUsuario);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("*** Limpiando credenciales de Preferences ***");
                        Preferences.Remove("recordar_usuario");
                        Preferences.Remove("ultimo_usuario");
                    }

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
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en login: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error al iniciar sesión: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnRegresarAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** LoginViewModel - Regresando a Bienvenida ***");
                
                if (Shell.Current != null)
                {
                    System.Diagnostics.Debug.WriteLine("*** Usando Shell.GoToAsync ***");
                    await Shell.Current.GoToAsync("//bienvenida");
                }
                else if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    System.Diagnostics.Debug.WriteLine("*** Usando NavigationPage.PopAsync ***");
                    await navPage.PopAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnRegresarAsync: {ex.Message} ***");
            }
        }

        public void CargarCredencialesGuardadas()
        {
            // Cargar estado de "Recordar mis datos"
            bool recordarUsuario = Preferences.Get("recordar_usuario", false);
            
            System.Diagnostics.Debug.WriteLine("?????????????????????????????????????????????????");
            System.Diagnostics.Debug.WriteLine("?  CARGANDO CREDENCIALES GUARDADAS              ?");
            System.Diagnostics.Debug.WriteLine("?????????????????????????????????????????????????");
            System.Diagnostics.Debug.WriteLine($"? Recordar usuario: {recordarUsuario}");
            
            if (recordarUsuario)
            {
                string ultimoUsuario = Preferences.Get("ultimo_usuario", string.Empty);
                System.Diagnostics.Debug.WriteLine($"? Último usuario guardado: '{ultimoUsuario}'");
                
                if (!string.IsNullOrWhiteSpace(ultimoUsuario))
                {
                    NombreUsuario = ultimoUsuario;
                    RecordarMe = true;
                    System.Diagnostics.Debug.WriteLine("? Usuario y CheckBox cargados correctamente");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("? Usuario vacío, limpiando preferences");
                    Preferences.Remove("recordar_usuario");
                    Preferences.Remove("ultimo_usuario");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("? No hay credenciales guardadas");
                // Asegurarse de que el CheckBox esté desmarcado
                RecordarMe = false;
                NombreUsuario = string.Empty;
            }
            
            System.Diagnostics.Debug.WriteLine("?????????????????????????????????????????????????");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
