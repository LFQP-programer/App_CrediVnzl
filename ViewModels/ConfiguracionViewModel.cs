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

        public ConfiguracionViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _databaseInfo = new DatabaseInfo();

            ActualizarInformacionCommand = new Command(async () => await CargarInformacionAsync());
            LimpiarDatosCommand = new Command(async () => await LimpiarDatosAsync());
            ReiniciarBaseDeDatosCommand = new Command(async () => await ReiniciarBaseDeDatosAsync());
            GestionarUsuariosCommand = new Command(async () => await OnGestionarUsuariosAsync());
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
                await Shell.Current.DisplayAlert("Error", $"No se pudo cargar la información: {ex.Message}", "OK");
            }
        }

        private async Task LimpiarDatosAsync()
        {
            try
            {
                var confirmar = await Shell.Current.DisplayAlert(
                    "?? Confirmar Limpieza de Datos",
                    $"Esta acción eliminará:\n\n" +
                    $"• {DatabaseInfo.TotalClientes} Cliente(s)\n" +
                    $"• {DatabaseInfo.TotalPrestamos} Préstamo(s)\n" +
                    $"• {DatabaseInfo.TotalPagos} Pago(s) Programado(s)\n" +
                    $"• {DatabaseInfo.TotalHistorialPagos} Registro(s) de Historial\n\n" +
                    $"Esta acción NO se puede deshacer.\n\n" +
                    $"¿Desea continuar?",
                    "Sí, Limpiar Todo",
                    "Cancelar");

                if (!confirmar)
                    return;

                // Segunda confirmación
                var confirmarFinal = await Shell.Current.DisplayAlert(
                    "?? Última Confirmación",
                    "¿Está COMPLETAMENTE SEGURO de eliminar todos los datos?\n\nEsta es su última oportunidad para cancelar.",
                    "SÍ, ELIMINAR TODO",
                    "Cancelar");

                if (!confirmarFinal)
                    return;

                // Mostrar indicador de actividad
                await Shell.Current.DisplayAlert("Procesando", "Limpiando base de datos...", "OK");

                // Limpiar datos
                await _databaseService.ReiniciarBaseDeDatosAsync();

                // Actualizar información
                await CargarInformacionAsync();

                await Shell.Current.DisplayAlert(
                    "? Éxito",
                    "Todos los datos han sido eliminados correctamente.\n\nLa base de datos está ahora vacía y lista para usar.",
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
                    "?? Confirmar Reinicio de Base de Datos",
                    $"Esta acción:\n\n" +
                    $"• Eliminará el archivo de base de datos completo\n" +
                    $"• Borrará todos los datos:\n" +
                    $"  - {DatabaseInfo.TotalClientes} Cliente(s)\n" +
                    $"  - {DatabaseInfo.TotalPrestamos} Préstamo(s)\n" +
                    $"  - {DatabaseInfo.TotalPagos} Pago(s)\n" +
                    $"  - {DatabaseInfo.TotalHistorialPagos} Historial(es)\n" +
                    $"• Creará una base de datos nueva y vacía\n\n" +
                    $"Esta acción NO se puede deshacer.\n\n" +
                    $"¿Desea continuar?",
                    "Sí, Reiniciar",
                    "Cancelar");

                if (!confirmar)
                    return;

                // Segunda confirmación
                var confirmarFinal = await Shell.Current.DisplayAlert(
                    "?? ADVERTENCIA FINAL",
                    "Está a punto de ELIMINAR PERMANENTEMENTE el archivo de base de datos.\n\n" +
                    $"Se perderá toda la información ({DatabaseInfo.TamañoArchivoFormateado}).\n\n" +
                    "Esta es su ÚLTIMA oportunidad para cancelar.\n\n" +
                    "¿Está ABSOLUTAMENTE SEGURO?",
                    "SÍ, ELIMINAR TODO",
                    "NO, Cancelar");

                if (!confirmarFinal)
                    return;

                // Mostrar indicador de actividad
                await Shell.Current.DisplayAlert("Procesando", "Eliminando y recreando base de datos...", "OK");

                // Reiniciar base de datos
                await _databaseService.EliminarBaseDeDatosCompletaAsync();

                // Actualizar información
                await CargarInformacionAsync();

                await Shell.Current.DisplayAlert(
                    "? Base de Datos Reiniciada",
                    "La base de datos ha sido eliminada y recreada exitosamente.\n\n" +
                    "La aplicación está ahora completamente limpia y lista para usar con datos nuevos.",
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
