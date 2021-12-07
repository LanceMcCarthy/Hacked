using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Hacked.Maui.Helpers.Converters;

internal class InvertBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool?) value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}