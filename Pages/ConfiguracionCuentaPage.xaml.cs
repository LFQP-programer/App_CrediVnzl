using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class ConfiguracionCuentaPage : ContentPage
    {
        private readonly ConfiguracionCuentaViewModel _viewModel;

        public ConfiguracionCuentaPage(AuthService authService, DatabaseService databaseService)
        {
            InitializeComponent();
            _viewModel = new ConfiguracionCuentaViewModel(authService, databaseService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarDatosUsuarioAsync();
        }
    }
}
