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
        private DateTime _selectedDate;
        private bool _mostrarCalendario = true;
        private EstadoPago _filtroEstado = EstadoPago.Todos;
        private ResumenPagos _resumen = new();

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
                _ = LoadPagosAsync();
            }
        }

        public bool MostrarCalendario
        {
            get => _mostrarCalendario;
            set
            {
                _mostrarCalendario = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MostrarLista));
            }
        }

        public bool MostrarLista => !_mostrarCalendario;

        public EstadoPago FiltroEstado
        {
            get => _filtroEstado;
            set
            {
                _filtroEstado = value;
                OnPropertyChanged();
                _ = LoadPagosAsync();
            }
        }

        public ResumenPagos Resumen
        {
            get => _resumen;
            set
            {
                _resumen = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Pago> Pagos { get; set; } = new();
        public ObservableCollection<Pago> PagosFiltrados { get; set; } = new();

        public ICommand CambiarVistaCommand { get; }
        public ICommand CambiarFiltroCommand { get; }
        public ICommand MarcarPagadoCommand { get; }
        public ICommand MesAnteriorCommand { get; }
        public ICommand MesSiguienteCommand { get; }

        public CalendarioPagosViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _selectedDate = DateTime.Today;
            
            CambiarVistaCommand = new Command<string>(CambiarVista);
            CambiarFiltroCommand = new Command<string>(CambiarFiltro);
            MarcarPagadoCommand = new Command<Pago>(async (pago) => await MarcarPagadoAsync(pago));
            MesAnteriorCommand = new Command(MesAnterior);
            MesSiguienteCommand = new Command(MesSiguiente);
            
            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            await _databaseService.ActualizarEstadosPagosVencidosAsync();
            await LoadResumenAsync();
            await LoadPagosAsync();
        }

        private async Task LoadResumenAsync()
        {
            Resumen = await _databaseService.GetResumenPagosMesAsync(SelectedDate.Year, SelectedDate.Month);
        }

        private async Task LoadPagosAsync()
        {
            List<Pago> pagos;
            
            if (MostrarCalendario)
            {
                // Cargar pagos del mes
                pagos = await _databaseService.GetPagosByMesAsync(SelectedDate.Year, SelectedDate.Month);
            }
            else
            {
                // Cargar pagos del dia seleccionado
                pagos = await _databaseService.GetPagosByFechaAsync(SelectedDate);
            }

            Pagos.Clear();
            foreach (var pago in pagos)
            {
                Pagos.Add(pago);
            }

            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            PagosFiltrados.Clear();
            
            IEnumerable<Pago> pagosFiltrados = Pagos;

            if (FiltroEstado != EstadoPago.Todos)
            {
                pagosFiltrados = pagosFiltrados.Where(p => p.Estado == FiltroEstado.ToString());
            }

            foreach (var pago in pagosFiltrados)
            {
                PagosFiltrados.Add(pago);
            }
        }

        private void CambiarVista(string vista)
        {
            if (vista == "Calendario")
            {
                MostrarCalendario = true;
            }
            else if (vista == "Lista")
            {
                MostrarCalendario = false;
            }
            
            _ = LoadPagosAsync();
        }

        private void CambiarFiltro(string filtro)
        {
            FiltroEstado = filtro switch
            {
                "Todos" => EstadoPago.Todos,
                "Pendientes" => EstadoPago.Pendiente,
                "Pagados" => EstadoPago.Pagado,
                "Vencidos" => EstadoPago.Vencido,
                _ => EstadoPago.Todos
            };
        }

        private async Task MarcarPagadoAsync(Pago pago)
        {
            if (pago == null) return;

            var confirmacion = await Shell.Current.DisplayAlertAsync(
                "Confirmar Pago",
                $"Marcar pago de {pago.ClienteNombre} como pagado?",
                "Si",
                "No");

            if (confirmacion)
            {
                await _databaseService.MarcarPagoComoPagadoAsync(pago.Id);
                await LoadDataAsync();
            }
        }

        private void MesAnterior()
        {
            SelectedDate = SelectedDate.AddMonths(-1);
            _ = LoadResumenAsync();
            _ = LoadPagosAsync();
        }

        private void MesSiguiente()
        {
            SelectedDate = SelectedDate.AddMonths(1);
            _ = LoadResumenAsync();
            _ = LoadPagosAsync();
        }

        public string GetMesAnio()
        {
            var meses = new[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                               "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            return $"{meses[SelectedDate.Month - 1]} {SelectedDate.Year}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
