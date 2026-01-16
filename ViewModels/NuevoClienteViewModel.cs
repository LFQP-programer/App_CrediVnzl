using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections.ObjectModel;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class NuevoClienteViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly WhatsAppService _whatsAppService;
        private Cliente _cliente = new();
        private bool _isEditing;
        private string _tipoAval = "Nueva persona";
        private bool _tieneImagenRecibo;
        private string? _rutaImagenRecibo;
        private ObservableCollection<Cliente> _clientesDisponibles = new();
        private Cliente? _clienteAvalSeleccionado;
        private bool _esTipoDNI = true;
        private bool _esTipoCarnet;

        public Cliente Cliente
        {
            get => _cliente;
            set
            {
                _cliente = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PageTitle));
            }
        }

        public string PageTitle => IsEditing ? "Editar Cliente" : "Nuevo Cliente";

        public string TipoAval
        {
            get => _tipoAval;
            set
            {
                _tipoAval = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EsAvalClienteExistente));
                OnPropertyChanged(nameof(EsAvalNuevo));
            }
        }

        public bool EsAvalClienteExistente
        {
            get => TipoAval == "Cliente existente";
            set
            {
                if (value)
                {
                    TipoAval = "Cliente existente";
                }
            }
        }

        public bool EsAvalNuevo
        {
            get => TipoAval == "Nueva persona";
            set
            {
                if (value)
                {
                    TipoAval = "Nueva persona";
                }
            }
        }

        public bool EsTipoDNI
        {
            get => _esTipoDNI;
            set
            {
                _esTipoDNI = value;
                if (value)
                {
                    Cliente.TipoDocumento = "DNI";
                    _esTipoCarnet = false;
                    OnPropertyChanged(nameof(EsTipoCarnet));
                }
                OnPropertyChanged();
            }
        }

        public bool EsTipoCarnet
        {
            get => _esTipoCarnet;
            set
            {
                _esTipoCarnet = value;
                if (value)
                {
                    Cliente.TipoDocumento = "Carnet de extranjeria";
                    _esTipoDNI = false;
                    OnPropertyChanged(nameof(EsTipoDNI));
                }
                OnPropertyChanged();
            }
        }

        public bool TieneImagenRecibo
        {
            get => _tieneImagenRecibo;
            set
            {
                _tieneImagenRecibo = value;
                OnPropertyChanged();
            }
        }

        public string? RutaImagenRecibo
        {
            get => _rutaImagenRecibo;
            set
            {
                _rutaImagenRecibo = value;
                TieneImagenRecibo = !string.IsNullOrEmpty(value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Cliente> ClientesDisponibles
        {
            get => _clientesDisponibles;
            set
            {
                _clientesDisponibles = value;
                OnPropertyChanged();
            }
        }

        public Cliente? ClienteAvalSeleccionado
        {
            get => _clienteAvalSeleccionado;
            set
            {
                _clienteAvalSeleccionado = value;
                if (value != null)
                {
                    Cliente.AvalClienteId = value.Id;
                }
                OnPropertyChanged();
            }
        }

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand SeleccionarImagenCommand { get; }

        public NuevoClienteViewModel(DatabaseService databaseService, WhatsAppService whatsAppService)
        {
            _databaseService = databaseService;
            _whatsAppService = whatsAppService;
            GuardarCommand = new Command(async () => await GuardarClienteAsync());
            CancelarCommand = new Command(async () => await CancelarAsync());
            SeleccionarImagenCommand = new Command(async () => await SeleccionarImagenAsync());
            
            // Inicializar valores por defecto
            Cliente.TipoDocumento = "DNI";
            Cliente.EstadoCliente = "Activo";
        }

        public async void Initialize(Cliente? cliente = null)
        {
            if (cliente != null)
            {
                Cliente = cliente;
                IsEditing = true;
                RutaImagenRecibo = cliente.RutaImagenRecibo;
                
                // Configurar tipo de documento
                EsTipoDNI = cliente.TipoDocumento == "DNI";
                EsTipoCarnet = cliente.TipoDocumento == "Carnet de extranjeria";
                
                if (cliente.AvalClienteId.HasValue)
                {
                    TipoAval = "Cliente existente";
                }
                else
                {
                    TipoAval = "Nueva persona";
                }
            }
            else
            {
                Cliente = new Cliente
                {
                    TipoDocumento = "DNI",
                    EstadoCliente = "Activo"
                };
                IsEditing = false;
                EsTipoDNI = true;
                EsTipoCarnet = false;
                TipoAval = "Nueva persona";
            }

            // Cargar clientes disponibles para aval
            await CargarClientesDisponiblesAsync();
        }

        private async Task CargarClientesDisponiblesAsync()
        {
            try
            {
                var clientes = await _databaseService.GetClientesAsync();
                ClientesDisponibles = new ObservableCollection<Cliente>(clientes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando clientes: {ex.Message}");
            }
        }

        private async Task SeleccionarImagenAsync()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Seleccionar imagen del recibo"
                });

                if (result != null)
                {
                    // Copiar la imagen a la carpeta de la aplicaci�n
                    var fileName = $"recibo_{Cliente.NumeroDocumento}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                    var targetPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
                    
                    using var stream = await result.OpenReadAsync();
                    using var fileStream = File.Create(targetPath);
                    await stream.CopyToAsync(fileStream);
                    
                    Cliente.RutaImagenRecibo = targetPath;
                    RutaImagenRecibo = targetPath;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo seleccionar la imagen: {ex.Message}", "OK");
            }
        }

        private async Task GuardarClienteAsync()
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(Cliente.NumeroDocumento))
            {
                await Shell.Current.DisplayAlert("Error", "El numero de documento es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Cliente.Nombres))
            {
                await Shell.Current.DisplayAlert("Error", "Los nombres son requeridos", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Cliente.Apellidos))
            {
                await Shell.Current.DisplayAlert("Error", "Los apellidos son requeridos", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Cliente.NumeroCelular))
            {
                await Shell.Current.DisplayAlert("Error", "El numero de celular es requerido", "OK");
                return;
            }

            if (Cliente.NumeroCelular.Length != 9)
            {
                await Shell.Current.DisplayAlert("Error", "El numero de celular debe tener exactamente 9 digitos", "OK");
                return;
            }

            // Validar datos del aval
            if (TipoAval == "Cliente existente" && Cliente.AvalClienteId == null)
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar un cliente como aval", "OK");
                return;
            }

            if (TipoAval == "Nueva persona")
            {
                if (string.IsNullOrWhiteSpace(Cliente.AvalNombres) || 
                    string.IsNullOrWhiteSpace(Cliente.AvalApellidos))
                {
                    await Shell.Current.DisplayAlert("Error", "Los datos del aval son requeridos", "OK");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(Cliente.AvalCelular) && Cliente.AvalCelular.Length != 9)
                {
                    await Shell.Current.DisplayAlert("Error", "El numero de celular del aval debe tener exactamente 9 digitos", "OK");
                    return;
                }
            }

            try
            {
                // Guardar el cliente primero
                await _databaseService.SaveClienteAsync(Cliente);

                // Si no est� editando, preguntar si desea generar contrase�a
                if (!IsEditing)
                {
                    var generarPassword = await Shell.Current.DisplayAlert(
                        "Acceso a la App",
                        "�Desea generar una contrase�a temporal para que el cliente pueda acceder a la aplicaci�n?",
                        "Si",
                        "No");

                    if (generarPassword)
                    {
                        await GenerarYEnviarPasswordAsync();
                    }
                }

                await Shell.Current.DisplayAlert("Exito", "Cliente guardado correctamente", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo guardar el cliente: {ex.Message}", "OK");
            }
        }

        private async Task GenerarYEnviarPasswordAsync()
        {
            try
            {
                // Generar contrase�a temporal de 6 d�gitos
                var random = new Random();
                var passwordTemporal = random.Next(100000, 999999).ToString();

                // Actualizar el cliente con la contrase�a temporal
                Cliente.TieneAccesoApp = true;
                Cliente.PasswordTemporal = passwordTemporal;
                Cliente.RequiereCambioPassword = true;
                Cliente.FechaGeneracionPassword = DateTime.Now;

                await _databaseService.SaveClienteAsync(Cliente);

                // Formatear n�mero de celular para Per� (+51)
                var numeroCompleto = $"51{Cliente.NumeroCelular}";

                // Crear mensaje de WhatsApp
                var mensaje = $"Hola {Cliente.Nombres},\n\n" +
                             $"Tu contrase�a temporal para acceder a CrediVnzl es: *{passwordTemporal}*\n\n" +
                             $"Por seguridad, te recomendamos cambiarla al iniciar sesi�n.\n\n" +
                             $"Puedes ingresar con tu DNI: {Cliente.NumeroDocumento}\n\n" +
                             $"�Bienvenido!";

                // Enviar mensaje por WhatsApp
                var enviado = await _whatsAppService.EnviarMensajeAsync(numeroCompleto, mensaje);

                if (enviado)
                {
                    await Shell.Current.DisplayAlert(
                        "Contrase�a Generada",
                        $"Contrase�a temporal: {passwordTemporal}\n\nSe ha abierto WhatsApp para enviar la contrase�a al cliente.",
                        "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert(
                        "Contrase�a Generada",
                        $"Contrase�a temporal: {passwordTemporal}\n\nNo se pudo abrir WhatsApp. Por favor, env�a esta contrase�a manualmente al cliente.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"Error al generar contrase�a: {ex.Message}",
                    "OK");
            }
        }

        private async Task CancelarAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
