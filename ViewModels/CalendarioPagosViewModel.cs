using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class CalendarioPagosViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        
        private DateTime _fechaSeleccionada;
        private string _mesYearTexto = string.Empty;
        private decimal _totalPagosDelDia;
        private int _cantidadPagosPendientes;
        private int _cantidadPagosVencidos;
        private int _cantidadPagosPagados;
        private decimal _totalEsperadoMes;
        private int _totalPagosMes;
        private string _filtroSeleccionado = "Todos";
        private bool _isLoading;

        public DateTime FechaSeleccionada
        {
            get => _fechaSeleccionada;
            set
            {
                _fechaSeleccionada = value;
                OnPropertyChanged();
            }
        }

        public string MesYearTexto
        {
            get => _mesYearTexto;
            set
            {
                _mesYearTexto = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DiaCalendario> DiasCalendario { get; set; } = new();
        public ObservableCollection<PagoCalendario> PagosDelDia { get; set; } = new();

        public decimal TotalPagosDelDia
        {
            get => _totalPagosDelDia;
            set
            {
                _totalPagosDelDia = value;
                OnPropertyChanged();
            }
        }

        public int CantidadPagosPendientes
        {
            get => _cantidadPagosPendientes;
            set
            {
                _cantidadPagosPendientes = value;
                OnPropertyChanged();
            }
        }

        public int CantidadPagosVencidos
        {
            get => _cantidadPagosVencidos;
            set
            {
                _cantidadPagosVencidos = value;
                OnPropertyChanged();
            }
        }

        public int CantidadPagosPagados
        {
            get => _cantidadPagosPagados;
            set
            {
                _cantidadPagosPagados = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalEsperadoMes
        {
            get => _totalEsperadoMes;
            set
            {
                _totalEsperadoMes = value;
                OnPropertyChanged();
            }
        }

        public int TotalPagosMes
        {
            get => _totalPagosMes;
            set
            {
                _totalPagosMes = value;
                OnPropertyChanged();
            }
        }

        public string FiltroSeleccionado
        {
            get => _filtroSeleccionado;
            set
            {
                _filtroSeleccionado = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand MesAnteriorCommand { get; }
        public ICommand MesSiguienteCommand { get; }
        public ICommand IrHoyCommand { get; }
        public ICommand SeleccionarDiaCommand { get; }
        public ICommand MarcarComoPagadoCommand { get; }
        public ICommand VerDetallePagoCommand { get; }
        public ICommand CambiarFiltroCommand { get; }

        public CalendarioPagosViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            FechaSeleccionada = DateTime.Today;

            MesAnteriorCommand = new Command(async () => await MesAnteriorAsync());
            MesSiguienteCommand = new Command(async () => await MesSiguienteAsync());
            IrHoyCommand = new Command(async () => await IrHoyAsync());
            SeleccionarDiaCommand = new Command<DiaCalendario>(async (dia) => await SeleccionarDiaAsync(dia));
            MarcarComoPagadoCommand = new Command<PagoCalendario>(async (pago) => await MarcarComoPagadoAsync(pago));
            VerDetallePagoCommand = new Command<PagoCalendario>(async (pago) => await VerDetallePagoAsync(pago));
            CambiarFiltroCommand = new Command<string>(async (filtro) => await CambiarFiltroAsync(filtro));

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await CargarCalendarioAsync();
            await CargarPagosDelDiaAsync(FechaSeleccionada);
        }

        private async Task MesAnteriorAsync()
        {
            FechaSeleccionada = FechaSeleccionada.AddMonths(-1);
            await CargarCalendarioAsync();
        }

        private async Task MesSiguienteAsync()
        {
            FechaSeleccionada = FechaSeleccionada.AddMonths(1);
            await CargarCalendarioAsync();
        }

        private async Task IrHoyAsync()
        {
            FechaSeleccionada = DateTime.Today;
            await CargarCalendarioAsync();
            await CargarPagosDelDiaAsync(FechaSeleccionada);
        }

        private async Task SeleccionarDiaAsync(DiaCalendario dia)
        {
            if (!dia.EsMesActual) return;

            foreach (var d in DiasCalendario)
            {
                d.EstaSeleccionado = false;
            }

            dia.EstaSeleccionado = true;
            FechaSeleccionada = dia.Fecha;

            await CargarPagosDelDiaAsync(dia.Fecha);
        }

        private async Task MarcarComoPagadoAsync(PagoCalendario pago)
        {
            try
            {
                IsLoading = true;

                var success = await _databaseService.MarcarPagoCalendarioComoPagadoAsync(pago.Id, DateTime.Now);

                if (success)
                {
                    await CargarCalendarioAsync();
                    await CargarPagosDelDiaAsync(FechaSeleccionada);

                    await Application.Current!.MainPage!.DisplayAlert(
                        "Éxito",
                        "Pago marcado como realizado",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    $"Error al marcar pago: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task VerDetallePagoAsync(PagoCalendario pago)
        {
            await Shell.Current.GoToAsync($"detallecliente?clienteId={pago.ClienteId}");
        }

        private async Task CambiarFiltroAsync(string filtro)
        {
            FiltroSeleccionado = filtro;
            await CargarPagosDelDiaAsync(FechaSeleccionada);
        }

        private async Task CargarCalendarioAsync()
        {
            try
            {
                IsLoading = true;

                var year = FechaSeleccionada.Year;
                var month = FechaSeleccionada.Month;

                var meses = new[] { "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO",
                                   "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE" };
                MesYearTexto = $"{meses[month - 1]} {year}";

                var pagosDelMes = await _databaseService.GetPagosCalendarioDelMesAsync(year, month);
                
                var resumen = await _databaseService.GetResumenCalendarioMesAsync(year, month);
                TotalEsperadoMes = resumen.MontoEsperado;
                TotalPagosMes = resumen.TotalMes;

                var primerDiaDelMes = new DateTime(year, month, 1);
                
                var primerDiaMostrar = primerDiaDelMes;
                while (primerDiaMostrar.DayOfWeek != DayOfWeek.Sunday)
                {
                    primerDiaMostrar = primerDiaMostrar.AddDays(-1);
                }

                DiasCalendario.Clear();
                var fechaActual = primerDiaMostrar;
                var hoy = DateTime.Today;

                for (int i = 0; i < 42; i++)
                {
                    var esMesActual = fechaActual.Month == month;
                    var pagosDelDia = pagosDelMes.Where(p => p.FechaPago.Date == fechaActual.Date).ToList();
                    
                    var dia = new DiaCalendario
                    {
                        Dia = fechaActual.Day,
                        Fecha = fechaActual,
                        EsMesActual = esMesActual,
                        TienePagos = pagosDelDia.Any(),
                        CantidadPagos = pagosDelDia.Count,
                        PagosPendientes = pagosDelDia.Count(p => !p.EsPagado && p.FechaPago.Date >= hoy),
                        PagosVencidos = pagosDelDia.Count(p => !p.EsPagado && p.FechaPago.Date < hoy),
                        PagosPagados = pagosDelDia.Count(p => p.EsPagado),
                        EsHoy = fechaActual.Date == hoy,
                        EstaSeleccionado = fechaActual.Date == FechaSeleccionada.Date
                    };

                    DiasCalendario.Add(dia);
                    fechaActual = fechaActual.AddDays(1);
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    $"Error al cargar calendario: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CargarPagosDelDiaAsync(DateTime fecha)
        {
            try
            {
                IsLoading = true;

                var todosPagos = await _databaseService.GetPagosCalendarioPorFechaAsync(fecha);
                
                var pagosFiltrados = FiltroSeleccionado switch
                {
                    "Pendientes" => todosPagos.Where(p => !p.EsPagado && p.FechaPago.Date >= DateTime.Today).ToList(),
                    "Vencidos" => todosPagos.Where(p => !p.EsPagado && p.FechaPago.Date < DateTime.Today).ToList(),
                    "Pagados" => todosPagos.Where(p => p.EsPagado).ToList(),
                    _ => todosPagos
                };

                PagosDelDia.Clear();
                foreach (var pago in pagosFiltrados.OrderBy(p => p.ClienteNombre))
                {
                    PagosDelDia.Add(pago);
                }

                TotalPagosDelDia = PagosDelDia.Sum(p => p.MontoPago);
                CantidadPagosPendientes = todosPagos.Count(p => !p.EsPagado && p.FechaPago.Date >= DateTime.Today);
                CantidadPagosVencidos = todosPagos.Count(p => !p.EsPagado && p.FechaPago.Date < DateTime.Today);
                CantidadPagosPagados = todosPagos.Count(p => p.EsPagado);
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    $"Error al cargar pagos: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task RefrescarDatosAsync()
        {
            await CargarCalendarioAsync();
            await CargarPagosDelDiaAsync(FechaSeleccionada);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
