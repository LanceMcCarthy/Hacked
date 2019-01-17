using System;
using System.Globalization;
using Hacked.Core.Models;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Converters
{
    internal class LogoSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var breach = value as Breach;

            if (breach == null)
                return null;

            //if (breach.LogoType == "svg")
            //{
            //    return $"Images/{breach.Title}.png";
            //}

            return $"https://az594751.vo.msecnd.net/cdn/{breach.Name}.{breach.LogoType}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
