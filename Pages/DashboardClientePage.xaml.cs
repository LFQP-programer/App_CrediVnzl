using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class DashboardClientePage : ContentPage
    {
        private readonly DashboardClienteViewModel _viewModel;

        public DashboardClientePage(DatabaseService databaseService, AuthService authService)
        {
            InitializeComponent();
            _viewModel = new DashboardClienteViewModel(databaseService, authService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDataAsync();
        }
    }
}
