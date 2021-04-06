using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Hacked.Wpf.Converters
{
    public class ToggleButtonForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(Colors.White);
            }

            return (bool)value ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
