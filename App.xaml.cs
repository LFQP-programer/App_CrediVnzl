using Microsoft.Extensions.DependencyInjection;
using App_CrediVnzl.Services;

namespace App_CrediVnzl
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** App Constructor - Iniciando ***");
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
                var window = new Window(new AppShell());
                
                System.Diagnostics.Debug.WriteLine("*** CreateWindow - Window creado OK ***");
                System.Diagnostics.Debug.WriteLine("*** AppShell configurado con BienvenidaPage como página inicial ***");
                
                // Inicializar después de que el window esté listo
                window.Created += async (s, e) =>
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("*** Window.Created - Iniciando inicialización ***");
                        
                        // Esperar un momento para que el contexto esté disponible
                        await Task.Delay(200);
                        
                        var authService = window.Handler?.MauiContext?.Services.GetService<AuthService>();
                        var databaseService = window.Handler?.MauiContext?.Services.GetService<DatabaseService>();
                        
                        if (authService != null && databaseService != null)
                        {
                            System.Diagnostics.Debug.WriteLine("*** Servicios obtenidos correctamente ***");
                            
                            // Inicializar base de datos
                            await databaseService.InitializeAsync();
                            System.Diagnostics.Debug.WriteLine("*** Base de datos inicializada ***");
                            
                            // Verificar y crear admin por defecto si es necesario
                            await authService.VerificarPrimerUsoAsync();
                            System.Diagnostics.Debug.WriteLine("*** Primer uso verificado ***");
                            
                            System.Diagnostics.Debug.WriteLine("");
                            System.Diagnostics.Debug.WriteLine("╔════════════════════════════════════════════════╗");
                            System.Diagnostics.Debug.WriteLine("║        APLICACIÓN LISTA                        ║");
                            System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════════════════╝");
                            System.Diagnostics.Debug.WriteLine("→ Página inicial: BienvenidaPage");
                            System.Diagnostics.Debug.WriteLine("→ Usuario verá opciones: Administrador y Cliente");
                            System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════════════════╝");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("*** ERROR: Servicios no disponibles ***");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("");
                        System.Diagnostics.Debug.WriteLine("❌❌❌ ERROR CRÍTICO en Window.Created ❌❌❌");
                        System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                    }
                };
                
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