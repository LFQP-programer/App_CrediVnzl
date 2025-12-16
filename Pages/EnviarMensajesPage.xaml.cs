using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class EnviarMensajesPage : ContentPage
    {
        private readonly EnviarMensajesViewModel _viewModel;

        public EnviarMensajesPage(DatabaseService databaseService, WhatsAppService whatsAppService)
        {
            InitializeComponent();
            _viewModel = new EnviarMensajesViewModel(databaseService, whatsAppService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDataAsync();
        }
    }
}
