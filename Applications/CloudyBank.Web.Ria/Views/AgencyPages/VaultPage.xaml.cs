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
using System.IO;
using CloudyBank.Web.Ria.ViewModels;

namespace CloudyBank.Web.Ria.UserControls
{
    public partial class VaultPage : Page
    {
        public VaultPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (DataContext!=null && DataContext is CustomerViewModel)
            {
                var customer = DataContext as CustomerViewModel;
                if (customer.Customer != null)
                {
                    VaultViewModel vm = new VaultViewModel(customer.Customer.Id);
                    DataContext = vm;
                }
            }
        }
    }
}
