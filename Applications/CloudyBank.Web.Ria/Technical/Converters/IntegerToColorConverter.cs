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

namespace CloudyBank.Web.Ria.Technical
{
    public class IntegerToColorConverter : IValueConverter
    {

        private Color[] _colors = { Colors.Red, Colors.Green};
        //Utils.ColorFromHexa("AA0000")

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush(_colors[(int)value % _colors.Length]);

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}
