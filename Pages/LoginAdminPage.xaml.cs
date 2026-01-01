using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class LoginAdminPage : ContentPage
    {
        private readonly LoginViewModel _viewModel;

        public LoginAdminPage(AuthService authService)
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
