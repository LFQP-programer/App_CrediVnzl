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
            Routing.RegisterRoute("calendario", typeof(CalendarioPagosPage));
        }
    }
}
