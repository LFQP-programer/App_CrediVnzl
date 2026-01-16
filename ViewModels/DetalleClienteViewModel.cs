using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    [QueryProperty(nameof(ClienteId), "clienteId")]
    public class DetalleClienteViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly WhatsAppService _whatsAppService;
        private Cliente _cliente;
        private int _clienteId;
        private int _prestamosCompletados;
        private decimal _capitalPendienteTotal;
        private decimal _totalAdeudadoHoy;

        public ObservableCollection<Prestamo> PrestamosActivos { get; set; } = new();

        public Cliente Cliente
        {
            get => _cliente;
            set
            {
                _cliente = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TieneAccesoApp));
                OnPropertyChanged(nameof(TextoEstadoAcceso));
            }
        }

        public int ClienteId
        {
            get => _clienteId;
            set
            {
                _clienteId = value;
                OnPropertyChanged();
            }
        }

        public int PrestamosCompletados
        {
            get => _prestamosCompletados;
            set
            {
                _prestamosCompletados = value;
                OnPropertyChanged();
            }
        }

        public decimal CapitalPendienteTotal
        {
            get => _capitalPendienteTotal;
            set
            {
                _capitalPendienteTotal = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalAdeudadoHoy
        {
            get => _totalAdeudadoHoy;
            set
            {
                _totalAdeudadoHoy = value;
                OnPropertyChanged();
            }
        }

        public bool TieneAccesoApp => Cliente?.TieneAccesoApp ?? false;

        public string TextoEstadoAcceso => TieneAccesoApp ? "Acceso habilitado" : "Sin acceso a la app";

        public ICommand NuevoPrestamoCommand { get; }
        public ICommand RegistrarPagoCommand { get; }
        public ICommand VerHistorialCommand { get; }
        public ICommand ToggleExpandirPrestamoCommand { get; }
        public ICommand ModificarClienteCommand { get; }
        public ICommand EliminarClienteCommand { get; }
        public ICommand GenerarPasswordCommand { get; }
        public ICommand RestablecerPasswordCommand { get; }
        public ICommand DeshabilitarAccesoCommand { get; }

        public DetalleClienteViewModel(DatabaseService databaseService, WhatsAppService whatsAppService)
        {
            _databaseService = databaseService;
            _whatsAppService = whatsAppService;
            _cliente = new Cliente();
            
            NuevoPrestamoCommand = new Command(async () => await NuevoPrestamo());
            RegistrarPagoCommand = new Command<Prestamo>(async (prestamo) => await RegistrarPago(prestamo));
            VerHistorialCommand = new Command(async () => await VerHistorial());
            ToggleExpandirPrestamoCommand = new Command<Prestamo>(ToggleExpandirPrestamo);
            ModificarClienteCommand = new Command(async () => await ModificarCliente());
            EliminarClienteCommand = new Command(async () => await EliminarCliente());
            GenerarPasswordCommand = new Command(async () => await GenerarPasswordAsync());
            RestablecerPasswordCommand = new Command(async () => await RestablecerPasswordAsync());
            DeshabilitarAccesoCommand = new Command(async () => await DeshabilitarAccesoAsync());
        }

        public async Task LoadDataAsync()
        {
            try
            {
                // Actualizar intereses de prestamos activos primero
                await _databaseService.ActualizarInteresesPrestamosActivosAsync();
                
                var cliente = await _databaseService.GetClienteAsync(ClienteId);
                if (cliente != null)
                {
                    Cliente = cliente;
                }

                var prestamos = await _databaseService.GetPrestamosByClienteAsync(ClienteId);
                
                PrestamosActivos.Clear();
                foreach (var prestamo in prestamos.Where(p => p.Estado == "Activo"))
                {
                    PrestamosActivos.Add(prestamo);
                }

                PrestamosCompletados = prestamos.Count(p => p.Estado == "Completado");
                CapitalPendienteTotal = prestamos.Where(p => p.Estado == "Activo").Sum(p => p.CapitalPendiente);
                TotalAdeudadoHoy = prestamos.Where(p => p.Estado == "Activo").Sum(p => p.TotalAdeudado);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar los datos: {ex.Message}", "OK");
            }
        }

        private async Task GenerarPasswordAsync()
        {
            try
            {
                if (Cliente.TieneAccesoApp)
                {
                    await Shell.Current.DisplayAlert(
                        "Advertencia",
                        "Este cliente ya tiene acceso a la aplicaci�n. Use 'Restablecer Contrase�a' para generar una nueva.",
                        "OK");
                    return;
                }

                var confirmar = await Shell.Current.DisplayAlert(
                    "Generar Contrase�a",
                    $"�Desea generar una contrase�a temporal para {Cliente.NombreCompleto}?\n\nSe enviar� autom�ticamente por WhatsApp.",
                    "S�",
                    "Cancelar");

                if (!confirmar)
                    return;

                // Generar contrase�a
                var password = await _databaseService.GenerarPasswordTemporalAsync(ClienteId);

                // Recargar cliente
                Cliente = await _databaseService.GetClienteAsync(ClienteId);

                // Formatear n�mero de celular para Per� (+51)
                var numeroCompleto = $"51{Cliente.NumeroCelular}";

                // Crear mensaje de WhatsApp
                var mensaje = $"Hola {Cliente.Nombres},\n\n" +
                             $"Tu contrase�a temporal para acceder a CrediVnzl es: *{password}*\n\n" +
                             $"Por seguridad, te recomendamos cambiarla al iniciar sesi�n.\n\n" +
                             $"Puedes ingresar con tu DNI: {Cliente.NumeroDocumento}\n\n" +
                             $"�Bienvenido!";

                // Enviar mensaje por WhatsApp
                var enviado = await _whatsAppService.EnviarMensajeAsync(numeroCompleto, mensaje);

                if (enviado)
                {
                    await Shell.Current.DisplayAlert(
                        "Contrase�a Generada",
                        $"Contrase�a temporal: {password}\n\nSe ha abierto WhatsApp para enviar la contrase�a al cliente.",
                        "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert(
                        "Contrase�a Generada",
                        $"Contrase�a temporal: {password}\n\nNo se pudo abrir WhatsApp. Por favor, env�a esta contrase�a manualmente al cliente.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al generar contrase�a: {ex.Message}", "OK");
            }
        }

        private async Task RestablecerPasswordAsync()
        {
            try
            {
                if (!Cliente.TieneAccesoApp)
                {
                    await Shell.Current.DisplayAlert(
                        "Advertencia",
                        "Este cliente no tiene acceso a la aplicaci�n. Use 'Generar Contrase�a' primero.",
                        "OK");
                    return;
                }

                var confirmar = await Shell.Current.DisplayAlert(
                    "Restablecer Contrase�a",
                    $"�Desea restablecer la contrase�a de {Cliente.NombreCompleto}?\n\nSe generar� una nueva contrase�a temporal y se enviar� por WhatsApp.",
                    "S�",
                    "Cancelar");

                if (!confirmar)
                    return;

                // Restablecer contrase�a
                await _databaseService.RestablecerPasswordClienteAsync(ClienteId);

                // Recargar cliente para obtener la nueva contrase�a
                Cliente = await _databaseService.GetClienteAsync(ClienteId);

                // Formatear n�mero de celular para Per� (+51)
                var numeroCompleto = $"51{Cliente.NumeroCelular}";

                // Crear mensaje de WhatsApp
                var mensaje = $"Hola {Cliente.Nombres},\n\n" +
                             $"Tu nueva contrase�a temporal para CrediVnzl es: *{Cliente.PasswordTemporal}*\n\n" +
                             $"Por seguridad, te recomendamos cambiarla al iniciar sesi�n.\n\n" +
                             $"Puedes ingresar con tu DNI: {Cliente.NumeroDocumento}";

                // Enviar mensaje por WhatsApp
                var enviado = await _whatsAppService.EnviarMensajeAsync(numeroCompleto, mensaje);

                if (enviado)
                {
                    await Shell.Current.DisplayAlert(
                        "Contrase�a Restablecida",
                        $"Nueva contrase�a temporal: {Cliente.PasswordTemporal}\n\nSe ha abierto WhatsApp para enviar la contrase�a al cliente.",
                        "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert(
                        "Contrase�a Restablecida",
                        $"Nueva contrase�a temporal: {Cliente.PasswordTemporal}\n\nNo se pudo abrir WhatsApp. Por favor, env�a esta contrase�a manualmente al cliente.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al restablecer contrase�a: {ex.Message}", "OK");
            }
        }

        private async Task DeshabilitarAccesoAsync()
        {
            try
            {
                if (!Cliente.TieneAccesoApp)
                {
                    await Shell.Current.DisplayAlert(
                        "Informaci�n",
                        "Este cliente no tiene acceso a la aplicaci�n.",
                        "OK");
                    return;
                }

                var confirmar = await Shell.Current.DisplayAlert(
                    "Deshabilitar Acceso",
                    $"�Desea deshabilitar el acceso de {Cliente.NombreCompleto} a la aplicaci�n?\n\nEl cliente no podr� iniciar sesi�n hasta que se le genere una nueva contrase�a.",
                    "S�",
                    "Cancelar");

                if (!confirmar)
                    return;

                await _databaseService.DeshabilitarAccesoClienteAsync(ClienteId);
                Cliente = await _databaseService.GetClienteAsync(ClienteId);

                await Shell.Current.DisplayAlert(
                    "�xito",
                    "Acceso deshabilitado correctamente.",
                    "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al deshabilitar acceso: {ex.Message}", "OK");
            }
        }

        private async Task NuevoPrestamo()
        {
            await Shell.Current.GoToAsync($"nuevoprestamo?clienteId={ClienteId}");
        }

        private async Task RegistrarPago(Prestamo prestamo)
        {
            if (prestamo != null)
            {
                await Shell.Current.GoToAsync($"registrarpago?prestamoId={prestamo.Id}");
            }
        }

        private async Task VerHistorial()
        {
            await Shell.Current.GoToAsync($"historialprestamos?clienteId={ClienteId}");
        }

        private async Task ModificarCliente()
        {
            await Shell.Current.GoToAsync($"editarcliente?clienteId={ClienteId}");
        }

        private async Task EliminarCliente()
        {
            try
            {
                // Verificar si tiene pr�stamos activos
                var prestamos = await _databaseService.GetPrestamosByClienteAsync(ClienteId);
                var prestamosActivos = prestamos.Where(p => p.Estado == "Activo").ToList();

                if (prestamosActivos.Any())
                {
                    var confirmar = await Shell.Current.DisplayAlert(
                        "Advertencia", 
                        $"Este cliente tiene {prestamosActivos.Count} pr�stamo(s) activo(s) con una deuda total de S/{prestamosActivos.Sum(p => p.TotalAdeudado):N2}.\n\n�Est� seguro de eliminar al cliente y todos sus pr�stamos, pagos e historial?", 
                        "S�, eliminar", 
                        "Cancelar");

                    if (!confirmar)
                        return;
                }
                else
                {
                    var confirmar = await Shell.Current.DisplayAlert(
                        "Confirmar eliminaci�n", 
                        "�Est� seguro de eliminar este cliente? Esta acci�n no se puede deshacer.", 
                        "S�, eliminar", 
                        "Cancelar");

                    if (!confirmar)
                        return;
                }

                // Eliminar todos los datos relacionados en cascada
                await _databaseService.EliminarClienteConDatosRelacionadosAsync(ClienteId);

                await Shell.Current.DisplayAlert("�xito", "Cliente eliminado correctamente", "OK");
                
                // Regresar a la p�gina de clientes
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo eliminar el cliente: {ex.Message}", "OK");
            }
        }

        private void ToggleExpandirPrestamo(Prestamo prestamo)
        {
            if (prestamo != null)
            {
                prestamo.Expandido = !prestamo.Expandido;
                
                // Refrescar la colecci�n para actualizar la UI
                var index = PrestamosActivos.IndexOf(prestamo);
                if (index >= 0)
                {
                    PrestamosActivos.RemoveAt(index);
                    PrestamosActivos.Insert(index, prestamo);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
