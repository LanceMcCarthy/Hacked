using Hacked.Core.Models;
using System.Globalization;

namespace Hacked.Maui.Converters;

internal class BreachDetailsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Breach breach)
            return "null";

        return $"Breached: {breach.BreachDate}, Accounts: {breach.PwnCount}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}