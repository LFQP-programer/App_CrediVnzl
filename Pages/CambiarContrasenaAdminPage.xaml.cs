using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class CambiarContrasenaAdminPage : ContentPage
    {
        public CambiarContrasenaAdminPage(AuthService authService)
        {
            InitializeComponent();
            BindingContext = new CambiarContrasenaAdminViewModel(authService);
        }

        private async void OnVolverTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}