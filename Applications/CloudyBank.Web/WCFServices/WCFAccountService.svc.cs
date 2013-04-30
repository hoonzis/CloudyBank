using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CloudyBank.Services;
using CloudyBank.Dto;
using System.Collections.Generic;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Security;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.ServiceModel.Web;

namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "octo.accounts.service", Name="WCFAccountService")]
    [ServiceBehavior(Namespace = "octo.accounts.port", Name = "WCFAccountPort")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFAccountService
    {
        public WCFAccountService()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        private IAccountServices _accountService;
        public IAccountServices AccountService
        {
            get {
                if (_accountService == null)
                {
                    _accountService = Global.GetObject<AccountServices>("AccountServices");
                }
                return _accountService;
            }
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="/accounts?id={id}", BodyStyle=WebMessageBodyStyle.Wrapped)]
        public IList<AccountDto> GetAccountsByCustomer(int id)
        {
            return AccountService.GetCustomerAccounts(id);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebInvoke(UriTemplate="create?name={name}&id={id}&role={role}", BodyStyle=WebMessageBodyStyle.Bare)]
        public void CreateAccount(String name, int id, int role)
        {
            AccountService.CreateAccount(name, id, role);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate = "/accountsEvolution?id={id}")]
        public IList<BalancePointDto> GetAccountEvolution(int id)
        {
            return AccountService.GetAccountBalanceEvolution(id);
        }
    }
}
