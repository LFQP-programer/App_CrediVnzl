using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class ConfiguracionViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private DatabaseInfo _databaseInfo;

        public DatabaseInfo DatabaseInfo
        {
            get => _databaseInfo;
            set
            {
                _databaseInfo = value;
                OnPropertyChanged();
            }
        }

        public ICommand ActualizarInformacionCommand { get; }
        public ICommand LimpiarDatosCommand { get; }
        public ICommand ReiniciarBaseDeDatosCommand { get; }
        public ICommand GestionarUsuariosCommand { get; }
        public ICommand ImportarDatosCSVCommand { get; }

        public ConfiguracionViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _databaseInfo = new DatabaseInfo();

            ActualizarInformacionCommand = new Command(async () => await CargarInformacionAsync());
            LimpiarDatosCommand = new Command(async () => await LimpiarDatosAsync());
            ReiniciarBaseDeDatosCommand = new Command(async () => await ReiniciarBaseDeDatosAsync());
            GestionarUsuariosCommand = new Command(async () => await OnGestionarUsuariosAsync());
            ImportarDatosCSVCommand = new Command(async () => await ImportarDatosCSVAsync());
        }

        private async Task OnGestionarUsuariosAsync()
        {
            await Shell.Current.GoToAsync("gestionarusuarios");
        }

        public async Task CargarInformacionAsync()
        {
            try
            {
                DatabaseInfo = await _databaseService.ObtenerInformacionBaseDeDatosAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo cargar la informacion: {ex.Message}", "OK");
            }
        }

        private async Task ImportarDatosCSVAsync()
        {
            try
            {
                var options = new PickOptions
                {
                    PickerTitle = "Seleccione el archivo CSV",
                };

                var result = await FilePicker.Default.PickAsync(options);

                if (result != null)
                {
                    await Shell.Current.DisplayAlert("Procesando", "Leyendo archivo, por favor espere no salga de esta pantalla...", "OK");

                    int clientesImportados = 0;
                    int prestamosImportados = 0;
                    int lineasOmitidas = 0;

                    using var stream = await result.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    var content = await reader.ReadToEndAsync();

                    // Separar por salto de linea
                    var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    if (lines.Length <= 1)
                    {
                        await Shell.Current.DisplayAlert("Aviso", "El archivo parece estar vacío o solo contiene la cabecera.", "OK");
                        return;
                    }

                    // Ignorar la cabecera e iterar desde la fila 1
                    var clientesActuales = await _databaseService.GetClientesAsync();

                    foreach (var line in lines.Skip(1))
                    {
                        try
                        {
                            var separator = line.Contains(";") ? ';' : ',';
                            var cols = line.Split(separator);

                            // Se espera MÍNIMO las columnas de cliente (5)
                            if (cols.Length < 5)
                            {
                                lineasOmitidas++;
                                continue;
                            }

                            string cedula = cols[0].Trim();
                            string nombres = cols[1].Trim();
                            string apellidos = cols[2].Trim();
                            string celular = cols[3].Trim();
                            string direccion = cols[4].Trim();

                            if (string.IsNullOrEmpty(cedula) || string.IsNullOrEmpty(nombres))
                            {
                                lineasOmitidas++;
                                continue;
                            }

                            // Verificar o crear cliente
                            var cliente = clientesActuales.FirstOrDefault(c => c.NumeroDocumento == cedula);
                            if (cliente == null)
                            {
                                cliente = new Models.Cliente
                                {
                                    TipoDocumento = "DNI",
                                    NumeroDocumento = cedula,
                                    Nombres = nombres,
                                    Apellidos = apellidos,
                                    NumeroCelular = celular,
                                    AvalDireccion = direccion,
                                    EstadoCliente = "Activo",
                                    FechaRegistro = DateTime.Now
                                };
                                await _databaseService.SaveClienteAsync(cliente);
                                clientesActuales.Add(cliente); // Añadir a cache para que los siguientes préstamos lo encuentren
                                clientesImportados++;
                            }

                            // Si tiene los datos de préstamo (11 o más columnas en total)
                            if (cols.Length >= 11)
                            {
                                if (decimal.TryParse(cols[5].Trim(), out decimal montoInicial) &&
                                    decimal.TryParse(cols[6].Trim(), out decimal tasaInteres) &&
                                    int.TryParse(cols[7].Trim(), out int duracion) &&
                                    DateTime.TryParse(cols[8].Trim(), out DateTime fechaInicio) &&
                                    decimal.TryParse(cols[9].Trim(), out decimal capitalPendiente) &&
                                    decimal.TryParse(cols[10].Trim(), out decimal interesAcumulado))
                                {
                                    var prestamo = new Models.Prestamo
                                    {
                                        ClienteId = cliente.Id,
                                        MontoInicial = montoInicial,
                                        TasaInteresSemanal = tasaInteres,
                                        DuracionSemanas = duracion,
                                        FechaInicio = fechaInicio,
                                        FrecuenciaPago = "Semanal",
                                        Estado = "Activo",
                                        CapitalPendiente = capitalPendiente,
                                        InteresAcumulado = interesAcumulado,
                                        TotalAdeudado = capitalPendiente + interesAcumulado,
                                        MontoPagado = montoInicial - capitalPendiente
                                    };

                                    await _databaseService.SavePrestamoAsync(prestamo);

                                    // Actualizar deuda del cliente
                                    cliente.PrestamosActivos++;
                                    cliente.DeudaPendiente += prestamo.TotalAdeudado;
                                    await _databaseService.SaveClienteAsync(cliente);

                                    prestamosImportados++;
                                }
                                else
                                {
                                    // Fallo el parseo de números o fechas en esta fila
                                    lineasOmitidas++;
                                }
                            }
                        }
                        catch
                        {
                            lineasOmitidas++;
                        }
                    }

                    await CargarInformacionAsync();

                    string resMsg = $"Nuevos Clientes: {clientesImportados}\nNuevos Préstamos: {prestamosImportados}";
                    if (lineasOmitidas > 0)
                        resMsg += $"\nLíneas omitidas por error/formato: {lineasOmitidas}";

                    await Shell.Current.DisplayAlert("Importación Finalizada", resMsg, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error de Lectura", $"Ocurrió un error al procesar el archivo CSV: {ex.Message}", "OK");
            }
        }

        private async Task LimpiarDatosAsync()
        {
            try
            {
                var confirmar = await Shell.Current.DisplayAlert(
                    "Confirmar Limpieza de Datos",
                    $"Esta accion eliminara:\n\n" +
                    $"- {DatabaseInfo.TotalClientes} Cliente(s)\n" +
                    $"- {DatabaseInfo.TotalPrestamos} Prestamo(s)\n" +
                    $"- {DatabaseInfo.TotalPagos} Pago(s) Programado(s)\n" +
                    $"- {DatabaseInfo.TotalHistorialPagos} Registro(s) de Historial\n\n" +
                    $"Esta accion NO se puede deshacer.\n\n" +
                    $"Desea continuar?",
                    "Si, Limpiar Todo",
                    "Cancelar");

                if (!confirmar)
                    return;

                // Segunda confirmacion
                var confirmarFinal = await Shell.Current.DisplayAlert(
                    "Ultima Confirmacion",
                    "Esta COMPLETAMENTE SEGURO de eliminar todos los datos?\n\nEsta es su ultima oportunidad para cancelar.",
                    "Si, ELIMINAR TODO",
                    "Cancelar");

                if (!confirmarFinal)
                    return;

                // Mostrar indicador de actividad
                await Shell.Current.DisplayAlert("Procesando", "Limpiando base de datos...", "OK");

                // Limpiar datos
                await _databaseService.ReiniciarBaseDeDatosAsync();

                // Actualizar informacion
                await CargarInformacionAsync();

                await Shell.Current.DisplayAlert(
                    "Exito",
                    "Todos los datos han sido eliminados correctamente.\n\nLa base de datos esta ahora vacia y lista para usar.",
                    "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo limpiar la base de datos: {ex.Message}", "OK");
            }
        }

        private async Task ReiniciarBaseDeDatosAsync()
        {
            try
            {
                var confirmar = await Shell.Current.DisplayAlert(
                    "Confirmar Reinicio de Base de Datos",
                    $"Esta accion:\n\n" +
                    $"- Eliminara el archivo de base de datos completo\n" +
                    $"- Borrara todos los datos:\n" +
                    $"  - {DatabaseInfo.TotalClientes} Cliente(s)\n" +
                    $"  - {DatabaseInfo.TotalPrestamos} Prestamo(s)\n" +
                    $"  - {DatabaseInfo.TotalPagos} Pago(s)\n" +
                    $"  - {DatabaseInfo.TotalHistorialPagos} Historial(es)\n" +
                    $"- Creara una base de datos nueva y vacia\n\n" +
                    $"Esta accion NO se puede deshacer.\n\n" +
                    $"Desea continuar?",
                    "Si, Reiniciar",
                    "Cancelar");

                if (!confirmar)
                    return;

                // Segunda confirmacion
                var confirmarFinal = await Shell.Current.DisplayAlert(
                    "ADVERTENCIA FINAL",
                    "Esta a punto de ELIMINAR PERMANENTEMENTE el archivo de base de datos.\n\n" +
                    $"Se perdera toda la informacion ({DatabaseInfo.TamañoArchivoFormateado}).\n\n" +
                    "Esta es su ULTIMA oportunidad para cancelar.\n\n" +
                    "Esta ABSOLUTAMENTE SEGURO?",
                    "Si, ELIMINAR TODO",
                    "NO, Cancelar");

                if (!confirmarFinal)
                    return;

                // Mostrar indicador de actividad
                await Shell.Current.DisplayAlert("Procesando", "Eliminando y recreando base de datos...", "OK");

                // Reiniciar base de datos
                await _databaseService.EliminarBaseDeDatosCompletaAsync();

                // Actualizar informacion
                await CargarInformacionAsync();

                await Shell.Current.DisplayAlert(
                    "Base de Datos Reiniciada",
                    "La base de datos ha sido eliminada y recreada exitosamente.\n\n" +
                    "La aplicacion esta ahora completamente limpia y lista para usar con datos nuevos.",
                    "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo reiniciar la base de datos: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
