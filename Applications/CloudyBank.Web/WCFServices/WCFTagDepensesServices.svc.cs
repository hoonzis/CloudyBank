using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CloudyBank.Core;
using System.Collections.Generic;
using CloudyBank.Dto;
using System.Security.Permissions;
using System.Threading;
using System.Web;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "octo.depenses.service", Name="WCFTagDepensesService")]
    [ServiceBehavior(Namespace = "octo.depenses.port", Name = "WCFTagDepensesPort")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFTagDepensesServices
    {
        public WCFTagDepensesServices()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        private ITagDepensesServices _tagDepensesServices;

        private ITagDepensesServices TagDepensesServices
        {
            get {
                if (_tagDepensesServices == null)
                {
                    _tagDepensesServices = Global.GetObject<ITagDepensesServices>("TagDepensesServices");
                }
                return _tagDepensesServices; 
            }
            
        }
        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public IList<TagDepensesDto> GetTagDepensesForProfile(int profileId)
        {
            return TagDepensesServices.GetTagDepensesForProfile(profileId);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public IList<TagDepensesDto> GetTagDepensesForCustomer(int customerId)
        {
            return TagDepensesServices.GetTagDepensesForCustomer(customerId);
        }
    }
}
