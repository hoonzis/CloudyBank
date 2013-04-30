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
using System.Windows.Navigation;
using CloudyBank.Web.Ria.Components;
using CloudyBank.Web.Ria.Views.Windows;
using CloudyBank.Web.Ria.Technical;
using CloudyBank.Web.Ria.ViewModels;
using System.Windows.Controls.Primitives;
using CloudyBank.Web.Ria.UserControls;

namespace CloudyBank.Web.Ria.Views
{
    public partial class BudgetPage : Page
    {
        public BudgetPage()
        {
            InitializeComponent();
        }

        private void EventCalendar_EventClick(object sender, CalendarEventArgs e)
        {
            //transfer the data context
            PaymentDetailsBorder.DataContext = (sender as Button).DataContext;
            PositionPaymentBorder(sender);
            InfoPaymentCtrl.Visibility = System.Windows.Visibility.Visible;
            EditPaymentCtrl.Visibility = System.Windows.Visibility.Collapsed;
        }

        
        /// <summary>
        /// This positionates the Payment border element on the place of the button which was clicked.
        /// Canvas is used in order to have a coordinate system. Canvas overlay would eat all mouse clikckcs,
        /// so it is collapsed and show just in time for positioning the element.
        /// </summary>
        /// <param name="sender"></param>
        private void PositionPaymentBorder(object sender)
        {
            Button element = sender as Button;
            GeneralTransform transform = element.TransformToVisual(CoordinateCanvas);// Application.Current.RootVisual as UIElement);
            Point offset = transform.Transform(new Point(0, 0));

            CoordinateCanvas.Visibility = System.Windows.Visibility.Visible;
            PaymentDetailsBorder.Visibility = System.Windows.Visibility.Visible;
            
            
            if (offset.X  + PaymentDetailsBorder.Width > this.ActualWidth)
            {
                offset.X = offset.X + PaymentDetailsBorder.Width - this.ActualWidth;
            }
            Canvas.SetTop(PaymentDetailsBorder, offset.Y);
            Canvas.SetLeft(PaymentDetailsBorder, offset.X);
        }

        private void EventCalendar_DayClick(object sender, CalendarEventArgs e)
        {
            //ShowPaymentRelativeToButton(sender);
            PaymentEventViewModel payment = new PaymentEventViewModel();
            payment.Date = e.Date;
            PaymentDetailsBorder.DataContext = payment;
            PositionPaymentBorder(sender);
            InfoPaymentCtrl.Visibility = System.Windows.Visibility.Collapsed;
            EditPaymentCtrl.Visibility = System.Windows.Visibility.Visible;
        }

        
        /// <summary>
        /// When each account checkbox is loaded, than it is checked wheather the account is present in the SelectedAccounts collection
        /// this is workaround in order not to have to add "IsChecked" value to the AccountViewModel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            AccountViewModel vm = checkbox.DataContext as AccountViewModel;
            CustomerViewModel customerVM = Customer.Data as CustomerViewModel;
            if (customerVM.SelectedAccounts.Contains(vm))
            {
                customerVM.SelectedAccounts.Remove(vm);
                checkbox.IsChecked = true;
            }
        }

        private void EditPayment_Click(object sender, EventArgs e)
        {

            InfoPaymentCtrl.Visibility = System.Windows.Visibility.Collapsed;
            EditPaymentCtrl.Visibility = System.Windows.Visibility.Visible;
        }

        private void ClosePaymentButton_Click(object sender, RoutedEventArgs e)
        {
            PaymentDetailsBorder.Visibility = System.Windows.Visibility.Collapsed;
            CoordinateCanvas.Visibility = System.Windows.Visibility.Collapsed;
        }


        #region Future aim - pay directly from calendar
        //private void PayPayment_Click(object sender, EventArgs e)
        //{
        //    var payment = (sender as PaymentEventControl).DataContext as PaymentEventViewModel;
        //    NavigationService.Navigate(new Uri(String.Format("/Agency?View={0}&Payment={1}", BankAction.Payment, payment.Id), UriKind.Relative));
        //}
        #endregion

        private void CoordinateCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CoordinateCanvas.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
