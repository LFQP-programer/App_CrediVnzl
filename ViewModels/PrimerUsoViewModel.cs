using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class PrimerUsoViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _nombreNegocio = string.Empty;
        private string _nombreAdmin = string.Empty;
        private string _nombreUsuario = string.Empty;
        private string _password = string.Empty;
        private string _confirmarPassword = string.Empty;
        private string _telefono = string.Empty;
        private string _email = string.Empty;
        private string _direccion = string.Empty;
        private bool _estaOcupado;

        public string NombreNegocio
        {
            get => _nombreNegocio;
            set { _nombreNegocio = value; OnPropertyChanged(); }
        }

        public string NombreAdmin
        {
            get => _nombreAdmin;
            set { _nombreAdmin = value; OnPropertyChanged(); }
        }

        // Alias para compatibilidad con XAML existente
        public string NombreCompleto
        {
            get => _nombreAdmin;
            set { _nombreAdmin = value; OnPropertyChanged(); OnPropertyChanged(nameof(NombreAdmin)); }
        }

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

        public string ConfirmarPassword
        {
            get => _confirmarPassword;
            set { _confirmarPassword = value; OnPropertyChanged(); }
        }

        public string Telefono
        {
            get => _telefono;
            set { _telefono = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Direccion
        {
            get => _direccion;
            set { _direccion = value; OnPropertyChanged(); }
        }

        public bool EstaOcupado
        {
            get => _estaOcupado;
            set { _estaOcupado = value; OnPropertyChanged(); }
        }

        public ICommand CrearAdminCommand { get; }
        public ICommand EliminarDatosCommand { get; }

        public PrimerUsoViewModel(AuthService authService)
        {
            _authService = authService;
            CrearAdminCommand = new Command(async () => await OnRegistrarAsync());
            EliminarDatosCommand = new Command(async () => await OnEliminarDatosAsync());
            
            // Valores por defecto
            NombreNegocio = "Mi Negocio de Préstamos";
            Direccion = "";
        }

        private async Task OnEliminarDatosAsync()
        {
            var confirmar = await Application.Current!.MainPage!.DisplayAlert(
                "?? Eliminar Datos",
                "¿Estás seguro de eliminar TODOS los datos existentes?\n\n" +
                "Esto incluye:\n" +
                "• Configuración del negocio\n" +
                "• Cuenta de administrador\n" +
                "• Todos los clientes\n" +
                "• Todos los préstamos\n" +
                "• Todo el historial\n\n" +
                "Esta acción NO se puede deshacer.",
                "Sí, eliminar todo",
                "Cancelar");

            if (!confirmar)
                return;

            EstaOcupado = true;

            try
            {
                var exito = await _authService.EliminarDatosExistentesAsync();

                if (exito)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "? Datos Eliminados",
                        "Todos los datos han sido eliminados.\n\nAhora puedes crear una nueva cuenta de administrador.",
                        "OK");
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error",
                        "No se pudieron eliminar los datos. Intenta cerrar y abrir la app nuevamente.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error eliminando datos: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnRegistrarAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreCompleto) ||
                string.IsNullOrWhiteSpace(NombreUsuario) ||
                string.IsNullOrWhiteSpace(Telefono) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Complete todos los campos obligatorios", "OK");
                return;
            }

            if (Password != ConfirmarPassword)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
                return;
            }

            if (Password.Length < 6)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "La contraseña debe tener al menos 6 caracteres", "OK");
                return;
            }

            EstaOcupado = true;

            try
            {
                var (exito, mensaje) = await _authService.RegistrarAdminAsync(
                    NombreNegocio,
                    NombreCompleto,
                    Telefono,
                    Email,
                    Direccion,
                    NombreUsuario,
                    Password
                );

                if (exito)
                {
                    // Hacer login automático después de crear la cuenta
                    var (loginExito, loginMensaje, usuario) = await _authService.LoginAsync(NombreUsuario, Password);
                    
                    if (loginExito && usuario != null)
                    {
                        await Application.Current!.MainPage!.DisplayAlert(
                            "? Configuración Completa", 
                            "Tu cuenta de administrador ha sido creada exitosamente.", 
                            "OK");
                        
                        // Ir directo al dashboard de admin
                        await Shell.Current.GoToAsync("//dashboard");
                    }
                    else
                    {
                        // Si falla el login automático, ir a página de login
                        await Application.Current!.MainPage!.DisplayAlert(
                            "Cuenta Creada", 
                            "Tu cuenta ha sido creada. Por favor, inicia sesión.", 
                            "OK");
                        await Shell.Current.GoToAsync("//login");
                    }
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registrando admin: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
