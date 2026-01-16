using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class LoginClienteViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private readonly DatabaseService _databaseService;
        private string _dni = string.Empty;
        private string _password = string.Empty;
        private bool _isLoading;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Dni
        {
            get => _dni;
            set
            {
                _dni = value;
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
        public ICommand VolverCommand { get; }

        public LoginClienteViewModel(AuthService authService, DatabaseService databaseService)
        {
            _authService = authService;
            _databaseService = databaseService;
            LoginCommand = new Command(async () => await OnLoginClicked());
            VolverCommand = new Command(async () => await OnVolverClicked());
        }

        private async Task OnLoginClicked()
        {
            if (string.IsNullOrWhiteSpace(Dni) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Por favor ingrese DNI y contrase�a",
                    "OK");
                return;
            }

            IsLoading = true;

            try
            {
                System.Diagnostics.Debug.WriteLine($"*** LoginClienteViewModel - Buscando cliente con DNI: {Dni} ***");
                
                // Inicializar base de datos
                await _databaseService.InitializeAsync();
                
                // Buscar cliente por DNI
                var clientes = await _databaseService.GetClientesAsync();
                var cliente = clientes.FirstOrDefault(c => c.Cedula == Dni);

                if (cliente == null)
                {
                    System.Diagnostics.Debug.WriteLine("*** LoginClienteViewModel - Cliente no encontrado ***");
                    await Application.Current.MainPage.DisplayAlert(
                        "Cliente no encontrado",
                        "No existe un cliente registrado con este DNI.",
                        "OK");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"*** LoginClienteViewModel - Cliente encontrado: {cliente.NombreCompleto} ***");

                // Verificar si el cliente tiene acceso a la app
                if (!cliente.TieneAccesoApp)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Acceso Denegado",
                        "Este cliente no tiene acceso a la aplicaci�n. Contacte al administrador.",
                        "OK");
                    return;
                }

                // Verificar contrase�a
                if (Password == cliente.PasswordTemporal)
                {
                    System.Diagnostics.Debug.WriteLine("*** LoginClienteViewModel - Contrase�a correcta ***");
                    
                    // Autenticar como cliente
                    _authService.LoginCliente(cliente.Id, cliente.NombreCompleto, cliente.Cedula);
                    
                    // Si requiere cambio de contrase�a, mostrar mensaje
                    if (cliente.RequiereCambioPassword)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Cambio de Contrase�a",
                            "Por seguridad, te recomendamos cambiar tu contrase�a temporal desde tu perfil.",
                            "OK");
                    }
                    
                    // Navegar al dashboard de cliente
                    System.Diagnostics.Debug.WriteLine("*** LoginClienteViewModel - Navegando al dashboard de cliente ***");
                    await Shell.Current.GoToAsync($"//clientedashboard?clienteId={cliente.Id}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("*** LoginClienteViewModel - Contrase�a incorrecta ***");
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "Contrase�a incorrecta",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en LoginClienteViewModel.OnLoginClicked: {ex.Message} ***");
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

        private async Task OnVolverClicked()
        {
            await Shell.Current.GoToAsync("//main");
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
