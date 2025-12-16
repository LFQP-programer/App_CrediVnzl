using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private DashboardViewModel? _viewModel;

        public DashboardPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            await _databaseService.InitializeAsync();
            
            if (_viewModel == null)
            {
                _viewModel = new DashboardViewModel(_databaseService);
                BindingContext = _viewModel;
            }
            
            await _viewModel.LoadDashboardDataAsync();
        }

        private async void OnClientesTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("clientes");
        }
    }
}
