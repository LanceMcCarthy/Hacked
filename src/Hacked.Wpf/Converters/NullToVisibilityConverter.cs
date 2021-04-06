using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hacked.Wpf.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; }
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
