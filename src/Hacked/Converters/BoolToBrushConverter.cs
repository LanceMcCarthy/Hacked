using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Hacked.Converters;

internal class BoolToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var altForegroundBrush = Application.Current.Resources["SystemControlBackgroundBaseHighBrush"] as SolidColorBrush;

        if (altForegroundBrush == null)
            altForegroundBrush = new SolidColorBrush(Colors.Gold);

        return (bool)value
            ? new SolidColorBrush(Colors.Red)
            : altForegroundBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
