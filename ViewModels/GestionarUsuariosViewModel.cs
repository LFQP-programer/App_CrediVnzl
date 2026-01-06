using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class GestionarUsuariosViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        private readonly WhatsAppService _whatsAppService;
        private Cliente? _clienteSeleccionado;
        private bool _estaOcupado;

        public ObservableCollection<Cliente> ClientesSinUsuario { get; set; } = new();
        public ObservableCollection<Usuario> UsuariosClientes { get; set; } = new();
        public ObservableCollection<Usuario> SolicitudesPendientes { get; set; } = new();

        public Cliente? ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set { _clienteSeleccionado = value; OnPropertyChanged(); }
        }

        public bool EstaOcupado
        {
            get => _estaOcupado;
            set { _estaOcupado = value; OnPropertyChanged(); }
        }

        public ICommand CrearUsuarioCommand { get; }
        public ICommand DesactivarUsuarioCommand { get; }
        public ICommand RefrescarCommand { get; }
        public ICommand AprobarSolicitudCommand { get; }
        public ICommand RechazarSolicitudCommand { get; }
        public ICommand RegenerarPasswordCommand { get; }
        public ICommand VolverCommand { get; }

        public GestionarUsuariosViewModel(DatabaseService databaseService, AuthService authService, WhatsAppService whatsAppService)
        {
            _databaseService = databaseService;
            _authService = authService;
            _whatsAppService = whatsAppService;

            CrearUsuarioCommand = new Command(async () => await OnCrearUsuarioAsync());
            DesactivarUsuarioCommand = new Command<Usuario>(async (usuario) => await OnDesactivarUsuarioAsync(usuario));
            RefrescarCommand = new Command(async () => await LoadDataAsync());
            AprobarSolicitudCommand = new Command<Usuario>(async (usuario) => await OnAprobarSolicitudAsync(usuario));
            RechazarSolicitudCommand = new Command<Usuario>(async (usuario) => await OnRechazarSolicitudAsync(usuario));
            RegenerarPasswordCommand = new Command<Usuario>(async (usuario) => await OnRegenerarPasswordAsync(usuario));
            VolverCommand = new Command(async () => await OnVolverAsync());
        }

        public async Task LoadDataAsync()
        {
            try
            {
                EstaOcupado = true;

                // Cargar todos los clientes
                var todosClientes = await _databaseService.GetClientesAsync();
                
                // Cargar usuarios de clientes
                var usuariosClientes = await _databaseService.GetUsuariosClientesAsync();

                // Cargar solicitudes pendientes
                var todosUsuarios = await _databaseService.GetUsuariosAsync();
                SolicitudesPendientes.Clear();
                foreach (var usuario in todosUsuarios.Where(u => u.Estado == EstadosUsuario.Pendiente))
                {
                    SolicitudesPendientes.Add(usuario);
                }
                
                // Filtrar clientes que no tienen usuario
                ClientesSinUsuario.Clear();
                foreach (var cliente in todosClientes)
                {
                    var tieneUsuario = usuariosClientes.Any(u => u.ClienteId == cliente.Id);
                    if (!tieneUsuario)
                    {
                        ClientesSinUsuario.Add(cliente);
                    }
                }

                // Cargar usuarios existentes activos
                UsuariosClientes.Clear();
                foreach (var usuario in usuariosClientes.Where(u => u.Activo))
                {
                    UsuariosClientes.Add(usuario);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando datos: {ex.Message}");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnCrearUsuarioAsync()
        {
            if (ClienteSeleccionado == null)
            {
                await Shell.Current.DisplayAlert("Error", "Seleccione un cliente", "OK");
                return;
            }

            EstaOcupado = true;

            try
            {
                // Crear usuario con DNI y contraseña automática
                var (exito, mensaje, passwordGenerada) = await _authService.RegistrarClienteUsuarioAsync(
                    ClienteSeleccionado.Id,
                    "", // No se usa, el método usa el DNI internamente
                    ""); // No se usa, se genera automáticamente

                if (exito && passwordGenerada != null)
                {
                    // Preparar mensaje de WhatsApp
                    var mensajeWhatsApp = $"¡Bienvenido a CrediVnzl! ??\n\n" +
                                         $"Tu cuenta ha sido creada exitosamente.\n\n" +
                                         $"?? *Tus credenciales de acceso:*\n" +
                                         $"Usuario: *{ClienteSeleccionado.NumeroDocumento}* (tu documento)\n" +
                                         $"Contraseña: *{passwordGenerada}*\n\n" +
                                         $"?? Descarga la app y accede con estas credenciales.\n\n" +
                                         $"?? Por seguridad, te recomendamos cambiar tu contraseña desde la opción 'Mi Cuenta' en la app.\n\n" +
                                         $"¡Gracias por confiar en nosotros!";

                    // Preguntar si desea enviar WhatsApp
                    var enviarWhatsApp = await Shell.Current.DisplayAlert(
                        "? Usuario Creado",
                        $"Usuario creado para: {ClienteSeleccionado.NombreCompleto}\n\n" +
                        $"?? Usuario (DNI): {ClienteSeleccionado.NumeroDocumento}\n" +
                        $"?? Contraseña: {passwordGenerada}\n\n" +
                        $"¿Deseas enviar las credenciales por WhatsApp ahora?",
                        "Sí, enviar WhatsApp",
                        "No, solo copiar");

                    if (enviarWhatsApp)
                    {
                        // Enviar mensaje por WhatsApp
                        var enviado = await _whatsAppService.EnviarMensajeAsync(ClienteSeleccionado.Telefono, mensajeWhatsApp);

                        if (enviado)
                        {
                            await Shell.Current.DisplayAlert(
                                "WhatsApp Enviado",
                                "Se ha abierto WhatsApp con el mensaje de credenciales. Por favor, envíalo al cliente.",
                                "OK");
                        }
                        else
                        {
                            await Shell.Current.DisplayAlert(
                                "Error",
                                "No se pudo abrir WhatsApp. Verifica el número de teléfono del cliente.",
                                "OK");
                        }
                    }
                    else
                    {
                        // Solo mostrar las credenciales para copiar manualmente
                        await Shell.Current.DisplayAlert(
                            "Credenciales Generadas",
                            $"Usuario: {ClienteSeleccionado.NumeroDocumento}\n" +
                            $"Contraseña: {passwordGenerada}\n\n" +
                            $"Comunica estas credenciales al cliente.",
                            "OK");
                    }
                    
                    // Limpiar selección
                    ClienteSeleccionado = null;
                    
                    // Recargar datos
                    await LoadDataAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando usuario: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnAprobarSolicitudAsync(Usuario usuario)
        {
            if (usuario == null) return;

            var confirmar = await Shell.Current.DisplayAlert(
                "Aprobar Solicitud",
                $"¿Aprobar la solicitud de {usuario.NombreCompleto}?\n\n" +
                $"Se generará una contraseña automática de 6 dígitos.",
                "Sí, aprobar",
                "Cancelar");

            if (!confirmar) return;

            try
            {
                EstaOcupado = true;
                
                var (exito, mensaje) = await _authService.AprobarClienteAsync(usuario.Id);

                if (exito)
                {
                    // Extraer la contraseña del mensaje
                    var password = mensaje.Replace("Cliente aprobado exitosamente. Contraseña temporal: ", "");
                    
                    // Obtener cliente para el teléfono
                    var cliente = await _databaseService.GetClienteAsync(usuario.ClienteId ?? 0);
                    
                    if (cliente != null)
                    {
                        // Preparar mensaje de WhatsApp
                        var mensajeWhatsApp = $"¡Bienvenido a CrediVnzl! ??\n\n" +
                                             $"Tu solicitud ha sido *aprobada* exitosamente.\n\n" +
                                             $"?? *Tus credenciales de acceso:*\n" +
                                             $"Usuario: *{usuario.NombreUsuario}* (tu DNI)\n" +
                                             $"Contraseña: *{password}*\n\n" +
                                             $"?? Descarga la app y accede con estas credenciales.\n\n" +
                                             $"?? Por seguridad, te recomendamos cambiar tu contraseña desde la opción 'Mi Cuenta' en la app.\n\n" +
                                             $"¡Gracias por confiar en nosotros!";

                        // Preguntar si desea enviar WhatsApp
                        var enviarWhatsApp = await Shell.Current.DisplayAlert(
                            "? Cliente Aprobado",
                            $"Cliente: {usuario.NombreCompleto}\n\n" +
                            $"?? Usuario (DNI): {usuario.NombreUsuario}\n" +
                            $"?? Contraseña: {password}\n\n" +
                            $"¿Deseas enviar las credenciales por WhatsApp ahora?",
                            "Sí, enviar WhatsApp",
                            "No, solo copiar");

                        if (enviarWhatsApp)
                        {
                            // Enviar mensaje por WhatsApp
                            var enviado = await _whatsAppService.EnviarMensajeAsync(cliente.Telefono, mensajeWhatsApp);

                            if (enviado)
                            {
                                await Shell.Current.DisplayAlert(
                                    "WhatsApp Enviado",
                                    "Se ha abierto WhatsApp con el mensaje de credenciales. Por favor, envíalo al cliente.",
                                    "OK");
                            }
                            else
                            {
                                await Shell.Current.DisplayAlert(
                                    "Error",
                                    "No se pudo abrir WhatsApp. Verifica el número de teléfono del cliente.",
                                    "OK");
                            }
                        }
                        else
                        {
                            // Solo mostrar las credenciales
                            await Shell.Current.DisplayAlert(
                                "Credenciales",
                                $"Usuario: {usuario.NombreUsuario}\n" +
                                $"Contraseña: {password}\n\n" +
                                $"Comunica estas credenciales al cliente.",
                                "OK");
                        }
                    }
                    
                    await LoadDataAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aprobando solicitud: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnRechazarSolicitudAsync(Usuario usuario)
        {
            if (usuario == null) return;

            var confirmar = await Shell.Current.DisplayAlert(
                "Rechazar Solicitud",
                $"¿Rechazar la solicitud de {usuario.NombreCompleto}?",
                "Sí, rechazar",
                "Cancelar");

            if (!confirmar) return;

            try
            {
                EstaOcupado = true;
                
                var (exito, mensaje) = await _authService.RechazarClienteAsync(usuario.Id, "Rechazado por el administrador");

                if (exito)
                {
                    await Shell.Current.DisplayAlert("Solicitud Rechazada", 
                        $"La solicitud de {usuario.NombreCompleto} ha sido rechazada.", 
                        "OK");
                    
                    await LoadDataAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error rechazando solicitud: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnRegenerarPasswordAsync(Usuario usuario)
        {
            if (usuario == null) return;

            var confirmar = await Shell.Current.DisplayAlert(
                "Regenerar Contraseña",
                $"¿Generar una nueva contraseña para {usuario.NombreCompleto}?\n\n" +
                $"La contraseña actual dejará de funcionar.",
                "Sí, regenerar",
                "Cancelar");

            if (!confirmar) return;

            try
            {
                EstaOcupado = true;
                
                var (exito, mensaje, passwordGenerada) = await _authService.RegenerarPasswordClienteAsync(usuario.Id);

                if (exito && passwordGenerada != null)
                {
                    // Obtener cliente para el teléfono
                    var cliente = await _databaseService.GetClienteAsync(usuario.ClienteId ?? 0);
                    
                    if (cliente != null)
                    {
                        // Preparar mensaje de WhatsApp
                        var mensajeWhatsApp = $"?? *Contraseña Regenerada - CrediVnzl*\n\n" +
                                             $"Hola {usuario.NombreCompleto},\n\n" +
                                             $"Se ha generado una nueva contraseña para tu cuenta.\n\n" +
                                             $"?? *Tus nuevas credenciales:*\n" +
                                             $"Usuario: *{usuario.NombreUsuario}* (tu DNI)\n" +
                                             $"Nueva Contraseña: *{passwordGenerada}*\n\n" +
                                             $"?? Tu contraseña anterior ya no funcionará.\n\n" +
                                             $"?? Recuerda que puedes cambiar tu contraseña desde la app en cualquier momento.\n\n" +
                                             $"Si no solicitaste este cambio, contacta inmediatamente al administrador.";

                        // Preguntar si desea enviar WhatsApp
                        var enviarWhatsApp = await Shell.Current.DisplayAlert(
                            "?? Contraseña Regenerada",
                            $"Cliente: {usuario.NombreCompleto}\n\n" +
                            $"?? Usuario (DNI): {usuario.NombreUsuario}\n" +
                            $"?? Nueva Contraseña: {passwordGenerada}\n\n" +
                            $"¿Deseas enviar la nueva contraseña por WhatsApp?",
                            "Sí, enviar WhatsApp",
                            "No, solo copiar");

                        if (enviarWhatsApp)
                        {
                            // Enviar mensaje por WhatsApp
                            var enviado = await _whatsAppService.EnviarMensajeAsync(cliente.Telefono, mensajeWhatsApp);

                            if (enviado)
                            {
                                await Shell.Current.DisplayAlert(
                                    "WhatsApp Enviado",
                                    "Se ha abierto WhatsApp con la nueva contraseña. Por favor, envíalo al cliente.",
                                    "OK");
                            }
                            else
                            {
                                await Shell.Current.DisplayAlert(
                                    "Error",
                                    "No se pudo abrir WhatsApp. Verifica el número de teléfono del cliente.",
                                    "OK");
                            }
                        }
                        else
                        {
                            // Solo mostrar la contraseña
                            await Shell.Current.DisplayAlert(
                                "Nueva Contraseña",
                                $"Usuario: {usuario.NombreUsuario}\n" +
                                $"Nueva Contraseña: {passwordGenerada}\n\n" +
                                $"Comunica la nueva contraseña al cliente.",
                                "OK");
                        }
                    }
                    
                    await LoadDataAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error regenerando contraseña: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnDesactivarUsuarioAsync(Usuario usuario)
        {
            if (usuario == null) return;

            var confirmar = await Shell.Current.DisplayAlert(
                "Confirmar",
                $"¿Desea desactivar el usuario de {usuario.NombreCompleto}?",
                "Sí",
                "No");

            if (!confirmar) return;

            try
            {
                usuario.Activo = false;
                await _databaseService.SaveUsuarioAsync(usuario);
                
                await Shell.Current.DisplayAlert("Éxito", "Usuario desactivado", "OK");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error desactivando usuario: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        private async Task OnVolverAsync()
        {
            try
            {
                // Navegar de vuelta al dashboard
                await Shell.Current.GoToAsync("//dashboard");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navegando al dashboard: {ex.Message}");
                // Si falla la navegación por Shell, intentar con el método alternativo
                await Shell.Current.GoToAsync("..");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
