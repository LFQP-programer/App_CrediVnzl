using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class ClientesPage : ContentPage
    {
        private readonly ClientesViewModel _viewModel;

        public ClientesPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _viewModel = new ClientesViewModel(databaseService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadClientesAsync();
            UpdateClientesCount();
        }

        private void UpdateClientesCount()
        {
            lblClientesCount.Text = $"{_viewModel.Clientes.Count} registrados";
        }
    }
}
