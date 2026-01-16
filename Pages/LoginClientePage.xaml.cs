using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class LoginClientePage : ContentPage
    {
        public LoginClientePage(AuthService authService, DatabaseService databaseService)
        {
            InitializeComponent();
            BindingContext = new LoginClienteViewModel(authService, databaseService);
        }
    }
}
