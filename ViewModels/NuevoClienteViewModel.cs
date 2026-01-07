using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;
using System.Collections.ObjectModel;
using Microsoft.Maui.Storage;

namespace App_CrediVnzl.ViewModels
{
    public class NuevoClienteViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        private readonly WhatsAppService _whatsAppService;
        private Cliente _cliente = new();
        private bool _isEditing;
        private bool _isDniSelected = true;
        private ObservableCollection<Cliente> _clientesParaAval = new();
        private Cliente? _avalClienteSeleccionado;
        private bool _usarClienteComoAval;
        private string? _imagenReciboPath;

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

        public bool IsDniSelected
        {
            get => _isDniSelected;
            set
            {
                _isDniSelected = value;
                Cliente.TipoDocumento = value ? "DNI" : "Carnet de Extranjería";
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCarnetSelected));
                OnPropertyChanged(nameof(Cliente));
            }
        }

        public bool IsCarnetSelected
        {
            get => !_isDniSelected;
            set
            {
                _isDniSelected = !value;
                Cliente.TipoDocumento = value ? "Carnet de Extranjería" : "DNI";
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsDniSelected));
                OnPropertyChanged(nameof(Cliente));
            }
        }

        public ObservableCollection<Cliente> ClientesParaAval
        {
            get => _clientesParaAval;
            set
            {
                _clientesParaAval = value;
                OnPropertyChanged();
            }
        }

        public Cliente? AvalClienteSeleccionado
        {
            get => _avalClienteSeleccionado;
            set
            {
                _avalClienteSeleccionado = value;
                if (value != null)
                {
                    Cliente.AvalClienteId = value.Id;
                    // Limpiar datos manuales del aval
                    Cliente.AvalNombres = "";
                    Cliente.AvalApellidos = "";
                    Cliente.AvalTelefono = "";
                    Cliente.AvalNumeroDocumento = "";
                }
                OnPropertyChanged();
            }
        }

        public bool UsarClienteComoAval
        {
            get => _usarClienteComoAval;
            set
            {
                _usarClienteComoAval = value;
                if (value)
                {
                    // Limpiar datos manuales del aval
                    Cliente.AvalNombres = "";
                    Cliente.AvalApellidos = "";
                    Cliente.AvalTelefono = "";
                    Cliente.AvalNumeroDocumento = "";
                }
                else
                {
                    // Limpiar cliente seleccionado
                    Cliente.AvalClienteId = null;
                    AvalClienteSeleccionado = null;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(UsarDatosManualAval));
            }
        }

        public bool UsarDatosManualAval => !_usarClienteComoAval;

        public string? ImagenReciboPath
        {
            get => _imagenReciboPath;
            set
            {
                _imagenReciboPath = value;
                Cliente.RutaImagenRecibo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TieneImagenRecibo));
                OnPropertyChanged(nameof(TextoBotonImagen));
            }
        }

        public bool TieneImagenRecibo => !string.IsNullOrEmpty(ImagenReciboPath);

        public string TextoBotonImagen => TieneImagenRecibo ? "Cambiar Imagen" : "Seleccionar Imagen";

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand SeleccionarImagenCommand { get; }
        public ICommand VerImagenCommand { get; }
        public ICommand EliminarImagenCommand { get; }

        private string _numeroTelefono = string.Empty;

        public string NumeroTelefono
        {
            get => _numeroTelefono;
            set
            {
                // Solo permitir números y espacios, máximo 11 caracteres (con espacios)
                var cleanValue = new string(value.Where(c => char.IsDigit(c)).ToArray());
                
                if (cleanValue.Length <= 9)
                {
                    // Formatear el número con espacios: 999 999 999
                    if (cleanValue.Length > 6)
                    {
                        _numeroTelefono = $"{cleanValue.Substring(0, 3)} {cleanValue.Substring(3, 3)} {cleanValue.Substring(6)}";
                    }
                    else if (cleanValue.Length > 3)
                    {
                        _numeroTelefono = $"{cleanValue.Substring(0, 3)} {cleanValue.Substring(3)}";
                    }
                    else
                    {
                        _numeroTelefono = cleanValue;
                    }
                    
                    // Actualizar el teléfono completo en el modelo Cliente
                    if (cleanValue.Length == 9)
                    {
                        Cliente.Telefono = $"+51{cleanValue}";
                    }
                    else if (cleanValue.Length > 0)
                    {
                        Cliente.Telefono = $"+51{cleanValue}";
                    }
                    else
                    {
                        Cliente.Telefono = "";
                    }
                    
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Cliente));
                }
            }
        }

        public NuevoClienteViewModel(DatabaseService databaseService, AuthService authService, WhatsAppService whatsAppService)
        {
            _databaseService = databaseService;
            _authService = authService;
            _whatsAppService = whatsAppService;
            GuardarCommand = new Command(async () => await GuardarClienteAsync());
            CancelarCommand = new Command(async () => await CancelarAsync());
            SeleccionarImagenCommand = new Command(async () => await SeleccionarImagenAsync());
            VerImagenCommand = new Command(async () => await VerImagenAsync());
            EliminarImagenCommand = new Command(EliminarImagen);
        }

        public async void Initialize(Cliente? cliente = null)
        {
            if (cliente != null)
            {
                Cliente = cliente;
                IsEditing = true;
                IsDniSelected = cliente.TipoDocumento == "DNI";
                ImagenReciboPath = cliente.RutaImagenRecibo;
                
                // Extraer el número de teléfono sin el código de país
                if (!string.IsNullOrEmpty(cliente.Telefono))
                {
                    var telefonoLimpio = cliente.Telefono.Replace("+51", "").Replace(" ", "").Replace("-", "");
                    if (telefonoLimpio.Length == 9 && telefonoLimpio.All(char.IsDigit))
                    {
                        NumeroTelefono = telefonoLimpio;
                    }
                    else
                    {
                        NumeroTelefono = "";
                    }
                }
                else
                {
                    NumeroTelefono = "";
                }
                
                if (cliente.AvalClienteId.HasValue)
                {
                    UsarClienteComoAval = true;
                    var avalCliente = await _databaseService.GetClienteByIdAsync(cliente.AvalClienteId.Value);
                    AvalClienteSeleccionado = avalCliente;
                }
                else if (!string.IsNullOrEmpty(cliente.AvalNombres))
                {
                    UsarClienteComoAval = false;
                }
            }
            else
            {
                Cliente = new Cliente { TipoDocumento = "DNI" };
                IsEditing = false;
                IsDniSelected = true;
                UsarClienteComoAval = false;
                NumeroTelefono = "";
            }

            await LoadClientesParaAvalAsync();
        }

        private async Task LoadClientesParaAvalAsync()
        {
            try
            {
                var clientes = await _databaseService.GetAllClientesAsync();
                
                // Filtrar el cliente actual si estamos editando
                if (IsEditing)
                {
                    clientes = clientes.Where(c => c.Id != Cliente.Id).ToList();
                }
                
                ClientesParaAval.Clear();
                foreach (var cliente in clientes)
                {
                    ClientesParaAval.Add(cliente);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando clientes para aval: {ex.Message}");
            }
        }

        private async Task SeleccionarImagenAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Seleccionar recibo de luz o agua"
                });

                if (result != null)
                {
                    // Crear directorio para almacenar las imágenes
                    var appDataPath = FileSystem.AppDataDirectory;
                    var imageDir = Path.Combine(appDataPath, "ImagenesRecibos");
                    Directory.CreateDirectory(imageDir);

                    // Generar nombre único para la imagen
                    var fileName = $"recibo_{Cliente.Id}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                    var filePath = Path.Combine(imageDir, fileName);

                    // Copiar la imagen seleccionada
                    using var sourceStream = await result.OpenReadAsync();
                    using var destStream = File.Create(filePath);
                    await sourceStream.CopyToAsync(destStream);

                    ImagenReciboPath = filePath;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo seleccionar la imagen: {ex.Message}", "OK");
            }
        }

        private async Task VerImagenAsync()
        {
            if (!string.IsNullOrEmpty(ImagenReciboPath) && File.Exists(ImagenReciboPath))
            {
                try
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(ImagenReciboPath)
                    });
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"No se pudo abrir la imagen: {ex.Message}", "OK");
                }
            }
        }

        private void EliminarImagen()
        {
            ImagenReciboPath = null;
        }

        private async Task GuardarClienteAsync()
        {
            // Validaciones
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

            if (string.IsNullOrWhiteSpace(Cliente.Telefono))
            {
                await Shell.Current.DisplayAlert("Error", "El teléfono es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Cliente.NumeroDocumento))
            {
                await Shell.Current.DisplayAlert("Error", $"El {Cliente.TipoDocumento} es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NumeroTelefono) || NumeroTelefono.Replace(" ", "").Length != 9)
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar un número de teléfono válido de 9 dígitos", "OK");
                return;
            }

            try
            {
                // Guardar cliente
                await _databaseService.SaveClienteAsync(Cliente);

                // Si es un nuevo cliente (no edición), preguntar si generar credenciales
                if (!IsEditing)
                {
                    bool generarCredenciales = await Shell.Current.DisplayAlert(
                        "? Cliente Guardado",
                        $"Cliente '{Cliente.NombreCompleto}' guardado correctamente.\n\n" +
                        $"¿Deseas generar credenciales automáticamente y enviar por WhatsApp ahora?",
                        "Sí, generar ahora",
                        "Más tarde");

                    if (generarCredenciales)
                    {
                        await GenerarYEnviarCredencialesAsync();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert(
                            "Información",
                            "Puedes generar las credenciales más tarde desde 'Gestionar Usuarios'.",
                            "OK");
                    }
                }
                else
                {
                    // Si es edición, solo mostrar mensaje de éxito
                    await Shell.Current.DisplayAlert("Éxito", "Cliente actualizado correctamente", "OK");
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo guardar el cliente: {ex.Message}", "OK");
            }
        }

        private async Task GenerarYEnviarCredencialesAsync()
        {
            try
            {
                // Obtener el cliente recién guardado (necesitamos el ID)
                var clienteGuardado = await _databaseService.GetClienteByDocumentoAsync(Cliente.NumeroDocumento);

                if (clienteGuardado == null)
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo encontrar el cliente guardado", "OK");
                    return;
                }

                // Generar credenciales (documento como usuario + contraseña de 6 dígitos)
                var (exito, mensaje, passwordGenerada) = await _authService.RegistrarClienteUsuarioAsync(
                    clienteGuardado.Id,
                    "", // No usado, se usa el documento internamente
                    ""); // No usado, se genera automáticamente

                if (exito && passwordGenerada != null)
                {
                    // Preparar mensaje de WhatsApp
                    var mensajeWhatsApp = $"¡Bienvenido a CrediVzla! ??\n\n" +
                                         $"Tu cuenta ha sido creada exitosamente.\n\n" +
                                         $"?? *Tus credenciales de acceso:*\n" +
                                         $"Usuario: *{clienteGuardado.NumeroDocumento}* (tu documento)\n" +
                                         $"Contraseña: *{passwordGenerada}*\n\n" +
                                         $"?? Descarga la app y accede con estas credenciales.\n\n" +
                                         $"?? Por seguridad, te recomendamos cambiar tu contraseña desde la opción 'Mi Cuenta' en la app.\n\n" +
                                         $"¡Gracias por confiar en nosotros!";

                    // Mostrar diálogo con credenciales y preguntar si enviar WhatsApp
                    var enviarWhatsApp = await Shell.Current.DisplayAlert(
                        "?? Credenciales Generadas",
                        $"Usuario creado para: {clienteGuardado.NombreCompleto}\n\n" +
                        $"?? Usuario: {clienteGuardado.NumeroDocumento}\n" +
                        $"?? Contraseña: {passwordGenerada}\n\n" +
                        $"¿Deseas enviar las credenciales por WhatsApp ahora?",
                        "Sí, enviar WhatsApp",
                        "No, solo copiar");

                    if (enviarWhatsApp)
                    {
                        // Enviar mensaje por WhatsApp
                        var enviado = await _whatsAppService.EnviarMensajeAsync(clienteGuardado.Telefono, mensajeWhatsApp);

                        if (enviado)
                        {
                            await Shell.Current.DisplayAlert(
                                "WhatsApp Abierto",
                                "Se ha abierto WhatsApp con el mensaje de credenciales. Por favor, envíalo al cliente.",
                                "OK");
                        }
                        else
                        {
                            await Shell.Current.DisplayAlert(
                                "Error",
                                "No se pudo abrir WhatsApp. Verifica el número de teléfono del cliente.\n\n" +
                                $"Credenciales generadas:\n" +
                                $"Usuario: {clienteGuardado.NumeroDocumento}\n" +
                                $"Contraseña: {passwordGenerada}",
                                "OK");
                        }
                    }
                    else
                    {
                        // Solo mostrar las credenciales para copiar manualmente
                        await Shell.Current.DisplayAlert(
                            "Credenciales Generadas",
                            $"Usuario: {clienteGuardado.NumeroDocumento}\n" +
                            $"Contraseña: {passwordGenerada}\n\n" +
                            $"Comunica estas credenciales al cliente.",
                            "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generando credenciales: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Error al generar credenciales: {ex.Message}", "OK");
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
