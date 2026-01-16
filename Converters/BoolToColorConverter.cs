using System.Globalization;

namespace App_CrediVnzl.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Si se proporciona un par�metro, usar los colores personalizados
                if (parameter is string colors)
                {
                    var colorArray = colors.Split('|');
                    if (colorArray.Length == 2)
                    {
                        return boolValue ? Color.FromArgb(colorArray[0]) : Color.FromArgb(colorArray[1]);
                    }
                }
                
                // Colores por defecto
                return boolValue ? Color.FromArgb("#2196F3") : Color.FromArgb("#E0E0E0");
            }
            return Color.FromArgb("#E0E0E0");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class BoolToTextConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string texts)
            {
                var textArray = texts.Split('|');
                if (textArray.Length == 2)
                {
                    return boolValue ? textArray[0] : textArray[1];
                }
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
