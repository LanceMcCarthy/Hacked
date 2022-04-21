using Hacked.Core.Models;
using System.Globalization;

namespace Hacked.Maui.Converters;

internal class AccountDetailsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not MonitoredAccount account)
            return "account is null";

        var newCount = account.Breaches.Where(a => a.IsNew).ToList().Count;

        return $"Breaches: {account.Breaches.Count}, New: {newCount}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}