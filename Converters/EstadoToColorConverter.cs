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
                    "Pagado" => Color.FromArgb("#4CAF50"),
                    "Pendiente" => Color.FromArgb("#FFC107"),
                    "Vencido" => Color.FromArgb("#F44336"),
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
