using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;
using App_CrediVnzl.Models;

namespace App_CrediVnzl.Pages
{
    public partial class NuevoClientePage : ContentPage
    {
        private readonly NuevoClienteViewModel _viewModel;

        public NuevoClientePage(DatabaseService databaseService, AuthService authService, WhatsAppService whatsAppService)
        {
            InitializeComponent();
            _viewModel = new NuevoClienteViewModel(databaseService, authService, whatsAppService);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.Initialize();
        }
    }
}
