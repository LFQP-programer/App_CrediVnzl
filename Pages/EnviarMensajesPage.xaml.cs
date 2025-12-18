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
            
            try
            {
                await _viewModel.LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en EnviarMensajesPage.OnAppearing: {ex.Message}");
                await DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "OK");
            }
        }
    }
}
