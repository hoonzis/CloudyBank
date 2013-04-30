using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CloudyBank.Core.Services;
using System.Collections.Generic;
using CloudyBank.Dto;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.ServiceModel.Web;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "octo.agencies.service", Name = "WCFAgencyService")]
    [ServiceBehavior(Namespace = "octo.agencies.port", Name = "WCFAgencyPort")]
    //[SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFAgencyService
    {
        public WCFAgencyService()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        private IAgencyServices _agencyServices;

        public IAgencyServices AgencyServices
        {
            get {
                if (_agencyServices == null)
                {
                    _agencyServices = Global.GetObject<IAgencyServices>("AgencyServices");
                }
                return _agencyServices;
            }
            set { _agencyServices = value; }
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet]
        public IList<AgencyDto> GetAgencies()
        {
            return AgencyServices.GetAcencies();
        }
    }
}
