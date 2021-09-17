using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Hacked.Maui.Helpers.Converters
{
    internal class DataClassesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "none";

            var classes = (string[]) value;

            if (classes.Length == 0)
                return "none";

            string classList = "";

            foreach (var item in classes)
            {
                var delimiter = string.IsNullOrEmpty(classList) ? "" : ", ";
                classList = classList + delimiter + item;
            }

            return classList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
