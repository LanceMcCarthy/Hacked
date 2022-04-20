using System;
using System.Globalization;
using Hacked.Core.Models;
using Microsoft.Maui.Controls;

namespace Hacked.Maui.Converters;

internal class BreachDetailsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var breach = value as Breach;

        if (breach == null)
            return "null";
            
        return $"Breached: {breach.BreachDate}, Accounts: {breach.PwnCount}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}