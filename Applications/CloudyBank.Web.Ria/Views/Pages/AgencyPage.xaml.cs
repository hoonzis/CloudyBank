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
using CloudyBank.Web.Ria.ViewModels;
using CloudyBank.Web.Ria.Technical;
using CloudyBank.Web.Ria.UserControls;

namespace CloudyBank.Web.Ria.Views
{
    public partial class AgencyPage : Page
    {
        public AgencyPage()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(NavigationContext.QueryString.ContainsKey("View")){
                
                AgencyNavigationFrame.Navigate(new Uri("/" + NavigationContext.QueryString["View"], UriKind.Relative));
            }
        }
    }
}
