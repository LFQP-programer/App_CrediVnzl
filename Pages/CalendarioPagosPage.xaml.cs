using App_CrediVnzl.Models;
using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;
using Microsoft.Maui.Controls.Shapes;

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
            
            try
            {
                await _viewModel.LoadDataAsync();
                ActualizarUI();
                GenerarCalendario();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CalendarioPagosPage.OnAppearing: {ex.Message}");
                await DisplayAlert("Error", $"Error al cargar calendario: {ex.Message}", "OK");
            }
        }

        private void ActualizarUI()
        {
            lblSubtitulo.Text = $"{_viewModel.Resumen.TotalMes} pagos programados";
            lblMesAnio.Text = _viewModel.GetMesAnio();
        }

        private void GenerarCalendario()
        {
            gridCalendario.Children.Clear();
            gridCalendario.RowDefinitions.Clear();
            gridCalendario.ColumnDefinitions.Clear();

            // 7 columnas (días)
            for (int i = 0; i < 7; i++)
            {
                gridCalendario.ColumnDefinitions.Add(
                    new ColumnDefinition { Width = GridLength.Star });
            }

            var primerDiaMes = new DateTime(
                _viewModel.SelectedDate.Year,
                _viewModel.SelectedDate.Month,
                1);

            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
            var diaSemanaInicio = (int)primerDiaMes.DayOfWeek;
            var totalDias = ultimoDiaMes.Day;

            var totalFilas = (int)Math.Ceiling((totalDias + diaSemanaInicio) / 7.0);

            for (int i = 0; i < totalFilas; i++)
            {
                gridCalendario.RowDefinitions.Add(
                    new RowDefinition { Height = GridLength.Auto });
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
                        var fecha = new DateTime(
                            _viewModel.SelectedDate.Year,
                            _viewModel.SelectedDate.Month,
                            diaActual);

                        var esHoy = fecha == hoy;

                        var pagosDia = _viewModel.Pagos
                            .Where(p => p.FechaProgramada.Date == fecha)
                            .ToList();

                        var tienePagos = pagosDia.Count > 0;

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

                        var border = new Border
                        {
                            BackgroundColor = esHoy
                                ? Color.FromArgb("#2196F3")
                                : colorFondo,

                            Padding = 8,
                            HeightRequest = 40,
                            WidthRequest = 40,
                            StrokeThickness = 0,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,

                            StrokeShape = new RoundRectangle
                            {
                                CornerRadius = new CornerRadius(20)
                            }
                        };

                        var tapGesture = new TapGestureRecognizer();
                        var fechaCapturada = fecha;
                        tapGesture.Tapped += (s, e) =>
                        {
                            _viewModel.SelectedDate = fechaCapturada;
                            _viewModel.MostrarCalendario = false;
                            ActualizarUI();
                        };
                        border.GestureRecognizers.Add(tapGesture);

                        var label = new Label
                        {
                            Text = diaActual.ToString(),
                            FontSize = 14,
                            FontAttributes = esHoy ? FontAttributes.Bold : FontAttributes.None,
                            TextColor = (esHoy || tienePagos)
                                ? Colors.White
                                : Color.FromArgb("#212121"),
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        };

                        border.Content = label;

                        Grid.SetRow(border, fila);
                        Grid.SetColumn(border, columna);
                        gridCalendario.Children.Add(border);

                        diaActual++;
                    }
                    else if (celda < diaSemanaInicio)
                    {
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
