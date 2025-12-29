using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class EditarClientePage : ContentPage
    {
        private readonly EditarClienteViewModel _viewModel;

        public EditarClientePage(DatabaseService databaseService)
        {
            InitializeComponent();
            _viewModel = new EditarClienteViewModel(databaseService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadClienteAsync();
        }
    }
}
