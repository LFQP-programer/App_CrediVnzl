using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class DashboardPage : ContentPage
    {
        private DashboardViewModel? _viewModel;
        private readonly DatabaseService _databaseService;

        public DashboardPage(DatabaseService databaseService)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** DashboardPage - Constructor iniciado ***");
                
                _databaseService = databaseService;
                
                InitializeComponent();
                
                System.Diagnostics.Debug.WriteLine("*** DashboardPage - InitializeComponent completado ***");
                
                _viewModel = new DashboardViewModel(_databaseService, this);
                BindingContext = _viewModel;
                
                System.Diagnostics.Debug.WriteLine("*** DashboardPage - ViewModel asignado ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en DashboardPage Constructor: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                System.Diagnostics.Debug.WriteLine($"*** InnerException: {ex.InnerException?.Message} ***");
                throw;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                System.Diagnostics.Debug.WriteLine("*** DashboardPage - OnAppearing iniciado ***");
                
                // Asegurar que la base de datos est� inicializada
                System.Diagnostics.Debug.WriteLine("*** DashboardPage - Verificando inicializaci�n de base de datos ***");
                await _databaseService.InitializeAsync();
                System.Diagnostics.Debug.WriteLine("*** DashboardPage - Base de datos verificada ***");
                
                if (_viewModel != null)
                {
                    System.Diagnostics.Debug.WriteLine("*** DashboardPage - Cargando datos del dashboard ***");
                    await _viewModel.LoadDashboardDataAsync();
                    System.Diagnostics.Debug.WriteLine("*** DashboardPage - Datos cargados exitosamente ***");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("*** ERROR: ViewModel es null en OnAppearing ***");
                    await DisplayAlert("Error", "Error al inicializar el dashboard", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en DashboardPage.OnAppearing: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                System.Diagnostics.Debug.WriteLine($"*** InnerException: {ex.InnerException?.Message} ***");
                
                await DisplayAlert(
                    "Error",
                    $"Error al cargar el dashboard:\n\n{ex.Message}\n\nPor favor, intenta nuevamente.",
                    "OK");
            }
        }

        // M�todo para el men� hamburguesa
        private void OnMenuHamburgesaTapped(object sender, EventArgs e)
        {
            try
            {
                if (_viewModel != null)
                {
                    _viewModel.MostrarMenuHamburguesa = !_viewModel.MostrarMenuHamburguesa;
                    
                    if (_viewModel.MostrarMenuHamburguesa)
                    {
                        AnimarAperturaMenuHamburguesa();
                    }
                    else
                    {
                        AnimarCierreMenuHamburguesa();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnMenuHamburgesaTapped: {ex.Message} ***");
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
                System.Diagnostics.Debug.WriteLine("*** OnClientesTapped - Navegando a clientes ***");
                await Shell.Current.GoToAsync("clientes");
                System.Diagnostics.Debug.WriteLine("*** OnClientesTapped - Navegaci�n completada ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnClientesTapped: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                await DisplayAlert("Error", $"Error al navegar a Clientes: {ex.Message}", "OK");
            }
        }

        private async void OnCalendarioTapped(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** OnCalendarioTapped - Navegando a calendario ***");
                await Shell.Current.GoToAsync("calendario");
                System.Diagnostics.Debug.WriteLine("*** OnCalendarioTapped - Navegaci�n completada ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnCalendarioTapped: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                await DisplayAlert("Error", $"Error al navegar a Calendario: {ex.Message}", "OK");
            }
        }

        private async void OnMensajesTapped(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** OnMensajesTapped - Navegando a mensajes ***");
                await Shell.Current.GoToAsync("mensajes");
                System.Diagnostics.Debug.WriteLine("*** OnMensajesTapped - Navegaci�n completada ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnMensajesTapped: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                await DisplayAlert("Error", $"Error al navegar a Mensajes: {ex.Message}", "OK");
            }
        }

        private async void OnReportesTapped(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** OnReportesTapped - Navegando a reportes ***");
                await Shell.Current.GoToAsync("reportes");
                System.Diagnostics.Debug.WriteLine("*** OnReportesTapped - Navegaci�n completada ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnReportesTapped: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                await DisplayAlert("Error", $"Error al navegar a Reportes: {ex.Message}", "OK");
            }
        }

        private async void OnNuevoPrestamoTapped(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** OnNuevoPrestamoTapped - Navegando a nuevo pr�stamo ***");
                await Shell.Current.GoToAsync("nuevoprestamo");
                System.Diagnostics.Debug.WriteLine("*** OnNuevoPrestamoTapped - Navegaci�n completada ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnNuevoPrestamoTapped: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                await DisplayAlert("Error", $"Error al navegar a Nuevo Pr�stamo: {ex.Message}", "OK");
            }
        }

        // Animaciones
        public async Task AnimarAperturaPopup()
        {
            try
            {
                var overlay = this.FindByName<BoxView>("OverlayCapital");
                var popup = this.FindByName<Border>("PopupCapital");

                if (overlay != null && popup != null)
                {
                    overlay.IsVisible = true;
                    popup.IsVisible = true;
                    
                    overlay.Opacity = 0;
                    popup.Scale = 0.8;
                    popup.Opacity = 0;

                    await Task.WhenAll(
                        overlay.FadeTo(1, 200),
                        popup.ScaleTo(1, 300, Easing.SpringOut),
                        popup.FadeTo(1, 200)
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en AnimarAperturaPopup: {ex.Message} ***");
            }
        }

        public async Task AnimarCierrePopup()
        {
            try
            {
                var overlay = this.FindByName<BoxView>("OverlayCapital");
                var popup = this.FindByName<Border>("PopupCapital");

                if (overlay != null && popup != null)
                {
                    await Task.WhenAll(
                        overlay.FadeTo(0, 200),
                        popup.ScaleTo(0.8, 200, Easing.CubicIn),
                        popup.FadeTo(0, 200)
                    );

                    overlay.IsVisible = false;
                    popup.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en AnimarCierrePopup: {ex.Message} ***");
            }
        }

        public async Task AnimarAperturaPopupGanancias()
        {
            try
            {
                var overlay = this.FindByName<BoxView>("OverlayGanancias");
                var popup = this.FindByName<Border>("PopupGanancias");

                if (overlay != null && popup != null)
                {
                    overlay.IsVisible = true;
                    popup.IsVisible = true;
                    
                    overlay.Opacity = 0;
                    popup.Scale = 0.8;
                    popup.Opacity = 0;

                    await Task.WhenAll(
                        overlay.FadeTo(1, 200),
                        popup.ScaleTo(1, 300, Easing.SpringOut),
                        popup.FadeTo(1, 200)
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en AnimarAperturaPopupGanancias: {ex.Message} ***");
            }
        }

        public async Task AnimarCierrePopupGanancias()
        {
            try
            {
                var overlay = this.FindByName<BoxView>("OverlayGanancias");
                var popup = this.FindByName<Border>("PopupGanancias");

                if (overlay != null && popup != null)
                {
                    await Task.WhenAll(
                        overlay.FadeTo(0, 200),
                        popup.ScaleTo(0.8, 200, Easing.CubicIn),
                        popup.FadeTo(0, 200)
                    );

                    overlay.IsVisible = false;
                    popup.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en AnimarCierrePopupGanancias: {ex.Message} ***");
            }
        }

        private async void AnimarAperturaMenuHamburguesa()
        {
            try
            {
                var overlay = this.FindByName<BoxView>("OverlayMenu");
                var menu = this.FindByName<Border>("MenuHamburguesa");

                if (overlay != null && menu != null)
                {
                    overlay.IsVisible = true;
                    menu.IsVisible = true;
                    
                    overlay.Opacity = 0;
                    menu.TranslationX = menu.Width;

                    await Task.WhenAll(
                        overlay.FadeTo(1, 200),
                        menu.TranslateTo(0, 0, 300, Easing.CubicOut)
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en AnimarAperturaMenuHamburguesa: {ex.Message} ***");
            }
        }

        private async void AnimarCierreMenuHamburguesa()
        {
            try
            {
                var overlay = this.FindByName<BoxView>("OverlayMenu");
                var menu = this.FindByName<Border>("MenuHamburguesa");

                if (overlay != null && menu != null)
                {
                    await Task.WhenAll(
                        overlay.FadeTo(0, 200),
                        menu.TranslateTo(menu.Width, 0, 200, Easing.CubicIn)
                    );

                    overlay.IsVisible = false;
                    menu.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en AnimarCierreMenuHamburguesa: {ex.Message} ***");
            }
        }

        private async void OnCerrarSesionTapped(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** OnCerrarSesionTapped - Cerrando sesi�n ***");
                
                if (_viewModel != null)
                {
                    _viewModel.MostrarMenuHamburguesa = false;
                }
                
                await Shell.Current.GoToAsync("//main");
                System.Diagnostics.Debug.WriteLine("*** OnCerrarSesionTapped - Navegaci�n completada ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnCerrarSesionTapped: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
            }
        }

        private async void OnPerfilTapped(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** OnPerfilTapped - Navegando a perfil admin ***");
                
                if (_viewModel != null)
                {
                    _viewModel.MostrarMenuHamburguesa = false;
                }
                
                await Shell.Current.GoToAsync("perfiladmin");
                System.Diagnostics.Debug.WriteLine("*** OnPerfilTapped - Navegaci�n completada ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnPerfilTapped: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                await DisplayAlert("Error", $"Error al navegar a Perfil: {ex.Message}", "OK");
            }
        }

        private async void OnConfiguracionTapped(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** OnConfiguracionTapped - Navegando a configuraci�n ***");
                
                if (_viewModel != null)
                {
                    _viewModel.MostrarMenuHamburguesa = false;
                }
                
                await Shell.Current.GoToAsync("configuracion");
                System.Diagnostics.Debug.WriteLine("*** OnConfiguracionTapped - Navegaci�n completada ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnConfiguracionTapped: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                await DisplayAlert("Error", $"Error al navegar a Configuraci�n: {ex.Message}", "OK");
            }
        }

        private async void OnAyudaTapped(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** OnAyudaTapped - Navegando a ayuda ***");
                
                if (_viewModel != null)
                {
                    _viewModel.MostrarMenuHamburguesa = false;
                }
                
                await Shell.Current.GoToAsync("ayuda");
                System.Diagnostics.Debug.WriteLine("*** OnAyudaTapped - Navegaci�n completada ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnAyudaTapped: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                await DisplayAlert("Error", $"Error al navegar a Ayuda: {ex.Message}", "OK");
            }
        }

        private void OnOverlayMenuTapped(object sender, EventArgs e)
        {
            try
            {
                if (_viewModel != null)
                {
                    _viewModel.MostrarMenuHamburguesa = false;
                    AnimarCierreMenuHamburguesa();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en OnOverlayMenuTapped: {ex.Message} ***");
            }
        }
    }
}
