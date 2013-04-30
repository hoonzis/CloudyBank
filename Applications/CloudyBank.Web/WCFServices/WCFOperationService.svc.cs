using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using CloudyBank.Dto;
using CloudyBank.Services;
using CloudyBank.Core.Services;
using System.Security.Permissions;

using System.Threading;
using System.Web;
using System.ServiceModel.Web;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "octo.operations.service", Name="WCFOperationService")]
    [ServiceBehavior(Namespace = "octo.operations.port", Name = "WCFOperationPort")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFOperationService
    {
        public WCFOperationService()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        private IOperationServices _operationServices;

        public IOperationServices OperationServices
        {
            get {
                if (_operationServices == null)
                {
                    _operationServices = Global.GetObject<OperationServices>("AdvicedOperationServices");
                }
                return _operationServices; }
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="account/operations/?account={accountId}")]
        public IList<OperationDto> GetOperationsByAccount(int accountID)
        {
            return OperationServices.GetOperations(accountID);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate = "makeTransfer?debitAccount={debitAccountId}&creditAccount={creditAccountId}&amount={amount}&motif={motif}", BodyStyle=WebMessageBodyStyle.Bare)]
        public String MakeTransfer(int debitAccountId, int creditAccountId, Decimal amount, String motif)
        {
            return OperationServices.MakeTransfer(debitAccountId, creditAccountId, amount, motif);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate = "makeTransferToExternal?debitAccount={debitAccountId}&creditAccountIban={creditAccountIban}&amount={amount}&motif={motif}", BodyStyle = WebMessageBodyStyle.Bare)]
        public String MakeTransferToExternal(int debitAccountId, String creditAccountIban, Decimal amount, String motif)
        {
            return OperationServices.MakeTransferToExternal(debitAccountId, creditAccountIban, amount, motif);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="operation?id={operationId}")]
        public OperationDto GetOperationById(int operationId)
        {
            return OperationServices.GetOperationById(operationId);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="operations?account={accountId}&code={transactionCode}", BodyStyle=WebMessageBodyStyle.Bare)]
        public OperationDto GetOperationByAccountIdCode(int accountId, String transactionCode)
        {
            return OperationServices.GetOperationByAccountIdCode(accountId, transactionCode);
        }
    }
}
