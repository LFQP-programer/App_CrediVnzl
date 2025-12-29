using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private DashboardViewModel? _viewModel;

        public DashboardPage(DatabaseService databaseService)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** DashboardPage Constructor - Iniciando ***");
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("*** DashboardPage Constructor - InitializeComponent OK ***");
                _databaseService = databaseService;
                System.Diagnostics.Debug.WriteLine("*** DashboardPage Constructor - Completo ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN DashboardPage Constructor ***: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException?.Message}");
                throw;
            }
        }

        protected override async void OnAppearing()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** DashboardPage OnAppearing - Iniciando ***");
                base.OnAppearing();
                
                System.Diagnostics.Debug.WriteLine("*** DashboardPage OnAppearing - Inicializando DB ***");
                await _databaseService.InitializeAsync();
                System.Diagnostics.Debug.WriteLine("*** DashboardPage OnAppearing - DB Inicializada OK ***");
                
                if (_viewModel == null)
                {
                    System.Diagnostics.Debug.WriteLine("*** DashboardPage OnAppearing - Creando ViewModel ***");
                    _viewModel = new DashboardViewModel(_databaseService);
                    BindingContext = _viewModel;
                    System.Diagnostics.Debug.WriteLine("*** DashboardPage OnAppearing - ViewModel creado OK ***");
                }
                
                System.Diagnostics.Debug.WriteLine("*** DashboardPage OnAppearing - Cargando datos ***");
                await _viewModel.LoadDashboardDataAsync();
                System.Diagnostics.Debug.WriteLine("*** DashboardPage OnAppearing - Datos cargados OK ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN DashboardPage.OnAppearing ***: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException?.Message}");
                
                try
                {
                    await DisplayAlert("Error", $"Error al cargar el dashboard: {ex.Message}", "OK");
                }
                catch
                {
                    // Si falla el DisplayAlert, al menos tenemos los logs
                }
            }
        }

        private async void OnClientesTapped(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("clientes");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN OnClientesTapped ***: {ex.Message}");
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnCalendarioTapped(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("calendario");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN OnCalendarioTapped ***: {ex.Message}");
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnMensajesTapped(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("mensajes");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN OnMensajesTapped ***: {ex.Message}");
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnConfiguracionTapped(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("configuracion");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN OnConfiguracionTapped ***: {ex.Message}");
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
