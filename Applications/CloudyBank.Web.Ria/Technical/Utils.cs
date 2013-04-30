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
using CloudyBank.Web.Ria.ViewModels;
using System.Linq;

namespace CloudyBank.Web.Ria.Technical
{
    public static class Utils
    {
        public static Color ColorFromHexa(string hexaColor)
        {
            return Color.FromArgb(255,
                Convert.ToByte(hexaColor.Substring(1, 2), 16),
                Convert.ToByte(hexaColor.Substring(3, 2), 16),
                Convert.ToByte(hexaColor.Substring(5, 2), 16));
        }

    }
}
