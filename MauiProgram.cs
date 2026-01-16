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
                System.Diagnostics.Debug.WriteLine("*** MauiProgram - Iniciando configuración ***");
                
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
                System.Diagnostics.Debug.WriteLine("*** MauiProgram - Logging configurado ***");
#endif

                // Registrar servicios
                System.Diagnostics.Debug.WriteLine("*** MauiProgram - Registrando servicios ***");
                builder.Services.AddSingleton<AuthService>();
                builder.Services.AddSingleton<DatabaseService>();
                builder.Services.AddSingleton<WhatsAppService>();
                builder.Services.AddSingleton<ReportesService>();
                System.Diagnostics.Debug.WriteLine("*** MauiProgram - Servicios registrados ***");

                // Registrar paginas
                System.Diagnostics.Debug.WriteLine("*** MauiProgram - Registrando páginas ***");
                builder.Services.AddTransient<LoginPage>();
                builder.Services.AddTransient<LoginClientePage>();
                builder.Services.AddTransient<DashboardPage>();
                builder.Services.AddTransient<ClienteDashboardPage>();
                builder.Services.AddTransient<ClientesPage>();
                builder.Services.AddTransient<NuevoClientePage>();
                builder.Services.AddTransient<DetalleClientePage>();
                builder.Services.AddTransient<EditarClientePage>();
                builder.Services.AddTransient<NuevoPrestamoPage>();
                builder.Services.AddTransient<RegistrarPagoPage>();
                builder.Services.AddTransient<HistorialPrestamosPage>();
                builder.Services.AddTransient<EnviarMensajesPage>();
                builder.Services.AddTransient<ConfiguracionPage>();
                builder.Services.AddTransient<ReportesPage>();
                builder.Services.AddTransient<PerfilAdminPage>();
                builder.Services.AddTransient<CambiarContrasenaAdminPage>();
                System.Diagnostics.Debug.WriteLine("*** MauiProgram - Páginas registradas ***");
                
                var app = builder.Build();
                System.Diagnostics.Debug.WriteLine("*** MauiProgram - App construida ***");
                
                // Configurar manejo de excepciones no controladas
                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    var exception = args.ExceptionObject as Exception;
                    System.Diagnostics.Debug.WriteLine($"*** EXCEPCION NO CONTROLADA ***: {exception?.Message}");
                    System.Diagnostics.Debug.WriteLine($"*** StackTrace: {exception?.StackTrace}");
                    System.Diagnostics.Debug.WriteLine($"*** InnerException: {exception?.InnerException?.Message}");
                };

                System.Diagnostics.Debug.WriteLine("*** MauiProgram - Configuración completada ***");
                return app;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN CreateMauiApp ***: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"*** InnerException: {ex.InnerException?.Message}");
                throw;
            }
        }
    }
}