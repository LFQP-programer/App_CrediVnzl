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
                System.Diagnostics.Debug.WriteLine("*** App Constructor - INICIANDO APLICACIÓN ***");
                
                // Configurar manejo de excepciones no controladas ANTES de InitializeComponent
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
                
                System.Diagnostics.Debug.WriteLine("*** App Constructor - Manejadores de excepciones configurados ***");
                
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("*** App Constructor - InitializeComponent OK ***");
                
                System.Diagnostics.Debug.WriteLine("*** App Constructor - COMPLETADO EXITOSAMENTE ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR CRÍTICO EN App Constructor ***: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException?.Message}");
                
                // Intentar mostrar un mensaje de error al usuario
                try
                {
                    var firstWindow = Windows?.FirstOrDefault();
                    if (firstWindow?.Page != null)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await firstWindow.Page.DisplayAlert("Error Crítico", 
                                $"Error al iniciar la aplicación: {ex.Message}", "OK");
                        });
                    }
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"*** ERROR mostrando alerta: {ex2.Message} ***");
                }
                
                throw;
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("╔═══════════════════════════════════════════════════════════╗");
                System.Diagnostics.Debug.WriteLine("║                    CREANDO VENTANA                       ║");
                System.Diagnostics.Debug.WriteLine("╚═══════════════════════════════════════════════════════════╝");
                System.Diagnostics.Debug.WriteLine("");
                
                System.Diagnostics.Debug.WriteLine("*** CreateWindow - Creando AppShell ***");
                var appShell = new AppShell();
                System.Diagnostics.Debug.WriteLine("*** CreateWindow - AppShell creado OK ***");
                
                var window = new Window(appShell);
                System.Diagnostics.Debug.WriteLine("*** CreateWindow - Window creado OK ***");
                
                // Configurar título de ventana
                window.Title = "CrediVzla - Sistema de Préstamos";
                
                // Configurar eventos
                window.Created += OnWindowCreated;
                window.Activated += OnWindowActivated;
                window.Destroying += OnWindowDestroying;
                
                System.Diagnostics.Debug.WriteLine("*** CreateWindow - Eventos configurados ***");
                
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("╔═══════════════════════════════════════════════════════════╗");
                System.Diagnostics.Debug.WriteLine("║                 VENTANA LISTA ✓                          ║");
                System.Diagnostics.Debug.WriteLine("╚═══════════════════════════════════════════════════════════╝");
                
                return window;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("❌❌❌ ERROR CRÍTICO EN CreateWindow ❌❌❌");
                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine("");
                throw;
            }
        }

        private async void OnWindowCreated(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("╔═══════════════════════════════════════════════════════════╗");
                System.Diagnostics.Debug.WriteLine("║              INICIALIZANDO SERVICIOS                     ║");
                System.Diagnostics.Debug.WriteLine("╚═══════════════════════════════════════════════════════════╝");
                
                if (sender is Window window)
                {
                    System.Diagnostics.Debug.WriteLine("*** Window.Created - Iniciando inicialización ***");
                    
                    // Esperar un momento para que el contexto esté disponible
                    await Task.Delay(500);
                    
                    var authService = window.Handler?.MauiContext?.Services.GetService<AuthService>();
                    var databaseService = window.Handler?.MauiContext?.Services.GetService<DatabaseService>();
                    
                    if (authService != null && databaseService != null)
                    {
                        System.Diagnostics.Debug.WriteLine("*** Servicios obtenidos correctamente ✓ ***");
                        
                        // Inicializar base de datos
                        System.Diagnostics.Debug.WriteLine("*** Inicializando base de datos... ***");
                        await databaseService.InitializeAsync();
                        System.Diagnostics.Debug.WriteLine("*** Base de datos inicializada ✓ ***");
                        
                        // Verificar y crear admin por defecto si es necesario
                        System.Diagnostics.Debug.WriteLine("*** Verificando primer uso... ***");
                        await authService.VerificarPrimerUsoAsync();
                        System.Diagnostics.Debug.WriteLine("*** Primer uso verificado ✓ ***");
                        
                        System.Diagnostics.Debug.WriteLine("");
                        System.Diagnostics.Debug.WriteLine("╔═══════════════════════════════════════════════════════════╗");
                        System.Diagnostics.Debug.WriteLine("║                ✅ APLICACIÓN LISTA ✅                     ║");
                        System.Diagnostics.Debug.WriteLine("║                                                           ║");
                        System.Diagnostics.Debug.WriteLine("║  → Página inicial: BienvenidaPage                        ║");
                        System.Diagnostics.Debug.WriteLine("║  → Usuario verá: Administrador y Cliente                 ║");
                        System.Diagnostics.Debug.WriteLine("║  → Base de datos inicializada                            ║");
                        System.Diagnostics.Debug.WriteLine("║  → Servicios funcionando                                 ║");
                        System.Diagnostics.Debug.WriteLine("╚═══════════════════════════════════════════════════════════╝");
                        System.Diagnostics.Debug.WriteLine("");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("❌ ERROR: Servicios no disponibles");
                        System.Diagnostics.Debug.WriteLine($"   - AuthService: {(authService != null ? "✓" : "❌")}");
                        System.Diagnostics.Debug.WriteLine($"   - DatabaseService: {(databaseService != null ? "✓" : "❌")}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("❌❌❌ ERROR CRÍTICO en Window.Created ❌❌❌");
                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException?.Message}");
            }
        }

        private void OnWindowActivated(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("*** Window Activada - Usuario puede interactuar ***");
        }

        private void OnWindowDestroying(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("*** Window destruyéndose - Cerrando aplicación ***");
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("💥💥💥 EXCEPCIÓN NO CONTROLADA 💥💥💥");
            System.Diagnostics.Debug.WriteLine($"Mensaje: {exception?.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {exception?.StackTrace}");
            System.Diagnostics.Debug.WriteLine($"Is Terminating: {e.IsTerminating}");
            System.Diagnostics.Debug.WriteLine("");
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("⚡⚡⚡ EXCEPCIÓN TASK NO OBSERVADA ⚡⚡⚡");
            System.Diagnostics.Debug.WriteLine($"Mensaje: {e.Exception.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {e.Exception.StackTrace}");
            System.Diagnostics.Debug.WriteLine("");
            e.SetObserved(); // Previene que la aplicación se cierre
        }
    }
}