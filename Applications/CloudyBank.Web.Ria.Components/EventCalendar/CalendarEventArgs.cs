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

namespace CloudyBank.Web.Ria.Components
{
    public class CalendarEventArgs : EventArgs
    {
        public Object CalendarEvent { get; set; }
        public DateTime Date { get; set; }

        public CalendarEventArgs(object eventBehind)
        {
            CalendarEvent = eventBehind;
        }

        public CalendarEventArgs(DateTime date)
        {
            Date = date;
        }
    }
}
