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
                    _viewModel = new DashboardViewModel(_databaseService, this);
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

        public async Task AnimarAperturaPopup()
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var modal = this.FindByName<Frame>("ModalCapital");
                if (modal != null)
                {
                    modal.Scale = 0.8;
                    modal.Opacity = 0;
                    
                    var scaleTask = modal.ScaleTo(1, 300, Easing.CubicOut);
                    var fadeTask = modal.FadeTo(1, 250);
                    
                    await Task.WhenAll(scaleTask, fadeTask);
                }
            });
        }

        public async Task AnimarCierrePopup()
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var modal = this.FindByName<Frame>("ModalCapital");
                if (modal != null)
                {
                    var scaleTask = modal.ScaleTo(0.8, 200, Easing.CubicIn);
                    var fadeTask = modal.FadeTo(0, 200);
                    
                    await Task.WhenAll(scaleTask, fadeTask);
                }
            });
        }

        public async Task AnimarAperturaPopupGanancias()
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var modal = this.FindByName<Frame>("ModalGanancias");
                if (modal != null)
                {
                    modal.Scale = 0.8;
                    modal.Opacity = 0;
                    
                    var scaleTask = modal.ScaleTo(1, 300, Easing.CubicOut);
                    var fadeTask = modal.FadeTo(1, 250);
                    
                    await Task.WhenAll(scaleTask, fadeTask);
                }
            });
        }

        public async Task AnimarCierrePopupGanancias()
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var modal = this.FindByName<Frame>("ModalGanancias");
                if (modal != null)
                {
                    var scaleTask = modal.ScaleTo(0.8, 200, Easing.CubicIn);
                    var fadeTask = modal.FadeTo(0, 200);
                    
                    await Task.WhenAll(scaleTask, fadeTask);
                }
            });
        }

        private void OnMenuButtonClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** OnMenuButtonClicked - Abriendo Flyout ***");
                Shell.Current.FlyoutIsPresented = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN OnMenuButtonClicked ***: {ex.Message}");
            }
        }

        private void OnMenuTapped(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** OnMenuTapped - Abriendo Flyout ***");
                Shell.Current.FlyoutIsPresented = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN OnMenuTapped ***: {ex.Message}");
            }
        }

        private async void OnClientesTapped(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** OnClientesTapped - Iniciando navegación ***");
                System.Diagnostics.Debug.WriteLine($"*** Shell.Current: {Shell.Current != null} ***");
                System.Diagnostics.Debug.WriteLine($"*** Shell.Current.Navigation: {Shell.Current?.Navigation != null} ***");
                
                await Shell.Current.GoToAsync("clientes");
                
                System.Diagnostics.Debug.WriteLine("*** OnClientesTapped - Navegación completada ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN OnClientesTapped ***: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"*** InnerException: {ex.InnerException?.Message}");
                
                await DisplayAlert("Error de navegación", 
                    $"No se pudo navegar a Clientes.\n\nError: {ex.Message}\n\nDetalles: {ex.InnerException?.Message ?? "N/A"}", 
                    "OK");
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

        private async void OnReportesTapped(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("reportes");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR EN OnReportesTapped ***: {ex.Message}");
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
