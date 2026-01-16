using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Services;
using App_CrediVnzl.Helpers;

namespace App_CrediVnzl.ViewModels
{
    public class CambiarContrasenaAdminViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        
        private string _contrasenaActual = string.Empty;
        private string _contrasenaNueva = string.Empty;
        private string _contrasenaConfirmar = string.Empty;
        private bool _isLoading;
        private bool _mostrarContrasenaActual;
        private bool _mostrarContrasenaNueva;
        private bool _mostrarContrasenaConfirmar;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string ContrasenaActual
        {
            get => _contrasenaActual;
            set { _contrasenaActual = value; OnPropertyChanged(); }
        }

        public string ContrasenaNueva
        {
            get => _contrasenaNueva;
            set { _contrasenaNueva = value; OnPropertyChanged(); }
        }

        public string ContrasenaConfirmar
        {
            get => _contrasenaConfirmar;
            set { _contrasenaConfirmar = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public bool MostrarContrasenaActual
        {
            get => _mostrarContrasenaActual;
            set { _mostrarContrasenaActual = value; OnPropertyChanged(); OnPropertyChanged(nameof(IconoContrasenaActual)); }
        }

        public bool MostrarContrasenaNueva
        {
            get => _mostrarContrasenaNueva;
            set { _mostrarContrasenaNueva = value; OnPropertyChanged(); OnPropertyChanged(nameof(IconoContrasenaNueva)); }
        }

        public bool MostrarContrasenaConfirmar
        {
            get => _mostrarContrasenaConfirmar;
            set { _mostrarContrasenaConfirmar = value; OnPropertyChanged(); OnPropertyChanged(nameof(IconoContrasenaConfirmar)); }
        }

        public string IconoContrasenaActual => MostrarContrasenaActual ? IconHelper.Eye : IconHelper.Lock;
        public string IconoContrasenaNueva => MostrarContrasenaNueva ? IconHelper.Eye : IconHelper.Lock;
        public string IconoContrasenaConfirmar => MostrarContrasenaConfirmar ? IconHelper.Eye : IconHelper.Lock;

        public ICommand CambiarContrasenaCommand { get; }
        public ICommand ToggleContrasenaActualCommand { get; }
        public ICommand ToggleContrasenaNuevaCommand { get; }
        public ICommand ToggleContrasenaConfirmarCommand { get; }

        public CambiarContrasenaAdminViewModel(AuthService authService)
        {
            _authService = authService;
            
            CambiarContrasenaCommand = new Command(async () => await OnCambiarContrasenaAsync());
            ToggleContrasenaActualCommand = new Command(() => MostrarContrasenaActual = !MostrarContrasenaActual);
            ToggleContrasenaNuevaCommand = new Command(() => MostrarContrasenaNueva = !MostrarContrasenaNueva);
            ToggleContrasenaConfirmarCommand = new Command(() => MostrarContrasenaConfirmar = !MostrarContrasenaConfirmar);
        }

        private async Task OnCambiarContrasenaAsync()
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(ContrasenaActual))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Ingresa tu contrase�a actual",
                    "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(ContrasenaNueva))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Ingresa una nueva contrase�a",
                    "OK");
                return;
            }

            if (ContrasenaNueva.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "La nueva contrase�a debe tener al menos 6 caracteres",
                    "OK");
                return;
            }

            if (ContrasenaNueva != ContrasenaConfirmar)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Las contrase�as no coinciden",
                    "OK");
                return;
            }

            try
            {
                IsLoading = true;

                // Verificar contrase�a actual (por ahora usamos "admin" como contrase�a por defecto)
                if (ContrasenaActual != "admin")
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "La contrase�a actual es incorrecta",
                        "OK");
                    return;
                }

                // TODO: Aqu� guardar�as la nueva contrase�a en la configuraci�n
                await Task.Delay(500); // Simulaci�n

                await Application.Current.MainPage.DisplayAlert(
                    $"{IconHelper.Success} �xito",
                    "Tu contrase�a ha sido cambiada correctamente",
                    "OK");

                // Limpiar campos
                ContrasenaActual = string.Empty;
                ContrasenaNueva = string.Empty;
                ContrasenaConfirmar = string.Empty;

                // Volver atr�s
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"No se pudo cambiar la contrase�a: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
