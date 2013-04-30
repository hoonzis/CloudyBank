using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CloudyBank.Core.Services;
using System.Collections.Generic;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Services;
using CloudyBank.Dto;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.ServiceModel.Web;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "octo.tags.service", Name="WCFTagService")]
    [ServiceBehavior(Namespace = "octo.tags.port", Name = "WCFTagPort")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFTagService
    {
        public WCFTagService()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        private ITagServices _tagServices;

        public ITagServices TagServices
        {
            get {
                if (_tagServices == null)
                {
                    _tagServices = Global.GetObject<TagServices>("TagServices");
                }
                return _tagServices; 
            }
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="customerTags?id={customerID}")]
        public IList<TagDto> GetTagsForCustomer(int customerID)
        {
            return TagServices.GetTagsForCustomer(customerID);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.Wrapped)]
        public void CreateTag(int customerID, TagDto tag)
        {
            TagServices.CreateTag(customerID, tag);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.Wrapped)]
        public void TagOperation(int operationID, int tagID)
        {
            TagServices.TagOperation(operationID, tagID);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void UpdateTag(TagDto tag, int customerID)
        {
            TagServices.UpdateTag(tag, customerID);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.Wrapped)]
        public void RemoveTag(int tagID, int customerID)
        {
            TagServices.RemoveTag(tagID, customerID);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="standardTags")]
        public List<TagDto> GetStandardTags()
       {
            return TagServices.GetStandardTags();
        }
    }
}
