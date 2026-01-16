using System.Globalization;

namespace App_CrediVnzl.Converters
{
    public class EstadoToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string estado)
            {
                return estado switch
                {
                    "Activo" => Color.FromArgb("#4CAF50"), // Verde Success
                    "Completado" => Color.FromArgb("#003B7A"), // Azul Tertiary
                    "Cancelado" => Color.FromArgb("#E4002B"), // Rojo Secondary
                    "Pagado" => Color.FromArgb("#4CAF50"), // Verde Success
                    "Pendiente" => Color.FromArgb("#FDB913"), // Amarillo Primary
                    "Vencido" => Color.FromArgb("#E4002B"), // Rojo Secondary
                    _ => Color.FromArgb("#9E9E9E")
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
