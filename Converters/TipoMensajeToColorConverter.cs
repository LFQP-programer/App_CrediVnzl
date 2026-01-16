using System.Globalization;

namespace App_CrediVnzl.Converters
{
    public class TipoMensajeToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string tipoActual && parameter is string tipoBoton)
            {
                return tipoActual == tipoBoton 
                    ? Color.FromArgb("#2196F3") 
                    : Color.FromArgb("#E0E0E0");
            }

            return Color.FromArgb("#E0E0E0");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
