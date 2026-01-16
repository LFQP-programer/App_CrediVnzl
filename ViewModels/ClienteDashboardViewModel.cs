using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class ClienteDashboardViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly WhatsAppService _whatsAppService;
        
        private string _nombreCliente = "Cargando...";
        private string _dniCliente = "...";
        private bool _tienePrestamo;
        private decimal _prestadoTotal;
        private decimal _interesTotal;
        private decimal _pagadoTotal;
        private decimal _porPagarTotal;
        private decimal _proximoPagoMonto;
        private DateTime _proximoPagoFecha = DateTime.Now.AddDays(7);
        private DateTime _fechaInicio = DateTime.Now;
        private DateTime _fechaVencimiento = DateTime.Now.AddMonths(12);
        private int _porcentajePagado;
        private string _estadoTexto = "Activo";
        private string _estadoColor = "#4CAF50";
        private string _numeroYape = "999 999 999";
        private string _nombreYape = "CrediVzla SAC";
        private int _prestamoActivoId;
        private string _numeroContactoWhatsApp = string.Empty;

        public ObservableCollection<HistorialPago> HistorialPagos { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public string NombreCliente
        {
            get => _nombreCliente;
            set { _nombreCliente = value; OnPropertyChanged(); }
        }

        public string DniCliente
        {
            get => _dniCliente;
            set { _dniCliente = value; OnPropertyChanged(); }
        }

        public bool TienePrestamo
        {
            get => _tienePrestamo;
            set { _tienePrestamo = value; OnPropertyChanged(); }
        }

        public decimal PrestadoTotal
        {
            get => _prestadoTotal;
            set { _prestadoTotal = value; OnPropertyChanged(); }
        }

        public decimal InteresTotal
        {
            get => _interesTotal;
            set { _interesTotal = value; OnPropertyChanged(); }
        }

        public decimal PagadoTotal
        {
            get => _pagadoTotal;
            set { _pagadoTotal = value; OnPropertyChanged(); }
        }

        public decimal PorPagarTotal
        {
            get => _porPagarTotal;
            set { _porPagarTotal = value; OnPropertyChanged(); }
        }

        public decimal ProximoPagoMonto
        {
            get => _proximoPagoMonto;
            set { _proximoPagoMonto = value; OnPropertyChanged(); }
        }

        public DateTime ProximoPagoFecha
        {
            get => _proximoPagoFecha;
            set { _proximoPagoFecha = value; OnPropertyChanged(); }
        }

        public DateTime FechaInicio
        {
            get => _fechaInicio;
            set { _fechaInicio = value; OnPropertyChanged(); }
        }

        public DateTime FechaVencimiento
        {
            get => _fechaVencimiento;
            set { _fechaVencimiento = value; OnPropertyChanged(); }
        }

        public int PorcentajePagado
        {
            get => _porcentajePagado;
            set { _porcentajePagado = value; OnPropertyChanged(); }
        }

        public string EstadoTexto
        {
            get => _estadoTexto;
            set { _estadoTexto = value; OnPropertyChanged(); }
        }

        public string EstadoColor
        {
            get => _estadoColor;
            set { _estadoColor = value; OnPropertyChanged(); }
        }

        public string NumeroYape
        {
            get => _numeroYape;
            set { _numeroYape = value; OnPropertyChanged(); }
        }

        public string NombreYape
        {
            get => _nombreYape;
            set { _nombreYape = value; OnPropertyChanged(); }
        }

        public string NumeroContactoWhatsApp
        {
            get => _numeroContactoWhatsApp;
            set { _numeroContactoWhatsApp = value; OnPropertyChanged(); }
        }

        public ICommand ContactarWhatsAppCommand { get; }
        public ICommand AbrirYapeCommand { get; }
        public ICommand CopiarNumeroYapeCommand { get; }

        public ClienteDashboardViewModel(DatabaseService databaseService, WhatsAppService whatsAppService)
        {
            _databaseService = databaseService;
            _whatsAppService = whatsAppService;
            ContactarWhatsAppCommand = new Command(async () => await ContactarWhatsAppAsync());
            AbrirYapeCommand = new Command(async () => await AbrirYapeAsync());
            CopiarNumeroYapeCommand = new Command(async () => await CopiarNumeroYapeAsync());
        }

        public async Task LoadClienteDataAsync(int clienteId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"*** ClienteDashboardViewModel - Cargando datos del cliente {clienteId} ***");
                
                // Inicializar base de datos
                await _databaseService.InitializeAsync();
                System.Diagnostics.Debug.WriteLine("*** ClienteDashboardViewModel - Base de datos inicializada ***");
                
                // Cargar informaci�n del cliente
                var cliente = await _databaseService.GetClienteAsync(clienteId);
                if (cliente == null)
                {
                    System.Diagnostics.Debug.WriteLine("*** ClienteDashboardViewModel - Cliente no encontrado ***");
                    NombreCliente = "Cliente no encontrado";
                    DniCliente = "N/A";
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"*** ClienteDashboardViewModel - Cliente encontrado: {cliente.NombreCompleto} ***");
                NombreCliente = string.IsNullOrWhiteSpace(cliente.NombreCompleto) ? "Sin nombre" : cliente.NombreCompleto;
                DniCliente = string.IsNullOrWhiteSpace(cliente.NumeroDocumento) ? "Sin DNI" : cliente.NumeroDocumento;
                
                // N�mero de contacto (puedes configurarlo o obtenerlo de configuraci�n)
                NumeroContactoWhatsApp = "51999999999"; // Configurar con el n�mero real del negocio
                
                // Cargar pr�stamos activos del cliente
                var prestamos = await _databaseService.GetPrestamosByClienteAsync(clienteId);
                System.Diagnostics.Debug.WriteLine($"*** ClienteDashboardViewModel - Total pr�stamos encontrados: {prestamos.Count} ***");
                
                var prestamoActivo = prestamos.FirstOrDefault(p => p.Estado == "Activo");

                if (prestamoActivo != null)
                {
                    System.Diagnostics.Debug.WriteLine($"*** ClienteDashboardViewModel - Pr�stamo activo encontrado ***");
                    System.Diagnostics.Debug.WriteLine($"*** Monto Inicial: {prestamoActivo.MontoInicial} ***");
                    System.Diagnostics.Debug.WriteLine($"*** Capital Pendiente: {prestamoActivo.CapitalPendiente} ***");
                    System.Diagnostics.Debug.WriteLine($"*** Inter�s Acumulado: {prestamoActivo.InteresAcumulado} ***");
                    System.Diagnostics.Debug.WriteLine($"*** Total Adeudado: {prestamoActivo.TotalAdeudado} ***");
                    System.Diagnostics.Debug.WriteLine($"*** Monto Pagado: {prestamoActivo.MontoPagado} ***");
                    System.Diagnostics.Debug.WriteLine($"*** Tasa Inter�s Semanal: {prestamoActivo.TasaInteresSemanal}% ***");
                    
                    _prestamoActivoId = prestamoActivo.Id;
                    TienePrestamo = true;
                    
                    // Datos del pr�stamo
                    PrestadoTotal = prestamoActivo.MontoInicial;
                    InteresTotal = prestamoActivo.TasaInteresSemanal; // Mostrar porcentaje de inter�s
                    PagadoTotal = prestamoActivo.MontoPagado;
                    PorPagarTotal = prestamoActivo.TotalAdeudado;
                    
                    // Pr�ximo pago (interes semanal)
                    ProximoPagoMonto = prestamoActivo.CapitalPendiente * (prestamoActivo.TasaInteresSemanal / 100);
                    ProximoPagoFecha = (prestamoActivo.FechaUltimoPago ?? prestamoActivo.FechaInicio).AddDays(7);
                    
                    System.Diagnostics.Debug.WriteLine($"*** Pr�ximo Pago Monto calculado: {ProximoPagoMonto} ***");
                    System.Diagnostics.Debug.WriteLine($"*** Pr�ximo Pago Fecha: {ProximoPagoFecha:yyyy-MM-dd} ***");
                    
                    // Fechas
                    FechaInicio = prestamoActivo.FechaInicio;
                    // Si DuracionSemanas es 0, calcular una duraci�n estimada o usar un valor por defecto
                    if (prestamoActivo.DuracionSemanas > 0)
                    {
                        FechaVencimiento = prestamoActivo.FechaInicio.AddDays(prestamoActivo.DuracionSemanas * 7);
                    }
                    else
                    {
                        // Sin duraci�n fija, usar 52 semanas (1 a�o) como referencia
                        FechaVencimiento = prestamoActivo.FechaInicio.AddDays(52 * 7);
                    }
                    
                    // Porcentaje pagado - Calculado correctamente
                    var totalOriginal = prestamoActivo.MontoInicial;
                    if (totalOriginal > 0)
                    {
                        PorcentajePagado = (int)((prestamoActivo.MontoPagado / totalOriginal) * 100);
                    }
                    else
                    {
                        PorcentajePagado = 0;
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"*** Porcentaje Pagado calculado: {PorcentajePagado}% ***");
                    
                    // Determinar estado
                    DeterminarEstado(ProximoPagoFecha);
                    System.Diagnostics.Debug.WriteLine($"*** Estado determinado: {EstadoTexto} ***");
                    
                    // Cargar historial de pagos
                    await CargarHistorialPagosAsync(prestamoActivo.Id);
                    System.Diagnostics.Debug.WriteLine($"*** Historial cargado: {HistorialPagos.Count} pagos ***");
                    
                    System.Diagnostics.Debug.WriteLine("*** ClienteDashboardViewModel - Datos cargados exitosamente ***");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("*** ClienteDashboardViewModel - No tiene pr�stamos activos ***");
                    TienePrestamo = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en ClienteDashboardViewModel.LoadClienteDataAsync: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                
                NombreCliente = "Error al cargar datos";
                DniCliente = "Error";
                TienePrestamo = false;
            }
        }

        private async Task CargarHistorialPagosAsync(int prestamoId)
        {
            try
            {
                var historial = await _databaseService.GetHistorialPagosByPrestamoAsync(prestamoId);
                
                HistorialPagos.Clear();
                foreach (var pago in historial.Take(10)) // Mostrar �ltimos 10 pagos
                {
                    HistorialPagos.Add(pago);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando historial: {ex.Message}");
            }
        }

        private void DeterminarEstado(DateTime fechaVencimiento)
        {
            var diasVencido = (DateTime.Now - fechaVencimiento).Days;
            
            if (diasVencido > 14) // M�s de 2 semanas
            {
                EstadoTexto = "Vencido";
                EstadoColor = "#E4002B"; // Rojo
            }
            else if (diasVencido > 7) // M�s de 1 semana
            {
                EstadoTexto = "Atrasado";
                EstadoColor = "#FF9800"; // Naranja
            }
            else if (diasVencido > 0) // Pas� la fecha pero menos de 1 semana
            {
                EstadoTexto = "Por vencer";
                EstadoColor = "#FFC107"; // Amarillo
            }
            else
            {
                EstadoTexto = "Activo";
                EstadoColor = "#4CAF50"; // Verde
            }
        }

        private async Task ContactarWhatsAppAsync()
        {
            try
            {
                var mensaje = $"Hola, soy {NombreCliente} (DNI: {DniCliente}). Tengo una consulta sobre mi pr�stamo.";
                await _whatsAppService.EnviarMensajeAsync(NumeroContactoWhatsApp, mensaje);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al contactar por WhatsApp: {ex.Message}");
            }
        }

        private async Task AbrirYapeAsync()
        {
            try
            {
                // Nota: Yape no tiene una API p�blica oficial de deep linking
                // Las alternativas son:
                
                // 1. Intentar abrir la app de Yape (solo abre la app, no precarga datos)
                var supportsUri = await Launcher.Default.CanOpenAsync("yape://");
                
                if (supportsUri)
                {
                    await Launcher.Default.OpenAsync("yape://");
                    
                    // Mostrar instrucciones al usuario
                    await Application.Current.MainPage.DisplayAlert(
                        "Instrucciones de Pago",
                        $"?? Por favor, en la app de Yape:\n\n" +
                        $"1?? Selecciona 'Yapear'\n" +
                        $"2?? Ingresa el n�mero: {NumeroYape}\n" +
                        $"3?? Monto a pagar: S/ {ProximoPagoMonto:N2}\n" +
                        $"4?? Nombre: {NombreYape}\n\n" +
                        $"?? El n�mero ha sido copiado al portapapeles",
                        "Entendido");
                    
                    // Copiar el n�mero al portapapeles para facilitar el pago
                    await Clipboard.Default.SetTextAsync(NumeroYape.Replace(" ", ""));
                }
                else
                {
                    // Si no tiene Yape instalado, mostrar instrucciones manuales
                    var resultado = await Application.Current.MainPage.DisplayAlert(
                        "Instrucciones de Pago",
                        $"Para realizar el pago por Yape:\n\n" +
                        $"?? N�mero: {NumeroYape}\n" +
                        $"?? Monto: S/ {ProximoPagoMonto:N2}\n" +
                        $"?? Nombre: {NombreYape}\n\n" +
                        $"�Deseas copiar el n�mero?",
                        "Copiar N�mero",
                        "Cancelar");
                    
                    if (resultado)
                    {
                        await Clipboard.Default.SetTextAsync(NumeroYape.Replace(" ", ""));
                        await Application.Current.MainPage.DisplayAlert(
                            "? Copiado",
                            "El n�mero de Yape ha sido copiado al portapapeles",
                            "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al abrir Yape: {ex.Message}");
                
                // Mostrar informaci�n manual si falla
                await Application.Current.MainPage.DisplayAlert(
                    "Informaci�n de Pago",
                    $"?? N�mero de Yape: {NumeroYape}\n" +
                    $"?? Monto a pagar: S/ {ProximoPagoMonto:N2}\n" +
                    $"?? Nombre: {NombreYape}",
                    "OK");
            }
        }

        private async Task CopiarNumeroYapeAsync()
        {
            try
            {
                // Copiar n�mero sin espacios
                var numeroSinEspacios = NumeroYape.Replace(" ", "");
                await Clipboard.Default.SetTextAsync(numeroSinEspacios);
                
                await Application.Current.MainPage.DisplayAlert(
                    "? Copiado",
                    $"N�mero {numeroSinEspacios} copiado al portapapeles",
                    "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al copiar n�mero: {ex.Message}");
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
