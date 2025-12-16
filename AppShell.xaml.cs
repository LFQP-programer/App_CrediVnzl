using App_CrediVnzl.Pages;

namespace App_CrediVnzl
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rutas de navegacion
            Routing.RegisterRoute("clientes", typeof(ClientesPage));
            Routing.RegisterRoute("nuevocliente", typeof(NuevoClientePage));
            Routing.RegisterRoute("detallecliente", typeof(DetalleClientePage));
            Routing.RegisterRoute("nuevoprestamo", typeof(NuevoPrestamoPage));
            Routing.RegisterRoute("registrarpago", typeof(RegistrarPagoPage));
            Routing.RegisterRoute("calendario", typeof(CalendarioPagosPage));
            Routing.RegisterRoute("mensajes", typeof(EnviarMensajesPage));
        }
    }
}
