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
                
                // DatabaseService debe registrarse primero ya que AuthService depende de él
                builder.Services.AddSingleton<DatabaseService>(sp =>
                {
                    System.Diagnostics.Debug.WriteLine("*** Creando instancia de DatabaseService ***");
                    var dbService = new DatabaseService();
                    // Inicializar de forma asíncrona pero no bloquear
                    Task.Run(async () =>
                    {
                        try
                        {
                            await dbService.InitializeAsync();
                            System.Diagnostics.Debug.WriteLine("*** DatabaseService inicializado correctamente ***");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"*** Error inicializando DatabaseService: {ex.Message} ***");
                        }
                    });
                    return dbService;
                });
                
                builder.Services.AddSingleton<AuthService>();
                builder.Services.AddSingleton<WhatsAppService>();
                builder.Services.AddSingleton<ReportesService>();
                builder.Services.AddSingleton<NotificationService>();
                System.Diagnostics.Debug.WriteLine("*** MauiProgram - Servicios registrados ***");

                // Registrar paginas
                System.Diagnostics.Debug.WriteLine("*** MauiProgram - Registrando páginas ***");
                builder.Services.AddTransient<BienvenidaPage>();
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
                builder.Services.AddTransient<ConfiguracionCuentaPage>();
                builder.Services.AddTransient<GestionarUsuariosPage>();
                builder.Services.AddTransient<ReportesPage>();
                builder.Services.AddTransient<PerfilAdminPage>();
                builder.Services.AddTransient<CambiarContrasenaAdminPage>();
                builder.Services.AddTransient<PrimerUsoPage>();
                builder.Services.AddTransient<AyudaPage>();
                System.Diagnostics.Debug.WriteLine("*** MauiProgram - Páginas registradas OK ***");
                
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