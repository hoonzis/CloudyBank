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
    public partial class EditPaymentEventControl : UserControl
    {
        public event EventHandler SaveEvent;
        public event EventHandler RemoveEvent;

        public EditPaymentEventControl()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveEvent != null)
            {
                SaveEvent(this, new EventArgs());
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (RemoveEvent != null)
            {
                RemoveEvent(this, new EventArgs());
            }
        }

    }
}
