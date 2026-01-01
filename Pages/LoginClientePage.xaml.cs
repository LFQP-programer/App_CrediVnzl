using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class LoginClientePage : ContentPage
    {
        private readonly LoginViewModel _viewModel;

        public LoginClientePage(AuthService authService)
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
