using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Hacked.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public bool IsInverted { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (IsInverted)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }
        else
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
