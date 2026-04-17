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
            System.Diagnostics.Debug.WriteLine("*** BienvenidaViewModel - Constructor llamado ***");
            AdminLoginCommand = new Command(async () => await OnAdminLoginAsync());
            ClienteLoginCommand = new Command(async () => await OnClienteLoginAsync());
            System.Diagnostics.Debug.WriteLine("*** BienvenidaViewModel - Comandos inicializados ***");
        }

        private async Task OnAdminLoginAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** BienvenidaViewModel - OnAdminLoginAsync INICIADO ***");
                System.Diagnostics.Debug.WriteLine($"*** Shell.Current: {Shell.Current != null} ***");
                System.Diagnostics.Debug.WriteLine($"*** Application.Current: {Application.Current != null} ***");
                
                if (Shell.Current != null)
                {
                    System.Diagnostics.Debug.WriteLine("*** Navegando con Shell.GoToAsync a loginadmin (ruta absoluta) ***");
                    await Shell.Current.GoToAsync("//loginadmin");
                    System.Diagnostics.Debug.WriteLine("*** Navegaci�n completada ***");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("*** Shell.Current es NULL ***");
                    await Application.Current!.MainPage!.DisplayAlert("Error", "Shell no disponible", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnAdminLoginAsync: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        private async Task OnClienteLoginAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** BienvenidaViewModel - OnClienteLoginAsync INICIADO ***");
                System.Diagnostics.Debug.WriteLine($"*** Shell.Current: {Shell.Current != null} ***");
                System.Diagnostics.Debug.WriteLine($"*** Application.Current: {Application.Current != null} ***");
                
                if (Shell.Current != null)
                {
                    System.Diagnostics.Debug.WriteLine("*** Navegando con Shell.GoToAsync a logincliente (ruta absoluta) ***");
                    await Shell.Current.GoToAsync("//logincliente");
                    System.Diagnostics.Debug.WriteLine("*** Navegaci�n completada ***");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("*** Shell.Current es NULL ***");
                    await Application.Current!.MainPage!.DisplayAlert("Error", "Shell no disponible", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnClienteLoginAsync: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
