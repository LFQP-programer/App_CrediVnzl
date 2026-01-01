using App_CrediVnzl.Pages;
using App_CrediVnzl.Services;

namespace App_CrediVnzl
{
    public partial class AppShell : Shell
    {
        private AuthService? _authService;
        private MenuItem? _cerrarSesionMenuItem;

        public AppShell()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** AppShell Constructor - Iniciando ***");
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("*** AppShell Constructor - InitializeComponent OK ***");

                // Registrar rutas de login específicas
                Routing.RegisterRoute("loginadmin", typeof(LoginAdminPage));
                Routing.RegisterRoute("logincliente", typeof(LoginClientePage));

                // Registrar ruta de configuración de cuenta
                Routing.RegisterRoute("configuracioncuenta", typeof(ConfiguracionCuentaPage));

                // Registrar rutas de navegación existentes
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
                Routing.RegisterRoute("gestionarusuarios", typeof(GestionarUsuariosPage));
                
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

        /// <summary>
        /// Muestra el menú del Flyout después de un login exitoso
        /// </summary>
        public void MostrarMenuDespuesDelLogin()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** Mostrando menú del Flyout ***");
                
                // Hacer visibles los FlyoutItems
                foreach (var item in Items)
                {
                    if (item is FlyoutItem flyoutItem)
                    {
                        flyoutItem.IsVisible = true;
                        System.Diagnostics.Debug.WriteLine($"   - {flyoutItem.Title} ahora visible");
                    }
                }
                
                // Agregar MenuItem de cerrar sesión si no existe
                if (_cerrarSesionMenuItem == null)
                {
                    _cerrarSesionMenuItem = new MenuItem
                    {
                        Text = "Cerrar Sesión"
                    };
                    _cerrarSesionMenuItem.Clicked += OnCerrarSesionClicked;
                    Items.Add(_cerrarSesionMenuItem);
                    System.Diagnostics.Debug.WriteLine("   - MenuItem 'Cerrar Sesión' agregado");
                }
                
                System.Diagnostics.Debug.WriteLine("*** Menú del Flyout visible ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR mostrando menú: {ex.Message}");
            }
        }

        /// <summary>
        /// Oculta el menú del Flyout (usado al cerrar sesión)
        /// </summary>
        public void OcultarMenu()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** Ocultando menú del Flyout ***");
                
                // Ocultar los FlyoutItems
                foreach (var item in Items)
                {
                    if (item is FlyoutItem flyoutItem)
                    {
                        flyoutItem.IsVisible = false;
                    }
                }
                
                // Quitar MenuItem de cerrar sesión
                if (_cerrarSesionMenuItem != null)
                {
                    Items.Remove(_cerrarSesionMenuItem);
                    _cerrarSesionMenuItem.Clicked -= OnCerrarSesionClicked;
                    _cerrarSesionMenuItem = null;
                    System.Diagnostics.Debug.WriteLine("   - MenuItem 'Cerrar Sesión' removido");
                }
                
                System.Diagnostics.Debug.WriteLine("*** Menú del Flyout oculto ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR ocultando menú: {ex.Message}");
            }
        }

        private AuthService GetAuthService()
        {
            if (_authService == null)
            {
                _authService = Handler?.MauiContext?.Services.GetService<AuthService>();
            }
            return _authService ?? throw new InvalidOperationException("AuthService no disponible");
        }

        private async void OnCerrarSesionClicked(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** OnCerrarSesionClicked - Iniciando ***");

                bool respuesta = await DisplayAlert(
                    "Cerrar Sesión",
                    "¿Estás seguro que deseas cerrar sesión?",
                    "Sí, cerrar sesión",
                    "Cancelar");

                System.Diagnostics.Debug.WriteLine($"*** Usuario respondió: {respuesta} ***");

                if (respuesta)
                {
                    System.Diagnostics.Debug.WriteLine("*** Usuario confirmó cerrar sesión ***");
                    
                    try
                    {
                        // Obtener el servicio y cerrar sesión
                        var authService = GetAuthService();
                        authService.Logout();
                        System.Diagnostics.Debug.WriteLine("*** Sesión cerrada en AuthService ***");

                        // IMPORTANTE: Limpiar preferences de login automático
                        System.Diagnostics.Debug.WriteLine("*** Limpiando preferences de login automático ***");
                        Preferences.Remove("recordar_usuario");
                        Preferences.Remove("ultimo_usuario");
                        System.Diagnostics.Debug.WriteLine("*** Preferences limpiadas correctamente ***");

                        // Ocultar el menú
                        OcultarMenu();

                        // Ocultar el Flyout
                        FlyoutIsPresented = false;
                        System.Diagnostics.Debug.WriteLine("*** Flyout cerrado ***");

                        // Esperar un momento para que se cierre el flyout
                        await Task.Delay(100);

                        // Navegar a la página de bienvenida
                        System.Diagnostics.Debug.WriteLine("*** Navegando a BienvenidaPage ***");
                        await GoToAsync("//bienvenida");
                        
                        System.Diagnostics.Debug.WriteLine("*** Navegación completada ***");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"*** ERROR durante el cierre de sesión: {ex.Message} ***");
                        System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                        throw;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("*** Usuario canceló cerrar sesión ***");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR FATAL en OnCerrarSesionClicked: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException?.Message}");
                
                try
                {
                    await DisplayAlert("Error", $"No se pudo cerrar sesión. Por favor, reinicia la aplicación.\n\nError: {ex.Message}", "OK");
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"*** ERROR mostrando alerta de error: {ex2.Message}");
                }
            }
        }
    }
}
