using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;
using App_CrediVnzl.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace App_CrediVnzl.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _isLoading;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await OnLoginClicked());
        }

        public void CargarCredencialesGuardadas()
        {
            // Intentar cargar credenciales guardadas de Preferences
            try
            {
                if (Preferences.ContainsKey("saved_username"))
                {
                    Username = Preferences.Get("saved_username", string.Empty);
                }
                
                if (Preferences.ContainsKey("saved_password"))
                {
                    Password = Preferences.Get("saved_password", string.Empty);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando credenciales guardadas: {ex.Message}");
            }
        }

        private async Task OnLoginClicked()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Por favor ingrese usuario y contrase�a",
                    "OK");
                return;
            }

            IsLoading = true;

            try
            {
                System.Diagnostics.Debug.WriteLine($"*** LoginViewModel - Intentando login con usuario: {Username} ***");

                if (_authService.Login(Username, Password, TipoUsuario.Administrador))
                {
                    System.Diagnostics.Debug.WriteLine("*** LoginViewModel - Login exitoso ***");
                    
                    // Navegar al dashboard usando Shell
                    System.Diagnostics.Debug.WriteLine("*** LoginViewModel - Navegando al dashboard ***");
                    await Shell.Current.GoToAsync("//dashboard");
                    
                    System.Diagnostics.Debug.WriteLine("*** LoginViewModel - Navegaci�n completada ***");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("*** LoginViewModel - Login fallido ***");
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "Usuario o contrase�a incorrectos",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en LoginViewModel.OnLoginClicked: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");

                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Error al iniciar sesi�n: {ex.Message}",
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
