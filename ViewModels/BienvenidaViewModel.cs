using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace App_CrediVnzl.ViewModels
{
    public class BienvenidaViewModel : INotifyPropertyChanged
    {
        public ICommand AdminLoginCommand { get; }
        public ICommand ClienteLoginCommand { get; }

        public BienvenidaViewModel()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** BienvenidaViewModel - Constructor iniciando ***");
                
                AdminLoginCommand = new Command(async () => await OnAdminLoginAsync());
                ClienteLoginCommand = new Command(async () => await OnClienteLoginAsync());
                
                System.Diagnostics.Debug.WriteLine("*** BienvenidaViewModel - Constructor completado exitosamente ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en BienvenidaViewModel Constructor: {ex.Message}");
                throw;
            }
        }

        private async Task OnAdminLoginAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** BienvenidaViewModel - Usuario tocó opción ADMINISTRADOR ***");
                
                // Verificar si estamos en un Shell
                if (Shell.Current != null)
                {
                    System.Diagnostics.Debug.WriteLine("*** Navegando a LoginAdmin usando Shell.GoToAsync ***");
                    // Usar la página individual de admin
                    await Shell.Current.GoToAsync("loginadmin");
                    System.Diagnostics.Debug.WriteLine("*** Navegación a LoginAdmin completada exitosamente ***");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("*** Shell.Current es null - intentando método alternativo ***");
                    
                    if (Application.Current?.Windows?.FirstOrDefault()?.Page is NavigationPage navPage)
                    {
                        System.Diagnostics.Debug.WriteLine("*** Usando NavigationPage.PushAsync ***");
                        var authService = Application.Current.Handler?.MauiContext?.Services.GetService<Services.AuthService>();
                        if (authService != null)
                        {
                            // Usar la página individual
                            var loginPage = new Pages.LoginAdminPage(authService);
                            await navPage.PushAsync(loginPage);
                            System.Diagnostics.Debug.WriteLine("*** NavigationPage.PushAsync completado ***");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("*** ERROR: AuthService no disponible ***");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("*** ERROR: NavigationPage no disponible ***");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnAdminLoginAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                try
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.DisplayAlert("Error", 
                            $"No se pudo navegar al login de administrador.\n\nError: {ex.Message}", 
                            "OK");
                    }
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"*** ERROR mostrando alerta: {ex2.Message}");
                }
            }
        }

        private async Task OnClienteLoginAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** BienvenidaViewModel - Usuario tocó opción CLIENTE ***");
                
                // Verificar si estamos en un Shell
                if (Shell.Current != null)
                {
                    System.Diagnostics.Debug.WriteLine("*** Navegando a LoginCliente usando Shell.GoToAsync ***");
                    // Usar la página individual de cliente
                    await Shell.Current.GoToAsync("logincliente");
                    System.Diagnostics.Debug.WriteLine("*** Navegación a LoginCliente completada exitosamente ***");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("*** Shell.Current es null - intentando método alternativo ***");
                    
                    if (Application.Current?.Windows?.FirstOrDefault()?.Page is NavigationPage navPage)
                    {
                        System.Diagnostics.Debug.WriteLine("*** Usando NavigationPage.PushAsync ***");
                        var authService = Application.Current.Handler?.MauiContext?.Services.GetService<Services.AuthService>();
                        if (authService != null)
                        {
                            // Usar la página individual
                            var loginPage = new Pages.LoginClientePage(authService);
                            await navPage.PushAsync(loginPage);
                            System.Diagnostics.Debug.WriteLine("*** NavigationPage.PushAsync completado ***");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("*** ERROR: AuthService no disponible ***");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("*** ERROR: NavigationPage no disponible ***");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnClienteLoginAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                try
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.DisplayAlert("Error", 
                            $"No se pudo navegar al login de cliente.\n\nError: {ex.Message}", 
                            "OK");
                    }
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"*** ERROR mostrando alerta: {ex2.Message}");
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
