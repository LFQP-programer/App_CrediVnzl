using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class RegistrarPagoPage : ContentPage
    {
        private readonly RegistrarPagoViewModel _viewModel;

        public RegistrarPagoPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _viewModel = new RegistrarPagoViewModel(databaseService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDataAsync();
        }
    }
}
