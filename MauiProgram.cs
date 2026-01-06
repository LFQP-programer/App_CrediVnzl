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
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("╔═══════════════════════════════════════════════════════════╗");
                System.Diagnostics.Debug.WriteLine("║                CONFIGURANDO MAUI APP                     ║");
                System.Diagnostics.Debug.WriteLine("╚═══════════════════════════════════════════════════════════╝");
                System.Diagnostics.Debug.WriteLine("");
                
                System.Diagnostics.Debug.WriteLine("*** MauiProgram.CreateMauiApp - Iniciando configuración ***");
                
                var builder = MauiApp.CreateBuilder();
                System.Diagnostics.Debug.WriteLine("*** Builder creado OK ***");
                
                builder
                    .UseMauiApp<App>()
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    });

                System.Diagnostics.Debug.WriteLine("*** Configuración básica de MAUI OK ***");

#if DEBUG
                builder.Logging.AddDebug();
                System.Diagnostics.Debug.WriteLine("*** Logging configurado para DEBUG ***");
#endif

                // Registrar servicios principales
                try
                {
                    System.Diagnostics.Debug.WriteLine("*** Registrando servicios principales ***");
                    
                    builder.Services.AddSingleton<DatabaseService>();
                    System.Diagnostics.Debug.WriteLine("   → DatabaseService registrado ✓");
                    
                    builder.Services.AddSingleton<WhatsAppService>();
                    System.Diagnostics.Debug.WriteLine("   → WhatsAppService registrado ✓");
                    
                    builder.Services.AddSingleton<ReportesService>();
                    System.Diagnostics.Debug.WriteLine("   → ReportesService registrado ✓");
                    
                    builder.Services.AddSingleton<AuthService>();
                    System.Diagnostics.Debug.WriteLine("   → AuthService registrado ✓");
                    
                    System.Diagnostics.Debug.WriteLine("*** Todos los servicios principales registrados OK ***");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ ERROR registrando servicios principales: {ex.Message}");
                    throw;
                }

                // Registrar ViewModels críticos
                try
                {
                    System.Diagnostics.Debug.WriteLine("*** Registrando ViewModels ***");
                    builder.Services.AddTransient<ConfiguracionCuentaViewModel>(sp =>
                    {
                        var authService = sp.GetRequiredService<AuthService>();
                        var databaseService = sp.GetRequiredService<DatabaseService>();
                        return new ConfiguracionCuentaViewModel(authService, databaseService);
                    });
                    System.Diagnostics.Debug.WriteLine("   → ConfiguracionCuentaViewModel registrado ✓");
                    System.Diagnostics.Debug.WriteLine("*** ViewModels registrados OK ***");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ ERROR registrando ViewModels: {ex.Message}");
                    throw;
                }

                // Registrar páginas críticas (las que se usan frecuentemente)
                try
                {
                    System.Diagnostics.Debug.WriteLine("*** Registrando páginas críticas ***");
                    
                    builder.Services.AddTransient<BienvenidaPage>();
                    System.Diagnostics.Debug.WriteLine("   → BienvenidaPage ✓");
                    
                    builder.Services.AddTransient<LoginPage>();
                    System.Diagnostics.Debug.WriteLine("   → LoginPage ✓");
                    
                    builder.Services.AddTransient<LoginAdminPage>();
                    System.Diagnostics.Debug.WriteLine("   → LoginAdminPage ✓");
                    
                    builder.Services.AddTransient<LoginClientePage>();
                    System.Diagnostics.Debug.WriteLine("   → LoginClientePage ✓");
                    
                    builder.Services.AddTransient<ConfiguracionInicialPage>();
                    System.Diagnostics.Debug.WriteLine("   → ConfiguracionInicialPage ✓");
                    
                    builder.Services.AddTransient<DashboardPage>();
                    System.Diagnostics.Debug.WriteLine("   → DashboardPage ✓");
                    
                    builder.Services.AddTransient<DashboardClientePage>();
                    System.Diagnostics.Debug.WriteLine("   → DashboardClientePage ✓");
                    
                    builder.Services.AddTransient<GestionarUsuariosPage>();
                    System.Diagnostics.Debug.WriteLine("   → GestionarUsuariosPage ✓");
                    
                    builder.Services.AddTransient<ConfiguracionCuentaPage>();
                    System.Diagnostics.Debug.WriteLine("   → ConfiguracionCuentaPage ✓");
                    
                    System.Diagnostics.Debug.WriteLine("*** Páginas críticas registradas OK ***");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ ERROR registrando páginas críticas: {ex.Message}");
                    throw;
                }

                // Registrar páginas secundarias
                try
                {
                    System.Diagnostics.Debug.WriteLine("*** Registrando páginas secundarias ***");
                    
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
                    
                    System.Diagnostics.Debug.WriteLine("   → 11 páginas secundarias registradas ✓");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ ERROR registrando páginas secundarias: {ex.Message}");
                    throw;
                }
                
                System.Diagnostics.Debug.WriteLine("*** Construyendo aplicación MAUI ***");
                var app = builder.Build();
                System.Diagnostics.Debug.WriteLine("*** Aplicación MAUI construida exitosamente ***");
                
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("╔═══════════════════════════════════════════════════════════╗");
                System.Diagnostics.Debug.WriteLine("║           ✅ MAUI APP CONFIGURADA ✅                     ║");
                System.Diagnostics.Debug.WriteLine("║                                                           ║");
                System.Diagnostics.Debug.WriteLine("║  → Servicios registrados correctamente                   ║");
                System.Diagnostics.Debug.WriteLine("║  → Páginas registradas correctamente                     ║");
                System.Diagnostics.Debug.WriteLine("║  → ViewModels registrados correctamente                  ║");
                System.Diagnostics.Debug.WriteLine("║  → Aplicación lista para usar                            ║");
                System.Diagnostics.Debug.WriteLine("╚═══════════════════════════════════════════════════════════╝");
                System.Diagnostics.Debug.WriteLine("");
                
                return app;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("💥💥💥 ERROR CRÍTICO EN CreateMauiApp 💥💥💥");
                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine("");
                
                // Re-lanzar la excepción para que el sistema la maneje
                throw;
            }
        }
    }
}