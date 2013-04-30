using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;

namespace CloudyBank.Web.Ria.Technical
{
    public class RadioButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter != null)
            {
                return (value.ToString() == parameter.ToString());
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType,
               object parameter, CultureInfo culture)
        {
            return (bool)value ? Enum.Parse(targetType, parameter.ToString(), true) : null;
        }
    }
}
