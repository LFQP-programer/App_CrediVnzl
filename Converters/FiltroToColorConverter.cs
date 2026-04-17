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
                    "Todos" => Color.FromArgb("#003B7A"), // Azul Tertiary
                    "Activo" => Color.FromArgb("#FDB913"), // Amarillo Primary
                    "Completado" => Color.FromArgb("#E4002B"), // Rojo Secondary
                    _ => Color.FromArgb("#003B7A")
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
