using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CloudyBank.CoreDomain.Bank;
using System.Collections.Generic;
using CloudyBank.Core.Services;
using CloudyBank.Dto;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.ServiceModel.Web;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "octo.events.service", Name="WCFPaymentEventService")]
    [ServiceBehavior(Namespace = "octo.events.port", Name = "WCFPaymentEventPort")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFPaymentEventService
    {
        public WCFPaymentEventService()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        private IPaymentEventServices _service;
        private IPaymentEventServices PaymentEventService
        {
            get
            {
                if (_service == null)
                {
                    _service = Global.GetObject<IPaymentEventServices>("PaymentEventServices");
                }
                return _service;
            }
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="customer?id={customerID}")]
        public List<PaymentEventDto> GetPaymentEventsForCustomer(int customerID)
        {
            return PaymentEventService.GetPaymentEventsForCustomer(customerID);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.Wrapped)]
        public int CreatePaymentEvent(PaymentEventDto paymentDto, int customerId)
        {
            return PaymentEventService.CreatePaymentEvent(paymentDto, customerId);
        }

        
        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="events?id={eventId}")]
        public PaymentEventDto GetPaymentEventById(int eventId)
        {
            return PaymentEventService.GetPaymentEventById(eventId);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="removeEvent?id={paymentId}")]
        public bool RemovePaymentEvent(int paymentId)
        {            
            return PaymentEventService.RemovePayment(paymentId);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.Wrapped)]
        public bool UpdatePaymentEvent(PaymentEventDto paymentDto)
        {
            return PaymentEventService.UpdatePaymentEvent(paymentDto);
        }
    }
}
