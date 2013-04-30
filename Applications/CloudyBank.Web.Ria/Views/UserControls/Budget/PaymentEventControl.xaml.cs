using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CloudyBank.Web.Ria.UserControls
{
    public partial class PaymentEventControl : UserControl
    {
        public event EventHandler PayEvent;
        public event EventHandler EditEvent;

        public PaymentEventControl()
        {
            InitializeComponent();
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (PayEvent != null)
            {
                PayEvent(this, null);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditEvent != null)
            {
                EditEvent(this, null);
            }
        }
    }
}
