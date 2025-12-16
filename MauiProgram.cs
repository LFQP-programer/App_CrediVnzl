using Microsoft.Extensions.Logging;
using App_CrediVnzl.Services;
using App_CrediVnzl.Pages;

namespace App_CrediVnzl
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
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

            // Registrar paginas
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ClientesPage>();
            builder.Services.AddTransient<NuevoClientePage>();

            return builder.Build();
        }
    }
}
