using System;
using System.Globalization;
using Hacked.Core.Models;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Converters
{
    internal class BreachTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var breach = value as Breach;

            if (breach == null)
                return "null";

            return breach.IsNew ? $"{breach.Name} (NEW)" : breach.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
