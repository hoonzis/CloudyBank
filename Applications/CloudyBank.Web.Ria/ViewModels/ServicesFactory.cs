using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.ServiceModel.Channels;
using CloudyBank.PortableServices.CustomerProfiles;
using CloudyBank.PortableServices.Partners;
using CloudyBank.PortableServices.PaymentEvents;
using CloudyBank.PortableServices.TagDepenses;
using CloudyBank.PortableServices.Advisors;
using CloudyBank.PortableServices.Agencies;
using CloudyBank.PortableServices.Customers;
using CloudyBank.PortableServices.Accounts;
using CloudyBank.PortableServices.Tags;
using CloudyBank.PortableServices.Operations;
using CloudyBank.PortableServices.Images;
using System.Collections.Generic;
using CloudyBank.PortableServices.Users;
using CloudyBank.PortableServices.OAuthTokens;

namespace CloudyBank.Web.Ria.ViewModels
{
    public static class ServicesFactory
    {
        public static ICreator Creator { get; set; }

        public static T GetObject<T>() where T: class {
           
            return Creator.GetObject<T>();
        }

    }

    public interface ICreator
    {
        T GetObject<T>() where T : class;
    }

    public class ServiceForFixturesCreator : ICreator
    {
        public T GetObject<T> () where T: class
        {
            return default(T);
        }
    }

    public class SimpleCreator : ICreator
    {
        private Dictionary<Type, Type> _registeredServices = new Dictionary<Type, Type>();

        public void Register(Type iface, Type implementation){
            _registeredServices.Add(iface,implementation);
        }

        public T GetObject<T>() where T : class
        {
            var implem = _registeredServices[typeof(T)];
            var instance = Activator.CreateInstance(implem);

            var client = instance as ClientBase<T>;
            if (_manageCookies)
            {
                var cookieManager = client.InnerChannel.GetProperty<IHttpCookieContainerManager>();

                if (cookieManager != null && cookieManager.CookieContainer == null)
                {
                    cookieManager.CookieContainer = Container;
                }
            }
            return client as T;
        }

        public bool _manageCookies;
        public SimpleCreator(bool manageCookies, CookieContainer container)
        {
            _manageCookies = manageCookies;
            Container = container;
            Register(typeof(WCFUserService), typeof(WCFUserServiceClient));
            Register(typeof(WCFAccountService), typeof(WCFAccountServiceClient));
            Register(typeof(WCFCustomersService), typeof(WCFCustomersServiceClient));
            Register(typeof(WCFOperationService), typeof(WCFOperationServiceClient));
            Register(typeof(WCFCustomerProfileService), typeof(WCFCustomerProfileServiceClient));
            Register(typeof(WCFTagService), typeof(WCFTagServiceClient));
            Register(typeof(WCFTagDepensesService), typeof(WCFTagDepensesServiceClient));
            Register(typeof(WCFPaymentEventService), typeof(WCFPaymentEventServiceClient));
            Register(typeof(WCFAgencyService), typeof(WCFAgencyServiceClient));
            Register(typeof(WCFImageService), typeof(WCFImageServiceClient));
            Register(typeof(WCFPartnerService), typeof(WCFPartnerServiceClient));
            Register(typeof(WCFOAuthManagementService), typeof(WCFOAuthManagementServiceClient));
            
        }
        public static CookieContainer Container { get; set; } 
    }
}
