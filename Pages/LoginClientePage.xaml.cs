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
            
            // Focus on DNI entry field
            Task.Run(async () =>
            {
                await Task.Delay(300);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DniEntry.Focus();
                });
            });
        }

        private async void OnVolverClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//bienvenida");
        }
    }
}
