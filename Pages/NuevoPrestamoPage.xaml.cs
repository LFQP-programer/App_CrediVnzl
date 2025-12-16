using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class NuevoPrestamoPage : ContentPage
    {
        private readonly NuevoPrestamoViewModel _viewModel;

        public NuevoPrestamoPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _viewModel = new NuevoPrestamoViewModel(databaseService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadClientesAsync();
        }
    }
}
