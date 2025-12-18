using System.Globalization;

namespace App_CrediVnzl.Converters
{
    public class FiltroToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Color.FromArgb("#9E9E9E");

            var filtroSeleccionado = value.ToString();
            var filtroBoton = parameter.ToString();

            if (filtroSeleccionado == filtroBoton)
            {
                return filtroBoton switch
                {
                    "Todos" => Color.FromArgb("#2196F3"),
                    "Activo" => Color.FromArgb("#4CAF50"),
                    "Completado" => Color.FromArgb("#9C27B0"),
                    _ => Color.FromArgb("#2196F3")
                };
            }

            return Color.FromArgb("#9E9E9E");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
