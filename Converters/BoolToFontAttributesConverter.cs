using System.Globalization;

namespace App_CrediVnzl.Converters
{
    public class BoolToFontAttributesConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isTrue && isTrue)
            {
                return FontAttributes.Bold;
            }
            return FontAttributes.None;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
