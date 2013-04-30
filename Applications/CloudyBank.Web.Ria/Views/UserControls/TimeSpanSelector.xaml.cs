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
using System.ComponentModel;
using System.Windows.Data;


namespace CloudyBank.Web.Ria.UserControls
{
    public partial class TimeSpanSelector : UserControl
    {
        public int Days
        {
            get { return (int)GetValue(DaysProperty); }
            set { 
                SetValue(DaysProperty, value); 
            }
        }

        public static readonly DependencyProperty DaysProperty =
            DependencyProperty.Register("Days", typeof(int), typeof(TimeSpanSelector),null);


        public TimeSpanSelector()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(TimeSpanSelector_Loaded);
        }

        void TimeSpanSelector_Loaded(object sender, RoutedEventArgs e)
        {
            Days = 30;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var value = (sender as RadioButton).CommandParameter;
            if (value != null)
            {
                if (value.ToString() == "Infinity")
                {
                    //return maximum which TimeSpan can handle (this is later used by TimeSpan).
                    Days = TimeSpan.MaxValue.Days;
                }
                else
                {
                    Days = Convert.ToInt32(value);
                }
            }
        }
    }
}
