using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Common.Logging;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFLogService
    {
        private ILog _log = LogManager.GetLogger(typeof(WCFLogService));

        [OperationContract]
        void LogError(String message)
        {
            _log.Error(message);
        }

        [OperationContract]
        void LogMessage(String message)
        {
            _log.Info(message);
        }
    }
}
