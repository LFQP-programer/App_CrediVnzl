using System.Globalization;

namespace App_CrediVnzl.Helpers
{
    /// <summary>
    /// Converter para transformar porcentajes (0-100) a valores de progreso (0.0-1.0)
    /// </summary>
    public class PercentageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                // Convertir de porcentaje (0-100) a decimal (0.0-1.0)
                return intValue / 100.0;
            }
            
            if (value is double doubleValue)
            {
                // Si ya es un valor entre 0 y 1, devolverlo tal cual
                if (doubleValue <= 1.0)
                    return doubleValue;
                
                // Si es un porcentaje (0-100), convertir a decimal
                return doubleValue / 100.0;
            }
            
            if (value is decimal decimalValue)
            {
                return (double)decimalValue / 100.0;
            }
            
            return 0.0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                // Convertir de decimal (0.0-1.0) a porcentaje (0-100)
                return (int)(doubleValue * 100);
            }
            
            return 0;
        }
    }
}
