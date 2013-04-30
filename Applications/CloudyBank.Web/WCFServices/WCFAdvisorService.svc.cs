using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Security;
using CloudyBank.Dto;
using CloudyBank.Core.Services;
using System.Security.Permissions;
using System.Threading;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "octo.advisors.service", Name = "WCFAdvisorService")]
    [ServiceBehavior(Namespace = "octo.advisors.port", Name = "WCFAdvisorPort")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFAdvisorService
    {
        public WCFAdvisorService()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        private IAdvisorServices _advisorServices;

        public IAdvisorServices AdvisorServices
        {
            get {
                if (_advisorServices == null)
                {
                    _advisorServices = Global.GetObject<IAdvisorServices>("AdvisorServices");
                }
                return _advisorServices;
            }
            set { _advisorServices = value; }
        }


        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public AdvisorDto GetCurrentAdvisor()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (HttpContext.Current.User.Identity as FormsIdentity);                 
                AdvisorDto advisor = AdvisorServices.GetAdvisorByIdentity(HttpContext.Current.User.Identity.Name);
                return advisor;
            }
            return null;        
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public AdvisorDto GetAdvisorById(int id)
        {
            return AdvisorServices.GetAdvisorById(id);
        }
    }
}
