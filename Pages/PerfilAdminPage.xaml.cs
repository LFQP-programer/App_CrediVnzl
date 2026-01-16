using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class PerfilAdminPage : ContentPage
    {
        public PerfilAdminPage(AuthService authService, DatabaseService databaseService)
        {
            InitializeComponent();
            BindingContext = new PerfilAdminViewModel(authService, databaseService);
        }

        private async void OnVolverTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}