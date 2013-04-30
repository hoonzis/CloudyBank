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
    public partial class AccountEllipse : UserControl
    {


        public decimal MaxBalance
        {
            get { return (decimal)GetValue(MaxBalanceProperty); }
            set { SetValue(MaxBalanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxBalance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxBalanceProperty =
            DependencyProperty.Register("MaxBalance", typeof(decimal), typeof(AccountEllipse), new PropertyMetadata(MaxBalanceChanged));

        public static void MaxBalanceChanged(DependencyObject d,DependencyPropertyChangedEventArgs e)
        {
            var ellipse = d as AccountEllipse;
            ellipse.ActualizeDimensions();
        }

        public decimal Balance
        {
            get { return (decimal)GetValue(BalanceProperty); }
            set { SetValue(BalanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Balance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BalanceProperty =
            DependencyProperty.Register("Balance", typeof(decimal), typeof(AccountEllipse), new PropertyMetadata(BalanceChanged));

        public AccountEllipse()
        {
            InitializeComponent();
        }

        public static void BalanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ellipse = d as AccountEllipse;
            ellipse.ActualizeDimensions();
            ellipse.BalanceText.Text = String.Format("{0:n2}", e.NewValue);
        }

        public void ActualizeDimensions()
        {
            var absBalance = Math.Abs(Balance);
            //minimal width will be 100 - if not it would get too small
            InnerEllipse.Width = Math.Max(100, (double)Map(absBalance, 0, MaxBalance, 1, 300));
            InnerEllipse.Height = InnerEllipse.Width;
        }

        //private static int MapValue(decimal value, decimal n)
        //{
            
        //    if (n == 0) { return 0; }
        //    var absValue = Math.Abs(value);

        //    int output = (int)Math.Round((100.0 / (double)(n - 1) * (double)(absValue - 1)) - 0.5, 0);
        //    if (output == -1) return 0;
        //    else return output;
        //}

        //Normalize value from range to 0 - 1
        public static double Normalize(decimal value, decimal minimum, decimal maximum)
        {
            if (maximum == 0 && minimum == 0) return 0;
            return (double)((value - minimum) / (maximum - minimum));
        }

        /// <summary>
        /// Linear interpolation - take value from 0 to 1 and map it on specified range
        /// </summary>
        /// <param name="normValue"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static decimal Interpolate(double normValue, decimal minimum, decimal maximum)
        {
            return minimum + (decimal)((double)(maximum - minimum) * normValue);
        }

        /// <summary>
        /// Map from one range to another
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min1"></param>
        /// <param name="max1"></param>
        /// <param name="min2"></param>
        /// <param name="max2"></param>
        /// <returns></returns>
        public static decimal Map(decimal value, decimal min1, decimal max1, decimal min2, decimal max2)
        {
            return Interpolate(Normalize(value, min1, max1), min2, max2);
        }
    }
}
