using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel _viewModel;

        public LoginPage(AuthService authService)
        {
            InitializeComponent();
            _viewModel = new LoginViewModel(authService);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.CargarCredencialesGuardadas();
        }
    }
}
