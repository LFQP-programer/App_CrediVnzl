using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(AuthService authService)
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(authService);
        }
    }
}
