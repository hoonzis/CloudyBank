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
using Microsoft.Phone.Controls;
using CloudyBank.Web.Ria.ViewModels;

namespace CloudyBank.Mobile.Pages
{
    public partial class CustomerPage : PhoneApplicationPage
    {
        public CustomerPage()
        {
            InitializeComponent();
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            var customer = LayoutRoot.DataContext as CustomerViewModel;
            var account = (sender as Button).DataContext as AccountViewModel;
            customer.SelectedAccount = account;
            NavigationService.Navigate(new Uri("/Pages/AccountPage.xaml", UriKind.Relative));
        }

        private void Transactions_Click(object sender, RoutedEventArgs e)
        {
            var customer = LayoutRoot.DataContext as CustomerViewModel;
            var account = (sender as Button).DataContext as AccountViewModel;
            customer.SelectedAccount = account;
            NavigationService.Navigate(new Uri("/Pages/OperationsPage.xaml", UriKind.Relative));
        }
    }
}
