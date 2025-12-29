using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class ReportesViewModel : INotifyPropertyChanged
    {
        private readonly ReportesService _reportesService;
        private ResumenReportes? _resumenActual;
        private PeriodoReporte _periodoSeleccionado = PeriodoReporte.Mes;
        private DateTime _fechaInicio;
        private DateTime _fechaFin;
        private bool _isLoading;
        private string _tituloReporte = "Reporte del Mes";

        public ObservableCollection<ClienteTop> TopClientes { get; set; } = new();
        public ObservableCollection<ClienteRiesgo> ClientesEnRiesgo { get; set; } = new();
        public ObservableCollection<DatoGrafico> DatosGrafico { get; set; } = new();

        public ICommand CambiarPeriodoCommand { get; }
        public ICommand RefrescarCommand { get; }
        public ICommand VerDetalleFinancieroCommand { get; }
        public ICommand VerDetallePrestamosCommand { get; }
        public ICommand VerDetalleClientesCommand { get; }

        public ReportesViewModel(ReportesService reportesService)
        {
            _reportesService = reportesService;

            // Inicializar fechas para el mes actual
            _fechaFin = DateTime.Now;
            _fechaInicio = new DateTime(_fechaFin.Year, _fechaFin.Month, 1);

            // Comandos
            CambiarPeriodoCommand = new Command<string>(async (periodo) => await CambiarPeriodo(periodo));
            RefrescarCommand = new Command(async () => await CargarReportesAsync());
            VerDetalleFinancieroCommand = new Command(VerDetalleFinanciero);
            VerDetallePrestamosCommand = new Command(VerDetallePrestamos);
            VerDetalleClientesCommand = new Command(VerDetalleClientes);
        }

        #region Propiedades de Resumen Financiero

        public decimal CapitalTotal => _resumenActual?.Financiero.CapitalTotal ?? 0;
        public decimal CapitalEnCalle => _resumenActual?.Financiero.CapitalEnCalle ?? 0;
        public decimal CapitalDisponible => _resumenActual?.Financiero.CapitalDisponible ?? 0;
        public decimal GananciaTotal => _resumenActual?.Financiero.GananciaTotal ?? 0;
        public decimal InteresesPendientes => _resumenActual?.Financiero.InteresesPendientes ?? 0;
        public decimal ROI => _resumenActual?.Financiero.ROI ?? 0;
        public decimal CambioGanancias => _resumenActual?.Financiero.CambioGanancias ?? 0;
        public string TendenciaGanancias => CambioGanancias >= 0 ? "?" : "?";
        public string ColorTendencia => CambioGanancias >= 0 ? "#4CAF50" : "#F44336";
        public decimal PorcentajeCambio => _resumenActual?.Financiero.PorcentajeCambioGanancias ?? 0;

        #endregion

        #region Propiedades de Préstamos

        public int TotalPrestamos => _resumenActual?.Prestamos.TotalPrestamos ?? 0;
        public int PrestamosActivos => _resumenActual?.Prestamos.PrestamosActivos ?? 0;
        public int PrestamosCompletados => _resumenActual?.Prestamos.PrestamosCompletados ?? 0;
        public int PrestamosVencidos => _resumenActual?.Prestamos.PrestamosVencidos ?? 0;
        public decimal MontoPromedioPrestamo => _resumenActual?.Prestamos.MontoPromedio ?? 0;
        public decimal TasaMorosidad => _resumenActual?.Prestamos.TasaMorosidad ?? 0;

        #endregion

        #region Propiedades de Clientes

        public int TotalClientes => _resumenActual?.Clientes.TotalClientes ?? 0;
        public int ClientesActivos => _resumenActual?.Clientes.ClientesActivos ?? 0;
        public int ClientesNuevos => _resumenActual?.Clientes.ClientesNuevosEsteMes ?? 0;

        #endregion

        #region Propiedades de Flujo de Caja

        public decimal TotalCobrado => _resumenActual?.FlujoCaja.TotalCobrado ?? 0;
        public decimal TotalPrestado => _resumenActual?.FlujoCaja.TotalPrestado ?? 0;
        public decimal FlujoNeto => _resumenActual?.FlujoCaja.FlujoNeto ?? 0;
        public string ColorFlujoNeto => FlujoNeto >= 0 ? "#4CAF50" : "#F44336";

        #endregion

        #region Propiedades de Control

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string TituloReporte
        {
            get => _tituloReporte;
            set
            {
                _tituloReporte = value;
                OnPropertyChanged();
            }
        }

        public PeriodoReporte PeriodoSeleccionado
        {
            get => _periodoSeleccionado;
            set
            {
                _periodoSeleccionado = value;
                OnPropertyChanged();
                ActualizarFechasSegunPeriodo();
            }
        }

        public string FechaRango => $"{_fechaInicio:dd/MM/yyyy} - {_fechaFin:dd/MM/yyyy}";

        #endregion

        #region Métodos Públicos

        public async Task CargarReportesAsync()
        {
            try
            {
                IsLoading = true;

                _resumenActual = await _reportesService.ObtenerResumenCompletoAsync(_fechaInicio, _fechaFin);

                // Actualizar todas las propiedades
                OnPropertyChanged(nameof(CapitalTotal));
                OnPropertyChanged(nameof(CapitalEnCalle));
                OnPropertyChanged(nameof(CapitalDisponible));
                OnPropertyChanged(nameof(GananciaTotal));
                OnPropertyChanged(nameof(InteresesPendientes));
                OnPropertyChanged(nameof(ROI));
                OnPropertyChanged(nameof(CambioGanancias));
                OnPropertyChanged(nameof(TendenciaGanancias));
                OnPropertyChanged(nameof(ColorTendencia));
                OnPropertyChanged(nameof(PorcentajeCambio));

                OnPropertyChanged(nameof(TotalPrestamos));
                OnPropertyChanged(nameof(PrestamosActivos));
                OnPropertyChanged(nameof(PrestamosCompletados));
                OnPropertyChanged(nameof(PrestamosVencidos));
                OnPropertyChanged(nameof(MontoPromedioPrestamo));
                OnPropertyChanged(nameof(TasaMorosidad));

                OnPropertyChanged(nameof(TotalClientes));
                OnPropertyChanged(nameof(ClientesActivos));
                OnPropertyChanged(nameof(ClientesNuevos));

                OnPropertyChanged(nameof(TotalCobrado));
                OnPropertyChanged(nameof(TotalPrestado));
                OnPropertyChanged(nameof(FlujoNeto));
                OnPropertyChanged(nameof(ColorFlujoNeto));

                // Actualizar colecciones
                TopClientes.Clear();
                foreach (var cliente in _resumenActual.Clientes.TopClientes.Take(5))
                {
                    TopClientes.Add(cliente);
                }

                ClientesEnRiesgo.Clear();
                foreach (var cliente in _resumenActual.Clientes.ClientesEnRiesgo.Take(5))
                {
                    ClientesEnRiesgo.Add(cliente);
                }

                // Cargar datos para gráfico
                var datosGrafico = await _reportesService.ObtenerDatosGraficoEvolucionAsync(_periodoSeleccionado);
                DatosGrafico.Clear();
                foreach (var dato in datosGrafico)
                {
                    DatosGrafico.Add(dato);
                }

                OnPropertyChanged(nameof(FechaRango));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CargarReportesAsync: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    "No se pudieron cargar los reportes. Intenta nuevamente.",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Métodos Privados

        private void ActualizarFechasSegunPeriodo()
        {
            _fechaFin = DateTime.Now;

            switch (_periodoSeleccionado)
            {
                case PeriodoReporte.Hoy:
                    _fechaInicio = DateTime.Today;
                    TituloReporte = "Reporte de Hoy";
                    break;

                case PeriodoReporte.Semana:
                    _fechaInicio = _fechaFin.AddDays(-7);
                    TituloReporte = "Reporte Semanal";
                    break;

                case PeriodoReporte.Mes:
                    _fechaInicio = new DateTime(_fechaFin.Year, _fechaFin.Month, 1);
                    TituloReporte = $"Reporte de {_fechaFin:MMMM yyyy}";
                    break;

                case PeriodoReporte.Trimestre:
                    _fechaInicio = _fechaFin.AddMonths(-3);
                    TituloReporte = "Reporte Trimestral";
                    break;

                case PeriodoReporte.Año:
                    _fechaInicio = new DateTime(_fechaFin.Year, 1, 1);
                    TituloReporte = $"Reporte de {_fechaFin.Year}";
                    break;
            }
        }

        private async Task CambiarPeriodo(string periodo)
        {
            PeriodoSeleccionado = periodo switch
            {
                "Hoy" => PeriodoReporte.Hoy,
                "Semana" => PeriodoReporte.Semana,
                "Mes" => PeriodoReporte.Mes,
                "Trimestre" => PeriodoReporte.Trimestre,
                "Año" => PeriodoReporte.Año,
                _ => PeriodoReporte.Mes
            };

            await CargarReportesAsync();
        }

        private void VerDetalleFinanciero()
        {
            // Navegar a detalle financiero (implementar después)
            Application.Current!.MainPage!.DisplayAlert(
                "Detalle Financiero",
                $"Capital Total: ${CapitalTotal:N2}\n" +
                $"Capital en Calle: ${CapitalEnCalle:N2}\n" +
                $"Disponible: ${CapitalDisponible:N2}\n" +
                $"Ganancias: ${GananciaTotal:N2}\n" +
                $"ROI: {ROI:F2}%",
                "OK");
        }

        private void VerDetallePrestamos()
        {
            Application.Current!.MainPage!.DisplayAlert(
                "Detalle de Préstamos",
                $"Total: {TotalPrestamos}\n" +
                $"Activos: {PrestamosActivos}\n" +
                $"Completados: {PrestamosCompletados}\n" +
                $"Vencidos: {PrestamosVencidos}\n" +
                $"Monto Promedio: ${MontoPromedioPrestamo:N2}\n" +
                $"Tasa Morosidad: {TasaMorosidad:F2}%",
                "OK");
        }

        private void VerDetalleClientes()
        {
            Application.Current!.MainPage!.DisplayAlert(
                "Detalle de Clientes",
                $"Total: {TotalClientes}\n" +
                $"Activos: {ClientesActivos}\n" +
                $"Nuevos este mes: {ClientesNuevos}",
                "OK");
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
