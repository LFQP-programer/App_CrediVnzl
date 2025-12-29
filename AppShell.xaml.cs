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

                // Registrar rutas de navegacion
                Routing.RegisterRoute("clientes", typeof(ClientesPage));
                Routing.RegisterRoute("nuevocliente", typeof(NuevoClientePage));
                Routing.RegisterRoute("detallecliente", typeof(DetalleClientePage));
                Routing.RegisterRoute("editarcliente", typeof(EditarClientePage));
                Routing.RegisterRoute("nuevoprestamo", typeof(NuevoPrestamoPage));
                Routing.RegisterRoute("registrarpago", typeof(RegistrarPagoPage));
                Routing.RegisterRoute("historialprestamos", typeof(HistorialPrestamosPage));
                Routing.RegisterRoute("calendario", typeof(CalendarioPagosPage));
                Routing.RegisterRoute("mensajes", typeof(EnviarMensajesPage));
                Routing.RegisterRoute("configuracion", typeof(ConfiguracionPage));
                Routing.RegisterRoute("reportes", typeof(ReportesPage));
                
                System.Diagnostics.Debug.WriteLine("*** AppShell Constructor - Rutas registradas OK ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN AppShell Constructor ***: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException?.Message}");
                throw;
            }
        }
    }
}
