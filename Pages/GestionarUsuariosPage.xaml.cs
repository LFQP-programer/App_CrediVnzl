using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class GestionarUsuariosPage : ContentPage
    {
        private readonly GestionarUsuariosViewModel _viewModel;

        public GestionarUsuariosPage(DatabaseService databaseService, AuthService authService, WhatsAppService whatsAppService)
        {
            InitializeComponent();
            _viewModel = new GestionarUsuariosViewModel(databaseService, authService, whatsAppService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDataAsync();
        }
    }
}
