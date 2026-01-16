using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;
using App_CrediVnzl.Pages;

namespace App_CrediVnzl.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly DashboardPage? _page;
        private int _totalClientes;
        private int _clientesMorosos;
        private int _prestamosActivos;
        private int _prestamosVencidos;
        private decimal _capitalInvertido;
        private decimal _capitalDisponible;
        private decimal _gananciaCobrada;
        private decimal _gananciaPendiente;
        private decimal _capitalEnCalle;
        private decimal _interesesAcumulados;
        private decimal _capitalInicial;
        private decimal _gananciaTotal;
        private bool _mostrarDialogoCapital;
        private bool _mostrarDialogoGanancias;
        private bool _mostrarMenuHamburguesa;

        public int TotalClientes
        {
            get => _totalClientes;
            set { _totalClientes = value; OnPropertyChanged(); }
        }

        public int ClientesMorosos
        {
            get => _clientesMorosos;
            set { _clientesMorosos = value; OnPropertyChanged(); }
        }

        public int PrestamosActivos
        {
            get => _prestamosActivos;
            set { _prestamosActivos = value; OnPropertyChanged(); }
        }

        public int PrestamosVencidos
        {
            get => _prestamosVencidos;
            set { _prestamosVencidos = value; OnPropertyChanged(); }
        }

        public decimal CapitalInvertido
        {
            get => _capitalInvertido;
            set { _capitalInvertido = value; OnPropertyChanged(); }
        }

        public decimal CapitalDisponible
        {
            get => _capitalDisponible;
            set { _capitalDisponible = value; OnPropertyChanged(); }
        }

        public decimal GananciaCobrada
        {
            get => _gananciaCobrada;
            set { _gananciaCobrada = value; OnPropertyChanged(); }
        }

        public decimal GananciaPendiente
        {
            get => _gananciaPendiente;
            set { _gananciaPendiente = value; OnPropertyChanged(); }
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

        public decimal CapitalInicial
        {
            get => _capitalInicial;
            set { _capitalInicial = value; OnPropertyChanged(); }
        }

        public decimal GananciaTotal
        {
            get => _gananciaTotal;
            set { _gananciaTotal = value; OnPropertyChanged(); }
        }

        public bool MostrarDialogoCapital
        {
            get => _mostrarDialogoCapital;
            set { _mostrarDialogoCapital = value; OnPropertyChanged(); }
        }

        public bool MostrarDialogoGanancias
        {
            get => _mostrarDialogoGanancias;
            set { _mostrarDialogoGanancias = value; OnPropertyChanged(); }
        }

        public bool MostrarMenuHamburguesa
        {
            get => _mostrarMenuHamburguesa;
            set { _mostrarMenuHamburguesa = value; OnPropertyChanged(); }
        }

        public ObservableCollection<DashboardCard> DashboardCards { get; set; } = new();
        public ObservableCollection<MenuCard> MenuCards { get; set; } = new();
        public ObservableCollection<PrestamoActivo> PrestamosActivosList { get; set; } = new();

        public ICommand ConfigurarCapitalCommand { get; }
        public ICommand GuardarCapitalCommand { get; }
        public ICommand CancelarCapitalCommand { get; }
        public ICommand VerGananciasCommand { get; }
        public ICommand CerrarGananciasCommand { get; }

        public DashboardViewModel(DatabaseService databaseService, DashboardPage? page = null)
        {
            _databaseService = databaseService;
            _page = page;
            LoadMenuCards();
            
            ConfigurarCapitalCommand = new Command(async () => await OnConfigurarCapitalAsync());
            GuardarCapitalCommand = new Command(async () => await OnGuardarCapitalAsync());
            CancelarCapitalCommand = new Command(async () => await OnCancelarCapitalAsync());
            VerGananciasCommand = new Command(async () => await OnVerGananciasAsync());
            CerrarGananciasCommand = new Command(async () => await OnCerrarGananciasAsync());
        }

        public async Task LoadDashboardDataAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** DashboardViewModel - Iniciando carga de datos ***");
                
                // Cargar configuracion de capital
                var capitalConfig = await _databaseService.GetCapitalConfigAsync();
                if (capitalConfig != null)
                {
                    CapitalInicial = capitalConfig.CapitalInicial;
                    CapitalDisponible = capitalConfig.CapitalDisponible;
                    GananciaTotal = capitalConfig.GananciaTotal;
                    System.Diagnostics.Debug.WriteLine($"*** Capital config cargado: Inicial={CapitalInicial}, Disponible={CapitalDisponible} ***");
                }

                // Cargar datos desde la base de datos
                System.Diagnostics.Debug.WriteLine("*** Cargando estad�sticas de clientes ***");
                TotalClientes = await _databaseService.GetTotalClientesAsync();
                ClientesMorosos = await _databaseService.GetClientesMorososAsync();
                
                System.Diagnostics.Debug.WriteLine("*** Cargando estad�sticas de pr�stamos ***");
                PrestamosActivos = await _databaseService.GetClientesConPrestamosActivosAsync();
                PrestamosVencidos = await _databaseService.GetPrestamosVencidosAsync();
                
                System.Diagnostics.Debug.WriteLine("*** Cargando estad�sticas de capital ***");
                CapitalInvertido = await _databaseService.GetTotalCapitalActivoAsync();
                CapitalEnCalle = CapitalInvertido; // Son lo mismo
                
                System.Diagnostics.Debug.WriteLine("*** Cargando estad�sticas de intereses ***");
                InteresesAcumulados = await _databaseService.GetTotalInteresGeneradoAsync();
                GananciaCobrada = await _databaseService.GetGananciaCobradaAsync();
                GananciaPendiente = await _databaseService.GetGananciaPendienteAsync();

                System.Diagnostics.Debug.WriteLine($"*** Estad�sticas cargadas: Clientes={TotalClientes}, Morosos={ClientesMorosos}, Activos={PrestamosActivos}, Vencidos={PrestamosVencidos} ***");

                // Actualizar capital disponible
                await _databaseService.ActualizarCapitalDisponibleAsync();
                capitalConfig = await _databaseService.GetCapitalConfigAsync();
                if (capitalConfig != null)
                {
                    CapitalDisponible = capitalConfig.CapitalDisponible;
                }

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
                        Title = "Morosos",
                        Value = ClientesMorosos.ToString(),
                        Icon = "M",
                        BackgroundColor = "#F44336",
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
                        Title = "Vencidos",
                        Value = PrestamosVencidos.ToString(),
                        Icon = "V",
                        BackgroundColor = "#FF9800",
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
                    },
                    new DashboardCard
                    {
                        Title = "Ganancia Cobrada",
                        Value = $"${GananciaCobrada:N2}",
                        Icon = "G",
                        BackgroundColor = "#8BC34A",
                        IconColor = "#FFFFFF"
                    },
                    new DashboardCard
                    {
                        Title = "Ganancia Pendiente",
                        Value = $"${GananciaPendiente:N2}",
                        Icon = "P",
                        BackgroundColor = "#FFC107",
                        IconColor = "#FFFFFF"
                    }
                };

                System.Diagnostics.Debug.WriteLine("*** Cargando pr�stamos activos ***");
                await LoadPrestamosActivos();
                
                System.Diagnostics.Debug.WriteLine("*** DashboardViewModel - Carga de datos completada ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en DashboardViewModel.LoadDashboardDataAsync: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                System.Diagnostics.Debug.WriteLine($"*** InnerException: {ex.InnerException?.Message} ***");
                throw; // Re-lanzar para que el DashboardPage lo maneje
            }
        }

        private async Task OnConfigurarCapitalAsync()
        {
            MostrarDialogoCapital = true;
            
            if (_page != null)
            {
                await _page.AnimarAperturaPopup();
            }
        }

        private async Task OnGuardarCapitalAsync()
        {
            try
            {
                var config = await _databaseService.GetCapitalConfigAsync();
                if (config != null)
                {
                    config.CapitalInicial = CapitalInicial;
                    await _databaseService.SaveCapitalConfigAsync(config);
                    await _databaseService.ActualizarCapitalDisponibleAsync();
                    
                    // Recargar datos
                    await LoadDashboardDataAsync();
                }
                
                if (_page != null)
                {
                    await _page.AnimarCierrePopup();
                }
                
                MostrarDialogoCapital = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando capital: {ex.Message}");
            }
        }

        private async Task OnCancelarCapitalAsync()
        {
            if (_page != null)
            {
                await _page.AnimarCierrePopup();
            }
            
            MostrarDialogoCapital = false;
        }

        private async Task OnVerGananciasAsync()
        {
            MostrarDialogoGanancias = true;
            
            if (_page != null)
            {
                await _page.AnimarAperturaPopupGanancias();
            }
        }

        private async Task OnCerrarGananciasAsync()
        {
            if (_page != null)
            {
                await _page.AnimarCierrePopupGanancias();
            }
            
            MostrarDialogoGanancias = false;
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
            var prestamos = await _databaseService.GetPrestamosActivosAsync();
            PrestamosActivosList = new ObservableCollection<PrestamoActivo>();
            
            foreach (var prestamo in prestamos)
            {
                var cliente = await _databaseService.GetClienteAsync(prestamo.ClienteId);
                if (cliente != null)
                {
                    var porcentajePagado = prestamo.MontoInicial > 0 
                        ? (int)((prestamo.MontoPagado / prestamo.MontoInicial) * 100) 
                        : 0;

                    // Calcular cuota semanal (capital + inter�s)
                    var cuotaSemanal = prestamo.CapitalPendiente * (prestamo.TasaInteresSemanal / 100);
                    
                    // Calcular fecha de vencimiento (�ltima fecha de pago + 7 d�as)
                    var fechaVencimiento = (prestamo.FechaUltimoPago ?? prestamo.FechaInicio).AddDays(7);

                    var prestamoActivo = new PrestamoActivo
                    {
                        ClienteNombre = cliente.NombreCompleto,
                        MontoInicial = prestamo.MontoInicial,
                        InteresSemanal = prestamo.CapitalPendiente * (prestamo.TasaInteresSemanal / 100),
                        MontoPagado = prestamo.MontoPagado,
                        MontoPendiente = prestamo.TotalAdeudado,
                        PorcentajePagado = porcentajePagado,
                        CuotaSemanal = cuotaSemanal,
                        FechaVencimiento = fechaVencimiento
                    };
                    
                    // Determinar el estado autom�ticamente
                    prestamoActivo.DeterminarEstado();
                    
                    PrestamosActivosList.Add(prestamoActivo);
                }
            }
            
            // Ordenar por estado: Vencidos primero, luego atrasados, luego al d�a
            PrestamosActivosList = new ObservableCollection<PrestamoActivo>(
                PrestamosActivosList.OrderBy(p => p.EstadoColor == "#E4002B" ? 0 : 
                                                   p.EstadoColor == "#FF9800" ? 1 :
                                                   p.EstadoColor == "#FFC107" ? 2 : 3)
            );
            
            OnPropertyChanged(nameof(PrestamosActivosList));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
