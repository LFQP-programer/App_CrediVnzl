using App_CrediVnzl.Pages;

namespace App_CrediVnzl
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** AppShell Constructor - Iniciando ***");
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("*** AppShell Constructor - InitializeComponent OK ***");

                // Registrar rutas de navegación
                System.Diagnostics.Debug.WriteLine("*** AppShell - Registrando rutas ***");
                
                Routing.RegisterRoute("clientes", typeof(ClientesPage));
                Routing.RegisterRoute("nuevocliente", typeof(NuevoClientePage));
                Routing.RegisterRoute("detallecliente", typeof(DetalleClientePage));
                Routing.RegisterRoute("editarcliente", typeof(EditarClientePage));
                Routing.RegisterRoute("nuevoprestamo", typeof(NuevoPrestamoPage));
                Routing.RegisterRoute("registrarpago", typeof(RegistrarPagoPage));
                Routing.RegisterRoute("historialprestamos", typeof(HistorialPrestamosPage));
                Routing.RegisterRoute("mensajes", typeof(EnviarMensajesPage));
                Routing.RegisterRoute("reportes", typeof(ReportesPage));
                Routing.RegisterRoute("configuracion", typeof(ConfiguracionPage));
                Routing.RegisterRoute("perfiladmin", typeof(PerfilAdminPage));
                Routing.RegisterRoute("cambiarcontrasenaadmin", typeof(CambiarContrasenaAdminPage));
                
                // Registrar ruta de calendario apuntando a reportes temporalmente
                // TODO: Crear CalendarioPagosPage cuando sea necesario
                Routing.RegisterRoute("calendario", typeof(ReportesPage));
                
                // Ruta de ayuda apuntando a configuración temporalmente
                // TODO: Crear AyudaPage cuando sea necesario
                Routing.RegisterRoute("ayuda", typeof(ConfiguracionPage));
                
                System.Diagnostics.Debug.WriteLine("*** AppShell Constructor - Rutas registradas OK ***");
                
                // Suscribirse al evento de navegación para debugging
                this.Navigating += OnNavigating;
                this.Navigated += OnNavigated;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN AppShell Constructor ***: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException?.Message}");
                throw;
            }
        }

        private void OnNavigating(object? sender, ShellNavigatingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"*** Navegando a: {e.Target.Location} ***");
        }

        private void OnNavigated(object? sender, ShellNavigatedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"*** Navegación completada a: {e.Current.Location} ***");
        }
    }
}
