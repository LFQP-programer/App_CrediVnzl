using Microsoft.Extensions.DependencyInjection;
using App_CrediVnzl.Services;

namespace App_CrediVnzl
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }

        public App(IServiceProvider services)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** App Constructor - Iniciando ***");
                
                Services = services;
                
                InitializeComponent();
                
                // Forzar tema claro en toda la aplicación
                // Esto mantiene las letras negras incluso cuando el sistema está en modo oscuro
                Application.Current.UserAppTheme = AppTheme.Light;
                
                System.Diagnostics.Debug.WriteLine("*** App Constructor - InitializeComponent OK ***");
                System.Diagnostics.Debug.WriteLine("*** Tema forzado a Light (Claro) ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN App Constructor ***: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException?.Message}");
                throw;
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** CreateWindow - Iniciando ***");
                
                // Asegurar que DatabaseService esté disponible e inicializado
                if (Services != null)
                {
                    System.Diagnostics.Debug.WriteLine("*** Obteniendo DatabaseService ***");
                    var databaseService = Services.GetService<DatabaseService>();
                    
                    if (databaseService != null)
                    {
                        System.Diagnostics.Debug.WriteLine("*** DatabaseService obtenido, iniciando inicialización ***");
                        // Inicializar en segundo plano sin bloquear el hilo de UI
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await databaseService.InitializeAsync();
                                System.Diagnostics.Debug.WriteLine("*** Base de datos inicializada correctamente ***");
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"*** ERROR al inicializar base de datos: {ex.Message} ***");
                                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                            }
                        });
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("*** ADVERTENCIA: DatabaseService es null ***");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("*** ADVERTENCIA: Services es null ***");
                }
                
                System.Diagnostics.Debug.WriteLine("*** Creando AppShell ***");
                var window = new Window(new AppShell())
                {
                    Title = "CrediVnzl"
                };
                
                // Inicializar notificaciones al arrancar la app
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // Esperar un poco para que la base de datos se inicialice
                        await Task.Delay(1000);
                        
                        var notificationService = Services?.GetService<NotificationService>();
                        if (notificationService != null)
                        {
                            await notificationService.InicializarNotificacionesAsync();
                            System.Diagnostics.Debug.WriteLine("*** Notificaciones inicializadas correctamente ***");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"*** Error al inicializar notificaciones: {ex.Message} ***");
                    }
                });
                
                System.Diagnostics.Debug.WriteLine("*** CreateWindow - Window creado OK ***");
                return window;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN CreateWindow ***: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException?.Message}");
                throw;
            }
        }
    }
}