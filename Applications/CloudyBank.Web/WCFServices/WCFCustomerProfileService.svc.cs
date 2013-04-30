using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CloudyBank.Dto;
using CloudyBank.Core.Services;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using System.Web;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "octo.customerprofiles.service", Name="WCFCustomerProfileService")]
    [ServiceBehavior(Namespace = "octo.customerprofiles.port", Name = "WCFCustomerProfilePort")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFCustomerProfileService
    {
        public WCFCustomerProfileService()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        private ICustomerProfileServices _profileServices;

        private ICustomerProfileServices ProfileServices
        {
            get {
                if (_profileServices == null)
                {
                    _profileServices =  Global.GetObject<ICustomerProfileServices>("CustomerProfileServices");
                }
                return _profileServices; 
            }
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public IList<CustomerProfileDto> GetCustomerProfiles()
        {
            return ProfileServices.GetAllCustomerProfiles();
        }

    }
}
