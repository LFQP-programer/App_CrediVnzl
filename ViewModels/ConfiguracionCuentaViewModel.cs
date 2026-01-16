using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class ConfiguracionCuentaViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private readonly DatabaseService _databaseService;
        
        private string _nombreCompleto = string.Empty;
        private string _telefono = string.Empty;
        private string _email = string.Empty;
        private string _nombreUsuario = string.Empty;
        private string _passwordActual = string.Empty;
        private string _passwordNuevo = string.Empty;
        private string _passwordConfirmar = string.Empty;
        private bool _estaOcupado;

        public string NombreCompleto
        {
            get => _nombreCompleto;
            set { _nombreCompleto = value; OnPropertyChanged(); }
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

        public string NombreUsuario
        {
            get => _nombreUsuario;
            set { _nombreUsuario = value; OnPropertyChanged(); }
        }

        public string PasswordActual
        {
            get => _passwordActual;
            set { _passwordActual = value; OnPropertyChanged(); }
        }

        public string PasswordNuevo
        {
            get => _passwordNuevo;
            set { _passwordNuevo = value; OnPropertyChanged(); }
        }

        public string PasswordConfirmar
        {
            get => _passwordConfirmar;
            set { _passwordConfirmar = value; OnPropertyChanged(); }
        }

        public bool EstaOcupado
        {
            get => _estaOcupado;
            set { _estaOcupado = value; OnPropertyChanged(); }
        }

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }

        public ConfiguracionCuentaViewModel(AuthService authService, DatabaseService databaseService)
        {
            _authService = authService;
            _databaseService = databaseService;
            
            GuardarCommand = new Command(async () => await OnGuardarAsync());
            CancelarCommand = new Command(async () => await OnCancelarAsync());
        }

        public async Task CargarDatosUsuarioAsync()
        {
            try
            {
                var usuario = _authService.UsuarioActual;
                if (usuario != null)
                {
                    NombreCompleto = usuario.NombreCompleto;
                    Telefono = usuario.Telefono ?? string.Empty;
                    Email = usuario.Email ?? string.Empty;
                    NombreUsuario = usuario.NombreUsuario;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando datos: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert("Error", "Error al cargar datos del usuario", "OK");
            }
        }

        private async Task OnGuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreCompleto))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "El nombre completo es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NombreUsuario))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "El nombre de usuario es requerido", "OK");
                return;
            }

            if (!string.IsNullOrWhiteSpace(PasswordNuevo))
            {
                if (string.IsNullOrWhiteSpace(PasswordActual))
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", "Debes ingresar tu contraseña actual para cambiarla", "OK");
                    return;
                }

                if (PasswordNuevo != PasswordConfirmar)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", "Las contraseñas nuevas no coinciden", "OK");
                    return;
                }

                if (PasswordNuevo.Length < 6)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", "La nueva contraseña debe tener al menos 6 caracteres", "OK");
                    return;
                }
            }

            EstaOcupado = true;

            try
            {
                var usuario = _authService.UsuarioActual;
                if (usuario == null)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", "No hay sesión activa", "OK");
                    return;
                }

                usuario.NombreCompleto = NombreCompleto;
                usuario.Telefono = Telefono;
                usuario.Email = Email;
                usuario.NombreUsuario = NombreUsuario;

                if (!string.IsNullOrWhiteSpace(PasswordNuevo))
                {
                    var (exito, mensaje) = await _authService.CambiarPasswordAsync(PasswordActual, PasswordNuevo);
                    if (!exito)
                    {
                        await Application.Current!.MainPage!.DisplayAlert("Error", mensaje, "OK");
                        return;
                    }
                }

                await _databaseService.SaveUsuarioAsync(usuario);

                await Application.Current!.MainPage!.DisplayAlert("Éxito", "Datos actualizados correctamente", "OK");
                
                PasswordActual = string.Empty;
                PasswordNuevo = string.Empty;
                PasswordConfirmar = string.Empty;

                await Shell.Current.GoToAsync("//dashboard");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error al guardar cambios: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnCancelarAsync()
        {
            await Shell.Current.GoToAsync("//dashboard");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
