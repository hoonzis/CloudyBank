using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Spring.Context.Support;
using Spring.Context;
using NHibernate;
using NHibernate.Context;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Diagnostics;
using System.Security;
using Common.Logging;
using CloudyBank.Web.Security.OAuth;
using DotNetOpenAuth.OAuth.Messages;
using CloudyBank.CoreDomain.Security;
using CloudyBank.Core.DataAccess;
using log4net.Config;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.Services.Strings;


namespace CloudyBank.Web
{
    public class Global : System.Web.HttpApplication
    {
        private static IApplicationContext _applicationContext;
        private static ISessionFactory _sessionFactory;
        
        private ILog _log = LogManager.GetLogger(typeof(Global));

        private static IUserRepository _userRepository;
        public static IUserRepository UserRepository
        {
            get {
                if (_userRepository == null)
                {
                    _userRepository = GetObject<IUserRepository>("UserRepository");
                }
                return _userRepository;
            }
        }

        private static ICustomerRepository _customerRepository;
        private static ICustomerRepository CustomerRepository
        {

            get
            {
                if (_customerRepository == null)
                {
                    _customerRepository = GetObject<ICustomerRepository>("CustomerRepository");
                }
                return _customerRepository;
            }
        }

        public static Customer LoggedInCustomer
        {
            get
            {
                return CustomerRepository.GetCustomerByIdentity(HttpContext.Current.User.Identity.Name);
            }
        }

        #region OAuth
        private static Dictionary<String, String> _scopeNames;

        public static Dictionary<String, String> ScopeNames
        {
            get {
                if (_scopeNames == null)
                {
                    _scopeNames = new Dictionary<string, string>();
                    _scopeNames.Add("GetAccounts", StringResources.AccountsScope);
                    _scopeNames.Add("GetOperations", StringResources.OperationsScope);
                }
                return Global._scopeNames; 
            }
            set { Global._scopeNames = value; }
        }

        private static DatabaseNonceStore _nonceStore;
        public static DatabaseNonceStore NonceStore
        {
            get
            {
                if (_nonceStore == null)
                {
                    _nonceStore = new DatabaseNonceStore();
                }
                return _nonceStore;
            }
        }


        private static DatabaseTokenManager _tokenManager;
        public static DatabaseTokenManager TokenManager {
            get
            {
                if (_tokenManager == null)
                {
                    _tokenManager = new DatabaseTokenManager();
                }
                return _tokenManager;
            }
        }

        public static UserAuthorizationRequest PendingOAuthAuthorization
        {
            get { return HttpContext.Current.Session["authrequest"] as UserAuthorizationRequest; }
            set { HttpContext.Current.Session["authrequest"] = value; }
        }

        public static void AuthorizePendingRequestToken()
        {
            ITokenContainingMessage tokenMessage = PendingOAuthAuthorization;
            TokenManager.AuthorizeRequestToken(tokenMessage.Token, LoggedInCustomer);
            PendingOAuthAuthorization = null;
        }

        #endregion

        public static T GetObject<T>(string id)
        {
            return (T)_applicationContext.GetObject(id);
        }

        public static T GetObject<T>()
        {
            return (T)_applicationContext.GetObject(typeof(T).Name);
        }

        public static void InitSessionFactory()
        {
            _applicationContext = ContextRegistry.GetContext();
            _sessionFactory = GetObject<ISessionFactory>("SessionFactory");
        }

        public static void InitCloud()
        {
            if (RoleEnvironment.IsAvailable)
            {

                //FromConfigurationSetting method executes the delegate passed to SetConfigurationSettingPublisher
                //call SetConfigurationSettingPublisher, passing in the logic to get connection string data from your custom source.
                CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                {
                    var connectionString = RoleEnvironment.GetConfigurationSettingValue(configName);
                    configSetter(connectionString); //configSetter is the delegate
                });
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            InitSessionFactory();
            InitCloud();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            CurrentSessionContext.Bind(_sessionFactory.OpenSession());
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            ISession session = CurrentSessionContext.Unbind(_sessionFactory);
            
            if (session != null)
            {
                session.Close();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var error = HttpContext.Current.Error;
            if (error.GetType() == typeof(SecurityException))
            {
                _log.Error("Security Exception", error);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

    }
}
