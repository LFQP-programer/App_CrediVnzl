using Microsoft.Extensions.Logging;
using App_CrediVnzl.Services;
using App_CrediVnzl.Pages;

namespace App_CrediVnzl
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            try
            {
                var builder = MauiApp.CreateBuilder();
                builder
                    .UseMauiApp<App>()
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    });

#if DEBUG
                builder.Logging.AddDebug();
#endif

                // Registrar servicios
                builder.Services.AddSingleton<DatabaseService>();
                builder.Services.AddSingleton<WhatsAppService>();

                // Registrar paginas
                builder.Services.AddTransient<DashboardPage>();
                builder.Services.AddTransient<ClientesPage>();
                builder.Services.AddTransient<NuevoClientePage>();
                builder.Services.AddTransient<DetalleClientePage>();
                builder.Services.AddTransient<NuevoPrestamoPage>();
                builder.Services.AddTransient<RegistrarPagoPage>();
                builder.Services.AddTransient<HistorialPrestamosPage>();
                builder.Services.AddTransient<CalendarioPagosPage>();
                builder.Services.AddTransient<EnviarMensajesPage>();

                var app = builder.Build();
                
                // Configurar manejo de excepciones no controladas
                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    var exception = args.ExceptionObject as Exception;
                    System.Diagnostics.Debug.WriteLine($"*** EXCEPCION NO CONTROLADA ***: {exception?.Message}");
                    System.Diagnostics.Debug.WriteLine($"StackTrace: {exception?.StackTrace}");
                };

                return app;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN CreateMauiApp ***: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}