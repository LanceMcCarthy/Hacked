using System;
using Windows.UI.Xaml.Data;

namespace Hacked.Converters;

public class StringToUriConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";

            return new Uri($"http://{value}", UriKind.RelativeOrAbsolute);
        }

        return new Uri("https://dvlup.com");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return new NotImplementedException();
    }
}
