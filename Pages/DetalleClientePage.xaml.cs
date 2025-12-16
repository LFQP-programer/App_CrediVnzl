using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class DetalleClientePage : ContentPage
    {
        private readonly DetalleClienteViewModel _viewModel;

        public DetalleClientePage(DatabaseService databaseService)
        {
            InitializeComponent();
            _viewModel = new DetalleClienteViewModel(databaseService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDataAsync();
        }
    }
}
