using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.Pages
{
    public partial class ClientesPage : ContentPage
    {
        private readonly ClientesViewModel _viewModel;

        public ClientesPage(DatabaseService databaseService)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** ClientesPage Constructor - Iniciando ***");
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("*** ClientesPage Constructor - InitializeComponent OK ***");
                
                _viewModel = new ClientesViewModel(databaseService);
                System.Diagnostics.Debug.WriteLine("*** ClientesPage Constructor - ViewModel creado ***");
                
                BindingContext = _viewModel;
                System.Diagnostics.Debug.WriteLine("*** ClientesPage Constructor - BindingContext asignado ***");
                System.Diagnostics.Debug.WriteLine("*** ClientesPage Constructor - Completo ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN ClientesPage Constructor ***: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"*** InnerException: {ex.InnerException?.Message}");
                throw;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            try
            {
                System.Diagnostics.Debug.WriteLine("*** ClientesPage OnAppearing - Iniciando ***");
                
                await _viewModel.LoadClientesAsync();
                System.Diagnostics.Debug.WriteLine("*** ClientesPage OnAppearing - Clientes cargados ***");
                
                UpdateClientesCount();
                System.Diagnostics.Debug.WriteLine("*** ClientesPage OnAppearing - Título actualizado ***");
                System.Diagnostics.Debug.WriteLine("*** ClientesPage OnAppearing - Completo ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN ClientesPage.OnAppearing ***: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"*** InnerException: {ex.InnerException?.Message}");
                
                await DisplayAlert("Error", $"Error al cargar clientes: {ex.Message}\n\nDetalles: {ex.InnerException?.Message ?? "N/A"}", "OK");
            }
        }

        private void UpdateClientesCount()
        {
            try
            {
                Title = $"Clientes ({_viewModel.Clientes.Count})";
                System.Diagnostics.Debug.WriteLine($"*** ClientesPage - Título actualizado: {Title} ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN UpdateClientesCount ***: {ex.Message}");
            }
        }
    }
}
