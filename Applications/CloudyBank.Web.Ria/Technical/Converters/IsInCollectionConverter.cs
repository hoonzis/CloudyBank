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
using System.Collections;
using System.Linq;
using CloudyBank.Web.Ria.ViewModels;
using System.Collections.Generic;

namespace CloudyBank.Web.Ria.Technical
{
    /// <summary>
    /// Simple converter, returns true if object is in the collection specefied as ConverterParameter
    /// </summary>
    public class IsInCollectionConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IEnumerable collection = parameter as IEnumerable;
            foreach (var item in collection)
            {
                if (item == value)
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
