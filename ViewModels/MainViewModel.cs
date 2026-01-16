using System.Windows.Input;

namespace App_CrediVnzl.ViewModels
{
    public class MainViewModel
    {
        public ICommand AdministradorCommand { get; }
        public ICommand ClienteCommand { get; }

        public MainViewModel()
        {
            AdministradorCommand = new Command(async () => await OnAdministradorClicked());
            ClienteCommand = new Command(async () => await OnClienteClicked());
        }

        private async Task OnAdministradorClicked()
        {
            await Shell.Current.GoToAsync("//login");
        }

        private async Task OnClienteClicked()
        {
            await Shell.Current.GoToAsync("//logincliente");
        }
    }
}
