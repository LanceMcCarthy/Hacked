using System;
using Windows.UI.Xaml.Data;

namespace Hacked.Converters
{
    public class StringToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(string.IsNullOrEmpty(value.ToString()))
                return new Uri("https://bing.com");

            return new Uri($"http://{value}", UriKind.RelativeOrAbsolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}
