using Microsoft.Extensions.Logging;
using App_CrediVnzl.Services;
using App_CrediVnzl.Pages;
using App_CrediVnzl.ViewModels;

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
                builder.Services.AddSingleton<ReportesService>();
                builder.Services.AddSingleton<AuthService>();

                // Registrar ViewModels con sus dependencias
                builder.Services.AddTransient<ConfiguracionCuentaViewModel>(sp =>
                {
                    var authService = sp.GetRequiredService<AuthService>();
                    var databaseService = sp.GetRequiredService<DatabaseService>();
                    return new ConfiguracionCuentaViewModel(authService, databaseService);
                });

                // Registrar páginas de autenticación
                builder.Services.AddTransient<LoginPage>();
                builder.Services.AddTransient<LoginAdminPage>();
                builder.Services.AddTransient<LoginClientePage>();
                builder.Services.AddTransient<PrimerUsoPage>();
                builder.Services.AddTransient<DashboardClientePage>();
                builder.Services.AddTransient<GestionarUsuariosPage>();
                builder.Services.AddTransient<ConfiguracionCuentaPage>();

                // Registrar páginas simplificadas
                builder.Services.AddTransient<BienvenidaPage>();

                // Registrar páginas existentes
                builder.Services.AddTransient<DashboardPage>();
                builder.Services.AddTransient<ClientesPage>();
                builder.Services.AddTransient<NuevoClientePage>();
                builder.Services.AddTransient<DetalleClientePage>();
                builder.Services.AddTransient<EditarClientePage>();
                builder.Services.AddTransient<NuevoPrestamoPage>();
                builder.Services.AddTransient<RegistrarPagoPage>();
                builder.Services.AddTransient<HistorialPrestamosPage>();
                builder.Services.AddTransient<CalendarioPagosPage>();
                builder.Services.AddTransient<EnviarMensajesPage>();
                builder.Services.AddTransient<ConfiguracionPage>();
                builder.Services.AddTransient<ReportesPage>();
                
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