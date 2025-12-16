using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;
using App_CrediVnzl.Models;

namespace App_CrediVnzl.Pages
{
    public partial class CalendarioPagosPage : ContentPage
    {
        private readonly CalendarioPagosViewModel _viewModel;

        public CalendarioPagosPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _viewModel = new CalendarioPagosViewModel(databaseService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDataAsync();
            ActualizarUI();
            GenerarCalendario();
        }

        private void ActualizarUI()
        {
            lblSubtitulo.Text = $"{_viewModel.Resumen.TotalMes} pagos programados";
            lblMesAnio.Text = _viewModel.GetMesAnio();
            
            if (_viewModel.MostrarCalendario)
            {
                lblEmptyMessage.Text = "No hay pagos programados para esta fecha";
            }
            else
            {
                lblEmptyMessage.Text = _viewModel.FiltroEstado == EstadoPago.Todos 
                    ? "No hay pagos vencidos este mes"
                    : $"No hay pagos {_viewModel.FiltroEstado.ToString().ToLower()}";
            }
        }

        private void GenerarCalendario()
        {
            gridCalendario.Children.Clear();
            gridCalendario.RowDefinitions.Clear();
            gridCalendario.ColumnDefinitions.Clear();

            // Crear 7 columnas (dias de la semana)
            for (int i = 0; i < 7; i++)
            {
                gridCalendario.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            var primerDiaMes = new DateTime(_viewModel.SelectedDate.Year, _viewModel.SelectedDate.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
            var diaSemanaInicio = (int)primerDiaMes.DayOfWeek;
            var totalDias = ultimoDiaMes.Day;

            var totalFilas = (int)Math.Ceiling((totalDias + diaSemanaInicio) / 7.0);
            
            for (int i = 0; i < totalFilas; i++)
            {
                gridCalendario.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            var diaActual = 1;
            var hoy = DateTime.Today;

            for (int fila = 0; fila < totalFilas; fila++)
            {
                for (int columna = 0; columna < 7; columna++)
                {
                    var celda = fila * 7 + columna;
                    
                    if (celda >= diaSemanaInicio && diaActual <= totalDias)
                    {
                        var fecha = new DateTime(_viewModel.SelectedDate.Year, _viewModel.SelectedDate.Month, diaActual);
                        var esHoy = fecha == hoy;
                        
                        // Verificar si hay pagos en esta fecha
                        var pagosDia = _viewModel.Pagos.Where(p => p.FechaProgramada.Date == fecha).ToList();
                        var tienePagos = pagosDia.Any();
                        
                        Color colorFondo = Colors.Transparent;
                        if (tienePagos)
                        {
                            if (pagosDia.Any(p => p.Estado == "Pagado"))
                                colorFondo = Color.FromArgb("#4CAF50");
                            else if (pagosDia.Any(p => p.Estado == "Vencido"))
                                colorFondo = Color.FromArgb("#F44336");
                            else
                                colorFondo = Color.FromArgb("#FFC107");
                        }

                        var frame = new Frame
                        {
                            BackgroundColor = esHoy ? Color.FromArgb("#2196F3") : colorFondo,
                            CornerRadius = 20,
                            Padding = 8,
                            HasShadow = false,
                            HeightRequest = 40,
                            WidthRequest = 40,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        };

                        var tapGesture = new TapGestureRecognizer();
                        var fechaCapturada = fecha;
                        tapGesture.Tapped += (s, e) =>
                        {
                            _viewModel.SelectedDate = fechaCapturada;
                            _viewModel.MostrarCalendario = false;
                            ActualizarUI();
                        };
                        frame.GestureRecognizers.Add(tapGesture);

                        var label = new Label
                        {
                            Text = diaActual.ToString(),
                            FontSize = 14,
                            FontAttributes = esHoy ? FontAttributes.Bold : FontAttributes.None,
                            TextColor = (esHoy || tienePagos) ? Colors.White : Color.FromArgb("#212121"),
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        };

                        frame.Content = label;
                        Grid.SetRow(frame, fila);
                        Grid.SetColumn(frame, columna);
                        gridCalendario.Children.Add(frame);

                        diaActual++;
                    }
                    else if (celda < diaSemanaInicio)
                    {
                        // Dias del mes anterior
                        var diasMesAnterior = primerDiaMes.AddDays(-1).Day;
                        var diaAnterior = diasMesAnterior - (diaSemanaInicio - celda - 1);
                        
                        var label = new Label
                        {
                            Text = diaAnterior.ToString(),
                            FontSize = 14,
                            TextColor = Color.FromArgb("#BDBDBD"),
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            Margin = new Thickness(0, 10)
                        };

                        Grid.SetRow(label, fila);
                        Grid.SetColumn(label, columna);
                        gridCalendario.Children.Add(label);
                    }
                }
            }
        }
    }
}
