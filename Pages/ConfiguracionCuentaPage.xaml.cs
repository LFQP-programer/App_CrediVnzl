using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class ConfiguracionCuentaPage : ContentPage
    {
        private readonly ConfiguracionCuentaViewModel _viewModel;

        public ConfiguracionCuentaPage(ConfiguracionCuentaViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarDatosUsuarioAsync();
        }
    }
}
