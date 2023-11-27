using System;
using Windows.UI.Xaml.Data;

namespace Hacked.Converters;

public class DisplayDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        try
        {
            switch (value)
            {
                case string dateString:
                    DateTime.TryParse(dateString, out var outDate);

                    return outDate.ToString("d");
                case DateTime dDate:
                    return dDate.ToString("d");
                default:
                    try
                    {
                        return ((DateTime?)value).Value.ToString("d");
                    }
                    catch
                    {
                        // ignored
                    }

                    break;
            }
        }
        catch
        {
            // ignored
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
