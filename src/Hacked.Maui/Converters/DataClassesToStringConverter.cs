using System.Globalization;

namespace Hacked.Maui.Converters;

internal class DataClassesToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string[] classes || classes.Length == 0)
            return "none";

        return string.Join(", ", classes);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }
}