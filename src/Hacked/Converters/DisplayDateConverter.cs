using System;
using Windows.UI.Xaml.Data;

namespace Hacked.Converters
{
    public class DisplayDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;

            //TODO fix DateTime parse
            DateTime date;
            return DateTime.TryParse((string)value, out date) ? date.ToString("g") : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
