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
            AdminLoginCommand = new Command(async () => await OnAdminLoginAsync());
            ClienteLoginCommand = new Command(async () => await OnClienteLoginAsync());
        }

        private async Task OnAdminLoginAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** BienvenidaViewModel - Navegando a LoginAdmin ***");
                
                // Verificar si estamos en un Shell
                if (Shell.Current != null)
                {
                    System.Diagnostics.Debug.WriteLine("*** Usando Shell.GoToAsync ***");
                    await Shell.Current.GoToAsync("loginadmin");
                }
                else if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    System.Diagnostics.Debug.WriteLine("*** Usando NavigationPage.PushAsync ***");
                    var authService = Application.Current.Handler?.MauiContext?.Services.GetService<Services.AuthService>();
                    if (authService != null)
                    {
                        var loginPage = new Pages.LoginAdminPage(authService);
                        await navPage.PushAsync(loginPage);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnAdminLoginAsync: {ex.Message} ***");
            }
        }

        private async Task OnClienteLoginAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** BienvenidaViewModel - Navegando a LoginCliente ***");
                
                // Verificar si estamos en un Shell
                if (Shell.Current != null)
                {
                    System.Diagnostics.Debug.WriteLine("*** Usando Shell.GoToAsync ***");
                    await Shell.Current.GoToAsync("logincliente");
                }
                else if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    System.Diagnostics.Debug.WriteLine("*** Usando NavigationPage.PushAsync ***");
                    var authService = Application.Current.Handler?.MauiContext?.Services.GetService<Services.AuthService>();
                    if (authService != null)
                    {
                        var loginPage = new Pages.LoginClientePage(authService);
                        await navPage.PushAsync(loginPage);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnClienteLoginAsync: {ex.Message} ***");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
