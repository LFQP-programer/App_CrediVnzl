using System.Globalization;

namespace App_CrediVnzl.Converters
{
    public class IsNotNullOrEmptyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            if (value is string stringValue)
                return !string.IsNullOrWhiteSpace(stringValue);

            return true;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
