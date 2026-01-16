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
                System.Diagnostics.Debug.WriteLine("*** App Constructor - InitializeComponent OK ***");
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
                
                // Inicializar la base de datos de manera asíncrona pero sin bloquear
                if (Services != null)
                {
                    var databaseService = Services.GetRequiredService<DatabaseService>();
                    Task.Run(async () =>
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine("*** Inicializando base de datos ***");
                            await databaseService.InitializeAsync();
                            System.Diagnostics.Debug.WriteLine("*** Base de datos inicializada OK ***");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"*** ERROR al inicializar base de datos: {ex.Message} ***");
                            System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                        }
                    });
                }
                
                var window = new Window(new AppShell())
                {
                    Title = "CrediVnzl"
                };
                
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