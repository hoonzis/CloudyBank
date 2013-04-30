using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CloudyBank.Core.Services;
using System.Collections;
using CloudyBank.Dto;
using System.Collections.Generic;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFOAuthManagementService
    {
        private IOAuthServices _oAuthServices;

        public IOAuthServices OAuthServices
        {
            get {
                if (_oAuthServices == null)
                {
                    _oAuthServices = Global.GetObject<IOAuthServices>("OAuthServices");
                }
                return _oAuthServices; 
            }
            
        }

        [OperationContract]
        public bool InvalidateToken(int tokenID)
        {
            return OAuthServices.InvalidateToken(tokenID);
        }

        [OperationContract]
        public IList<TokenDto> GetTokensForCustomer(int customerID)
        {
            return OAuthServices.GetCustomersTokens(customerID);
        }
    }
}
