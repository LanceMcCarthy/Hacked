using System;
using System.Globalization;
using System.Windows.Data;

namespace Hacked.Wpf.Converters
{
    public class StringToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string url && !string.IsNullOrEmpty(url))
            {
                return new Uri($"http://{value}", UriKind.RelativeOrAbsolute);
            }

            return new Uri("https://bing.com");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
