using System;
using Windows.UI.Xaml.Data;
using Hacked.Core.Models;

namespace Hacked.Converters
{
    public class LogoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var breach = value as Breach;
            
            if (breach == null)
                return "Images/ListFallBackIcon.jpg";

            //if (breach.LogoType == "svg")
            //{
            //    return $"Images/{breach.Title}.png";
            //}

            return $"https://az594751.vo.msecnd.net/cdn/{breach.Name}.{breach.LogoType}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
