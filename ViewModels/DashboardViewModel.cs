using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private int _totalClientes;
        private int _prestamosActivos;
        private decimal _capitalEnCalle;
        private decimal _interesesAcumulados;

        public int TotalClientes
        {
            get => _totalClientes;
            set { _totalClientes = value; OnPropertyChanged(); }
        }

        public int PrestamosActivos
        {
            get => _prestamosActivos;
            set { _prestamosActivos = value; OnPropertyChanged(); }
        }

        public decimal CapitalEnCalle
        {
            get => _capitalEnCalle;
            set { _capitalEnCalle = value; OnPropertyChanged(); }
        }

        public decimal InteresesAcumulados
        {
            get => _interesesAcumulados;
            set { _interesesAcumulados = value; OnPropertyChanged(); }
        }

        public ObservableCollection<DashboardCard> DashboardCards { get; set; } = new();
        public ObservableCollection<MenuCard> MenuCards { get; set; } = new();
        public ObservableCollection<PrestamoActivo> PrestamosActivosList { get; set; } = new();

        public DashboardViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadMenuCards();
        }

        public async Task LoadDashboardDataAsync()
        {
            try
            {
                // Cargar datos desde la base de datos
                TotalClientes = await _databaseService.GetTotalClientesAsync();
                PrestamosActivos = await _databaseService.GetClientesConPrestamosActivosAsync();
                CapitalEnCalle = 0; // Se calculara cuando se implementen los prestamos
                InteresesAcumulados = 0; // Se calculara cuando se implementen los prestamos

                DashboardCards = new ObservableCollection<DashboardCard>
                {
                    new DashboardCard
                    {
                        Title = "Clientes",
                        Value = TotalClientes.ToString(),
                        Icon = "C",
                        BackgroundColor = "#2196F3",
                        IconColor = "#FFFFFF"
                    },
                    new DashboardCard
                    {
                        Title = "Activos",
                        Value = PrestamosActivos.ToString(),
                        Icon = "A",
                        BackgroundColor = "#4CAF50",
                        IconColor = "#FFFFFF"
                    },
                    new DashboardCard
                    {
                        Title = "Capital en la Calle",
                        Value = $"${CapitalEnCalle:N2}",
                        Icon = "$",
                        BackgroundColor = "#9C27B0",
                        IconColor = "#FFFFFF"
                    },
                    new DashboardCard
                    {
                        Title = "Intereses Acumulados",
                        Value = $"${InteresesAcumulados:N2}",
                        Icon = "I",
                        BackgroundColor = "#FF5722",
                        IconColor = "#FFFFFF"
                    }
                };

                await LoadPrestamosActivos();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando datos del dashboard: {ex.Message}");
            }
        }

        private void LoadMenuCards()
        {
            MenuCards = new ObservableCollection<MenuCard>
            {
                new MenuCard
                {
                    Title = "Clientes",
                    Subtitle = "Gestionar clientes",
                    Icon = "C",
                    BackgroundColor = "#2196F3",
                    Route = "clientes"
                },
                new MenuCard
                {
                    Title = "Calendario",
                    Subtitle = "Pagos programados",
                    Icon = "Cal",
                    BackgroundColor = "#4CAF50",
                    Route = "calendario"
                },
                new MenuCard
                {
                    Title = "Mensajes",
                    Subtitle = "Enviar recordatorios",
                    Icon = "M",
                    BackgroundColor = "#9C27B0",
                    Route = "mensajes"
                },
                new MenuCard
                {
                    Title = "Reportes",
                    Subtitle = "Estadisticas",
                    Icon = "R",
                    BackgroundColor = "#FF5722",
                    Route = "reportes"
                }
            };
        }

        private async Task LoadPrestamosActivos()
        {
            // Lista vacia - se implementara cuando se agreguen prestamos
            PrestamosActivosList = new ObservableCollection<PrestamoActivo>();
            await Task.CompletedTask;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
