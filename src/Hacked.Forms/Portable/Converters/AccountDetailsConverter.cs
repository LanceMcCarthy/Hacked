using System;
using System.Globalization;
using System.Linq;
using Hacked.Core.Models;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Converters
{
    internal class AccountDetailsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is MonitoredAccount account))
                return "account is null";

            var newCount = account.Breaches.Where(a => a.IsNew).ToList().Count;

            return $"Breaches: {account.Breaches.Count}, New: {newCount}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
