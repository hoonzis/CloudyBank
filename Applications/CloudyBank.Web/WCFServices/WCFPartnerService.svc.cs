using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CloudyBank.Dto;
using System.Collections.Generic;
using CloudyBank.Core.Services;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.ServiceModel.Web;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "octo.partners.service", Name="WCFPartnerService")]
    [ServiceBehavior(Namespace = "octo.partners.port", Name = "WCFPartnerPort")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFPartnerService
    {
        public WCFPartnerService()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        private IBusinessPartnerServices _partnerServices;

        public IBusinessPartnerServices PartnerServices
        {
            get {
                if (_partnerServices == null)
                {
                    _partnerServices = Global.GetObject<IBusinessPartnerServices>("BusinessPartnerServices");
                }
                return _partnerServices;
            }
        }


        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="customer/partners?id={customerID}")]
        public List<BusinessPartnerDto> GetPartnersForCustomer(int customerID)
        {
            return PartnerServices.GetBusinessPartnerForCustomer(customerID);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebInvoke(UriTemplate="createPartner",BodyStyle=WebMessageBodyStyle.Wrapped)]
        public int CreatePartner(BusinessPartnerDto partnerDto, int customerId)
        {
            return PartnerServices.CreateBusinessPartner(partnerDto,customerId);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebInvoke(UriTemplate = "updatePartner", BodyStyle = WebMessageBodyStyle.Wrapped)]
        public bool UpdatePartner(BusinessPartnerDto partner, int customerId){
            return PartnerServices.UpdateBusinessPartner(partner,customerId);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="removePartner?id={partnerId}&customer={customerId}", BodyStyle=WebMessageBodyStyle.Bare)]
        public bool RemovePartner(int partnerId, int customerId)
        {
            return PartnerServices.RemoveBusinessPartner(partnerId, customerId);
        }

    }
}
