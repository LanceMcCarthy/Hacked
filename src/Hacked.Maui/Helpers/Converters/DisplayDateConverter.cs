using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Hacked.Maui.Helpers.Converters;

internal class DisplayDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;

        //TODO fix DateTime parse
        //DateTime date;
        //return DateTime.TryParse((string) value, out date) ? date.ToString("g") : value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}