using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class CalendarioPagosPage : ContentPage
    {
        private readonly CalendarioPagosViewModel _viewModel;

        public CalendarioPagosPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _viewModel = new CalendarioPagosViewModel(databaseService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            try
            {
                await _viewModel.RefrescarDatosAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CalendarioPagosPage.OnAppearing: {ex.Message}");
                await DisplayAlert("Error", $"Error al cargar calendario: {ex.Message}", "OK");
            }
        }
    }
}
