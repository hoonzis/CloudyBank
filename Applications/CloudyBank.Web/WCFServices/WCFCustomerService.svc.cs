using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CloudyBank.Services;
using CloudyBank.Dto;
using System.Collections.Generic;
using Common.Logging;
using System.Web;
using CloudyBank.Core.Services;
using System.Web.Security;
using System.Security.Permissions;
using System.Threading;
using System.Security;
using System.ServiceModel.Web;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "octo.customers.service", Name = "WCFCustomersService")]
    [ServiceBehavior(Namespace = "octo.customers.port", Name = "WCFCustomersPort")]
    //[SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFCustomerService
    {
        public WCFCustomerService()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        private ICustomerServices _customerServices;
        public ICustomerServices CustomerServices
        {
            get {
                if (_customerServices == null)
                {
                    //_customerServices = Global.GetObject<CustomerServices>("CustomerServices");
                    _customerServices = Global.GetObject<ICustomerServices>("AdvicedCustomerService");
                }
                return _customerServices; 

            }
            set { _customerServices = value; }
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void SaveCustomerDto(CustomerDto customerDto)
        {
            CustomerServices.SaveCustomer(customerDto);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public IList<CustomerDto> GetCustomersForAdvisor(int advisorID)
        {
            return CustomerServices.GetCustomersForAdvisor(advisorID);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated=true)]
        public CustomerDto GetCurrentCustomer()
        {
            FormsIdentity identity = (HttpContext.Current.User.Identity as FormsIdentity);
            CustomerDto customer = CustomerServices.GetCustomerByIdentity(HttpContext.Current.User.Identity.Name);
            return customer;   
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="/customers?id={id}")]
        public CustomerDto GetCustomerByID(int id)
        {
            var customer = CustomerServices.GetCustomerById(id);
            return customer;
        }
    }
}
