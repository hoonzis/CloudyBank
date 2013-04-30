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
using System.Diagnostics;
using CloudyBank.PortableServices.Users;


namespace CloudyBank.Mobile.Pages
{
    public partial class AuthenticationPage : PhoneApplicationPage
    {
        WCFUserService _userService;

        public WCFUserService UserService
        {
            get {
                if (_userService == null)
                {
                    _userService = ServicesFactory.GetObject<WCFUserService>();
                }
                return _userService; 
            }
            set { _userService = value; }
        }

        CookieContainer _container;
        public AuthenticationPage()
        {
            InitializeComponent();
            _container = new CookieContainer();
            
            ServicesFactory.Creator = new SimpleCreator(true,_container);

        }

        void EndLoginGetID(IAsyncResult result)
        {

            var custID = UserService.EndLoginGetID(result);
            CustomerViewModel vm = new CustomerViewModel(custID);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/Pages/CustomerPage.xaml", UriKind.Relative));
                (App.Current.RootVisual as PhoneApplicationFrame).DataContext = vm;
            });
        }

       
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
           UserService.BeginLoginGetID(ClientNumberBox.Password, PasswordBox.Password, EndLoginGetID, null);
        }

        
    }
}
