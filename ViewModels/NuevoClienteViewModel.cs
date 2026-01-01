using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class NuevoClienteViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        private readonly WhatsAppService _whatsAppService;
        private Cliente _cliente = new();
        private bool _isEditing;

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

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }

        public NuevoClienteViewModel(DatabaseService databaseService, AuthService authService, WhatsAppService whatsAppService)
        {
            _databaseService = databaseService;
            _authService = authService;
            _whatsAppService = whatsAppService;
            GuardarCommand = new Command(async () => await GuardarClienteAsync());
            CancelarCommand = new Command(async () => await CancelarAsync());
        }

        public void Initialize(Cliente? cliente = null)
        {
            if (cliente != null)
            {
                Cliente = cliente;
                IsEditing = true;
            }
            else
            {
                Cliente = new Cliente();
                IsEditing = false;
            }
        }

        private async Task GuardarClienteAsync()
        {
            if (string.IsNullOrWhiteSpace(Cliente.NombreCompleto))
            {
                await Shell.Current.DisplayAlertAsync("Error", "El nombre completo es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Cliente.Telefono))
            {
                await Shell.Current.DisplayAlertAsync("Error", "El teléfono es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Cliente.Cedula))
            {
                await Shell.Current.DisplayAlertAsync("Error", "La cédula/DNI es requerida", "OK");
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
                var clienteGuardado = await _databaseService.GetClienteByCedulaAsync(Cliente.Cedula);

                if (clienteGuardado == null)
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo encontrar el cliente guardado", "OK");
                    return;
                }

                // Generar credenciales (DNI como usuario + contraseña de 6 dígitos)
                var (exito, mensaje, passwordGenerada) = await _authService.RegistrarClienteUsuarioAsync(
                    clienteGuardado.Id,
                    "", // No usado, se usa el DNI internamente
                    ""); // No usado, se genera automáticamente

                if (exito && passwordGenerada != null)
                {
                    // Preparar mensaje de WhatsApp
                    var mensajeWhatsApp = $"¡Bienvenido a CrediVzla! ??\n\n" +
                                         $"Tu cuenta ha sido creada exitosamente.\n\n" +
                                         $"?? *Tus credenciales de acceso:*\n" +
                                         $"Usuario: *{clienteGuardado.Cedula}* (tu DNI)\n" +
                                         $"Contraseña: *{passwordGenerada}*\n\n" +
                                         $"?? Descarga la app y accede con estas credenciales.\n\n" +
                                         $"?? Por seguridad, te recomendamos cambiar tu contraseña desde la opción 'Mi Cuenta' en la app.\n\n" +
                                         $"¡Gracias por confiar en nosotros!";

                    // Mostrar diálogo con credenciales y preguntar si enviar WhatsApp
                    var enviarWhatsApp = await Shell.Current.DisplayAlert(
                        "? Credenciales Generadas",
                        $"Usuario creado para: {clienteGuardado.NombreCompleto}\n\n" +
                        $"?? Usuario (DNI): {clienteGuardado.Cedula}\n" +
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
                                $"Usuario: {clienteGuardado.Cedula}\n" +
                                $"Contraseña: {passwordGenerada}",
                                "OK");
                        }
                    }
                    else
                    {
                        // Solo mostrar las credenciales para copiar manualmente
                        await Shell.Current.DisplayAlert(
                            "Credenciales Generadas",
                            $"Usuario: {clienteGuardado.Cedula}\n" +
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
