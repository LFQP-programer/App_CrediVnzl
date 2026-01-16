using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    [QueryProperty(nameof(ClienteId), "clienteId")]
    public partial class ClienteDashboardPage : ContentPage
    {
        private ClienteDashboardViewModel? _viewModel;
        private int _clienteId;

        public int ClienteId 
        { 
            get => _clienteId;
            set 
            { 
                _clienteId = value;
                System.Diagnostics.Debug.WriteLine($"*** ClienteDashboardPage - ClienteId recibido: {value} ***");
                
                // Cargar datos cuando se establece el ClienteId
                if (_viewModel != null && value > 0)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await _viewModel.LoadClienteDataAsync(value);
                    });
                }
            }
        }

        public ClienteDashboardPage(DatabaseService databaseService, WhatsAppService whatsAppService)
        {
            InitializeComponent();
            _viewModel = new ClienteDashboardViewModel(databaseService, whatsAppService);
            BindingContext = _viewModel;
            
            System.Diagnostics.Debug.WriteLine("*** ClienteDashboardPage - Constructor llamado ***");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            System.Diagnostics.Debug.WriteLine($"*** ClienteDashboardPage - OnAppearing llamado, ClienteId: {ClienteId} ***");
            
            // Intentar cargar datos si a�n no se han cargado
            if (_viewModel != null && ClienteId > 0)
            {
                await _viewModel.LoadClienteDataAsync(ClienteId);
            }
        }

        private async void OnCerrarSesionTapped(object sender, EventArgs e)
        {
            var confirmar = await DisplayAlert(
                "Cerrar Sesion",
                "�Estas seguro de que deseas cerrar sesion?",
                "Si",
                "No");

            if (confirmar)
            {
                // Limpiar el ClienteId al cerrar sesi�n
                ClienteId = 0;
                await Shell.Current.GoToAsync("//logincliente");
            }
        }
    }
}
