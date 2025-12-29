using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class ConfiguracionPage : ContentPage
    {
        private readonly ConfiguracionViewModel _viewModel;

        public ConfiguracionPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _viewModel = new ConfiguracionViewModel(databaseService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarInformacionAsync();
        }
    }
}
