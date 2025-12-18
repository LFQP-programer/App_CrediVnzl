using Microsoft.Extensions.DependencyInjection;

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