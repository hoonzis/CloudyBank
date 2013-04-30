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
using System.Collections.Generic;
using System.Linq;

namespace CloudyBank.Web.Ria.ViewModels
{
    public class VMUtils
    {
        //to depenses only negative payments are counted (that's why Amount<0)
        //depenses are shown as positive values (Math.abs(sum))
        public static Dictionary<String, double> GetTagsRepartition(IEnumerable<OperationViewModel> operations, DateTime compareDate)
        {
            var data = operations
                .Where(x => !String.IsNullOrWhiteSpace(x.TagName))
                .Where(x => x.Amount < 0)
                .Where(x => x.Date.CompareTo(compareDate) > 0)
                .GroupBy(x => x.TagName)
                .ToDictionary(x => x.Key, x => Math.Abs(x.Sum(y => (double)y.Amount)));

            return data;
        }
    }
}
