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
using System.Windows.Navigation;
using System.Linq.Expressions;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Browser;
using CloudyBank.Web.Ria.ViewModels;
using CloudyBank.Web.Ria.Views;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using CloudyBank.PortableServices.Users;

namespace CloudyBank.Web.Ria
{
    public partial class MainPage : UserControl
    {
        #region Services
        WCFUserService _userService;

        public WCFUserService UserService
        {
            get {
                if (_userService == null)
                {
                    _userService = new WCFUserServiceClient();
                }
                return _userService;
            }
            set { _userService = value; }
        }

        #endregion

        public MainPage()
        {
            InitializeComponent();
            ServicesFactory.Creator = new SimpleCreator(false, null);

            DataContext = this;
            GetCurrentUser();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            UserService.BeginLogout(EndLogout,null); 
        }

        void GetCurrentUser()
        {
            UserService.BeginGetCurrentUser(EndGetCurrentUser, null);
        }

        void EndLogout(IAsyncResult e)
        {
            OpenLoginPage();
        }

        public void OpenLoginPage()
        {
            Dispatcher.BeginInvoke(() =>
            {
                var login = new LoginPage();
                login.LogedIn += new EventHandler(login_LogedIn);

                MainContent.Content = login;
                LogoutButton.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        void login_LogedIn(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var login = MainContent.Content as LoginPage;
                login.LogedIn -= login_LogedIn;
                GetCurrentUser();
            });
        }
        
        void EndGetCurrentUser(IAsyncResult e)
        {
            UserIdentityDto userIdentity = UserService.EndGetCurrentUser(e);
            
            //there is a logged user
            if (userIdentity != null)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    //load the referential data
                    ReferentialDataViewModel vm = App.Current.Resources["Referential"] as ReferentialDataViewModel;
                    vm.LoadData();
                    LogoutButton.Visibility = System.Windows.Visibility.Visible;
                    CustomerHome customer = new CustomerHome();
                    customer.DataContext = new CustomerViewModel(userIdentity.Id);
                    MainContent.Content = customer;

                });            
            }
            else
            {
                OpenLoginPage();
            }
            
        }
    }
}
