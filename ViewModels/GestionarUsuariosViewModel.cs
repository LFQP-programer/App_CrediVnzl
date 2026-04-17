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
                await Application.Current!.MainPage!.DisplayAlert("Error", "Seleccione un cliente", "OK");
                return;
            }

            EstaOcupado = true;

            try
            {
                // Crear usuario con DNI y contrase�a autom�tica
                var (exito, mensaje, passwordGenerada) = await _authService.RegistrarClienteUsuarioAsync(
                    ClienteSeleccionado.Id,
                    "", // No se usa, el m�todo usa el DNI internamente
                    ""); // No se usa, se genera autom�ticamente

                if (exito && passwordGenerada != null)
                {
                    // Preparar mensaje de WhatsApp
                    var mensajeWhatsApp = $"�Bienvenido a CrediVnzl! ??\n\n" +
                                         $"Tu cuenta ha sido creada exitosamente.\n\n" +
                                         $"?? *Tus credenciales de acceso:*\n" +
                                         $"Usuario: *{ClienteSeleccionado.Cedula}* (tu DNI)\n" +
                                         $"Contrase�a: *{passwordGenerada}*\n\n" +
                                         $"?? Descarga la app y accede con estas credenciales.\n\n" +
                                         $"?? Por seguridad, te recomendamos cambiar tu contrase�a desde la opci�n 'Mi Cuenta' en la app.\n\n" +
                                         $"�Gracias por confiar en nosotros!";

                    // Preguntar si desea enviar WhatsApp
                    var enviarWhatsApp = await Application.Current!.MainPage!.DisplayAlert(
                        "? Usuario Creado",
                        $"Usuario creado para: {ClienteSeleccionado.NombreCompleto}\n\n" +
                        $"?? Usuario (DNI): {ClienteSeleccionado.Cedula}\n" +
                        $"?? Contrase�a: {passwordGenerada}\n\n" +
                        $"�Deseas enviar las credenciales por WhatsApp ahora?",
                        "S�, enviar WhatsApp",
                        "No, solo copiar");

                    if (enviarWhatsApp)
                    {
                        // Enviar mensaje por WhatsApp
                        var enviado = await _whatsAppService.EnviarMensajeAsync(ClienteSeleccionado.Telefono, mensajeWhatsApp);

                        if (enviado)
                        {
                            await Application.Current!.MainPage!.DisplayAlert(
                                "WhatsApp Enviado",
                                "Se ha abierto WhatsApp con el mensaje de credenciales. Por favor, env�alo al cliente.",
                                "OK");
                        }
                        else
                        {
                            await Application.Current!.MainPage!.DisplayAlert(
                                "Error",
                                "No se pudo abrir WhatsApp. Verifica el n�mero de tel�fono del cliente.",
                                "OK");
                        }
                    }
                    else
                    {
                        // Solo mostrar las credenciales para copiar manualmente
                        await Application.Current!.MainPage!.DisplayAlert(
                            "Credenciales Generadas",
                            $"Usuario: {ClienteSeleccionado.Cedula}\n" +
                            $"Contrase�a: {passwordGenerada}\n\n" +
                            $"Comunica estas credenciales al cliente.",
                            "OK");
                    }
                    
                    // Limpiar selecci�n
                    ClienteSeleccionado = null;
                    
                    // Recargar datos
                    await LoadDataAsync();
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando usuario: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnAprobarSolicitudAsync(Usuario usuario)
        {
            if (usuario == null) return;

            var confirmar = await Application.Current!.MainPage!.DisplayAlert(
                "Aprobar Solicitud",
                $"�Aprobar la solicitud de {usuario.NombreCompleto}?\n\n" +
                $"Se generar� una contrase�a autom�tica de 6 d�gitos.",
                "S�, aprobar",
                "Cancelar");

            if (!confirmar) return;

            try
            {
                EstaOcupado = true;
                
                var (exito, mensaje) = await _authService.AprobarClienteAsync(usuario.Id);

                if (exito)
                {
                    // Extraer la contrase�a del mensaje
                    var password = mensaje.Replace("Cliente aprobado exitosamente. Contrase�a temporal: ", "");
                    
                    // Obtener cliente para el tel�fono
                    var cliente = await _databaseService.GetClienteAsync(usuario.ClienteId ?? 0);
                    
                    if (cliente != null)
                    {
                        // Preparar mensaje de WhatsApp
                        var mensajeWhatsApp = $"�Bienvenido a CrediVnzl! ??\n\n" +
                                             $"Tu solicitud ha sido *aprobada* exitosamente.\n\n" +
                                             $"?? *Tus credenciales de acceso:*\n" +
                                             $"Usuario: *{usuario.NombreUsuario}* (tu DNI)\n" +
                                             $"Contrase�a: *{password}*\n\n" +
                                             $"?? Descarga la app y accede con estas credenciales.\n\n" +
                                             $"?? Por seguridad, te recomendamos cambiar tu contrase�a desde la opci�n 'Mi Cuenta' en la app.\n\n" +
                                             $"�Gracias por confiar en nosotros!";

                        // Preguntar si desea enviar WhatsApp
                        var enviarWhatsApp = await Application.Current!.MainPage!.DisplayAlert(
                            "? Cliente Aprobado",
                            $"Cliente: {usuario.NombreCompleto}\n\n" +
                            $"?? Usuario (DNI): {usuario.NombreUsuario}\n" +
                            $"?? Contrase�a: {password}\n\n" +
                            $"�Deseas enviar las credenciales por WhatsApp ahora?",
                            "S�, enviar WhatsApp",
                            "No, solo copiar");

                        if (enviarWhatsApp)
                        {
                            // Enviar mensaje por WhatsApp
                            var enviado = await _whatsAppService.EnviarMensajeAsync(cliente.Telefono, mensajeWhatsApp);

                            if (enviado)
                            {
                                await Application.Current!.MainPage!.DisplayAlert(
                                    "WhatsApp Enviado",
                                    "Se ha abierto WhatsApp con el mensaje de credenciales. Por favor, env�alo al cliente.",
                                    "OK");
                            }
                            else
                            {
                                await Application.Current!.MainPage!.DisplayAlert(
                                    "Error",
                                    "No se pudo abrir WhatsApp. Verifica el n�mero de tel�fono del cliente.",
                                    "OK");
                            }
                        }
                        else
                        {
                            // Solo mostrar las credenciales
                            await Application.Current!.MainPage!.DisplayAlert(
                                "Credenciales",
                                $"Usuario: {usuario.NombreUsuario}\n" +
                                $"Contrase�a: {password}\n\n" +
                                $"Comunica estas credenciales al cliente.",
                                "OK");
                        }
                    }
                    
                    await LoadDataAsync();
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aprobando solicitud: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnRechazarSolicitudAsync(Usuario usuario)
        {
            if (usuario == null) return;

            var confirmar = await Application.Current!.MainPage!.DisplayAlert(
                "Rechazar Solicitud",
                $"�Rechazar la solicitud de {usuario.NombreCompleto}?",
                "S�, rechazar",
                "Cancelar");

            if (!confirmar) return;

            try
            {
                EstaOcupado = true;
                
                var (exito, mensaje) = await _authService.RechazarClienteAsync(usuario.Id, "Rechazado por el administrador");

                if (exito)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Solicitud Rechazada", 
                        $"La solicitud de {usuario.NombreCompleto} ha sido rechazada.", 
                        "OK");
                    
                    await LoadDataAsync();
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error rechazando solicitud: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnRegenerarPasswordAsync(Usuario usuario)
        {
            if (usuario == null) return;

            var confirmar = await Application.Current!.MainPage!.DisplayAlert(
                "Regenerar Contrase�a",
                $"�Generar una nueva contrase�a para {usuario.NombreCompleto}?\n\n" +
                $"La contrase�a actual dejar� de funcionar.",
                "S�, regenerar",
                "Cancelar");

            if (!confirmar) return;

            try
            {
                EstaOcupado = true;
                
                var (exito, mensaje, passwordGenerada) = await _authService.RegenerarPasswordClienteAsync(usuario.Id);

                if (exito && passwordGenerada != null)
                {
                    // Obtener cliente para el tel�fono
                    var cliente = await _databaseService.GetClienteAsync(usuario.ClienteId ?? 0);
                    
                    if (cliente != null)
                    {
                        // Preparar mensaje de WhatsApp
                        var mensajeWhatsApp = $"?? *Contrase�a Regenerada - CrediVnzl*\n\n" +
                                             $"Hola {usuario.NombreCompleto},\n\n" +
                                             $"Se ha generado una nueva contrase�a para tu cuenta.\n\n" +
                                             $"?? *Tus nuevas credenciales:*\n" +
                                             $"Usuario: *{usuario.NombreUsuario}* (tu DNI)\n" +
                                             $"Nueva Contrase�a: *{passwordGenerada}*\n\n" +
                                             $"?? Tu contrase�a anterior ya no funcionar�.\n\n" +
                                             $"?? Recuerda que puedes cambiar tu contrase�a desde la app en cualquier momento.\n\n" +
                                             $"Si no solicitaste este cambio, contacta inmediatamente al administrador.";

                        // Preguntar si desea enviar WhatsApp
                        var enviarWhatsApp = await Application.Current!.MainPage!.DisplayAlert(
                            "? Contrase�a Regenerada",
                            $"Cliente: {usuario.NombreCompleto}\n\n" +
                            $"?? Usuario (DNI): {usuario.NombreUsuario}\n" +
                            $"?? Nueva Contrase�a: {passwordGenerada}\n\n" +
                            $"�Deseas enviar la nueva contrase�a por WhatsApp?",
                            "S�, enviar WhatsApp",
                            "No, solo copiar");

                        if (enviarWhatsApp)
                        {
                            // Enviar mensaje por WhatsApp
                            var enviado = await _whatsAppService.EnviarMensajeAsync(cliente.Telefono, mensajeWhatsApp);

                            if (enviado)
                            {
                                await Application.Current!.MainPage!.DisplayAlert(
                                    "WhatsApp Enviado",
                                    "Se ha abierto WhatsApp con la nueva contrase�a. Por favor, env�alo al cliente.",
                                    "OK");
                            }
                            else
                            {
                                await Application.Current!.MainPage!.DisplayAlert(
                                    "Error",
                                    "No se pudo abrir WhatsApp. Verifica el n�mero de tel�fono del cliente.",
                                    "OK");
                            }
                        }
                        else
                        {
                            // Solo mostrar la contrase�a
                            await Application.Current!.MainPage!.DisplayAlert(
                                "Nueva Contrase�a",
                                $"Usuario: {usuario.NombreUsuario}\n" +
                                $"Nueva Contrase�a: {passwordGenerada}\n\n" +
                                $"Comunica la nueva contrase�a al cliente.",
                                "OK");
                        }
                    }
                    
                    await LoadDataAsync();
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error regenerando contrase�a: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        private async Task OnDesactivarUsuarioAsync(Usuario usuario)
        {
            if (usuario == null) return;

            var confirmar = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar",
                $"�Desea desactivar el usuario de {usuario.NombreCompleto}?",
                "S�",
                "No");

            if (!confirmar) return;

            try
            {
                usuario.Activo = false;
                await _databaseService.SaveUsuarioAsync(usuario);
                
                await Application.Current!.MainPage!.DisplayAlert("�xito", "Usuario desactivado", "OK");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error desactivando usuario: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
