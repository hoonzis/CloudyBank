using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CloudyBank.CoreDomain;
using CloudyBank.Core.Services;
using System.Web;
using CloudyBank.Dto;
using System.Threading;
using System.Web.Security;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel.Web;
using CloudyBank.Core.DataAccess;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "octo.users.service",Name="WCFUserService")]
    [ServiceBehavior(Namespace = "octo.users.port", Name = "WCFUserPort")]
    //[SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFUserService
    {
        public WCFUserService()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        private IUserServices _userService;

        public IUserServices UserService
        {
            get {
                if (_userService == null)
                {
                    _userService = Global.GetObject<IUserServices>("UserServices");
                }
                return _userService; }
            set { 
                _userService = value;
            }
        }

        private ICustomerServices _customerServices;

        public ICustomerServices CustomerServices
        {
            get {
                if (_customerServices == null)
                {
                    _customerServices = Global.GetObject<ICustomerServices>("CustomerServices");
                }
                return _customerServices; }
            set { _customerServices = value; }
        }


        [OperationContract]
        public UserIdentityDto GetCurrentUser()
        {
            //if user is logged in, return the user
            //if not return null
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return UserService.GetUserByIdentity(HttpContext.Current.User.Identity.Name);
            }
            else
            {
                return null;
            }
        }

        
        [OperationContract]
        [WebGet(BodyStyle=WebMessageBodyStyle.WrappedRequest)]
        public String LoginCookie(String login, String password)
        {
            var user = UserService.AuthenticateUser(login, password);
            if (user != null)
            {
                
                /* This is the insid of SetAuthCookie
                FormsAuthenticationTicket ticket = new
                        FormsAuthenticationTicket(1, login, DateTime.Now, DateTime.Now.AddMinutes(30), false, login);
                
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                HttpCookie cookie =
                   new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                //HttpContext.Current.Response.Cookies.Add(cookie);
                //Response.Cookies.Add(cookie);

                FormsIdentity id = new FormsIdentity(ticket);
                
                System.Security.Principal.GenericPrincipal principal = new System.Security.Principal.GenericPrincipal(id, ticket.UserData.Split(new char[] { '|' }));

                HttpContext.Current.User = principal;
                */
                
                var cookie = FormsAuthentication.GetAuthCookie(login, false);
                return cookie.Value; // this is equivalent to returning encryptedTicket
            }
            return null;
        }

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public bool Login(String login, String password)
        {
            var user = UserService.AuthenticateUser(login, password);
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(login, false);
                return true;
            }
            return false;
        }

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public int LoginGetID(String login, String password)
        {
            var user = UserService.AuthenticateUser(login, password);
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(login, false);
                return user.Id;
            }
            return -1;
        }
        
        [OperationContract]
        [WebInvoke(UriTemplate = "Logout")]
        public void Logout()
        {
            FormsAuthentication.SignOut();
        }
    }
}
