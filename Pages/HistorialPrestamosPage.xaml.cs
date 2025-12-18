using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class HistorialPrestamosPage : ContentPage
    {
        private readonly HistorialPrestamosViewModel _viewModel;

        public HistorialPrestamosPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _viewModel = new HistorialPrestamosViewModel(databaseService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDataAsync();
        }
    }
}
