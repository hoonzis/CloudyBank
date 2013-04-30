using System.Collections.Generic;
using System.Globalization;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using System.Diagnostics.Contracts;
using System;

namespace CloudyBank.Core.Services
{
    [ContractClass(typeof(IAccountServicesContracts))]
    public interface IAccountServices
    {
        IList<AccountDto> GetCustomerAccounts(int customerId);
        IList<AccountDto> GetAccountsByCustomerCode(string code);
        void CreateAccount(string accountName, int customerId,int roleId);
        decimal Balance(Account account);
        IList<BalancePointDto> GetAccountBalanceEvolution(int accountId);

    }

    [ContractClassFor(typeof(IAccountServices))]
    abstract class IAccountServicesContracts : IAccountServices
    {

        public IList<AccountDto> GetCustomerAccounts(int customerId)
        {
            throw new System.NotImplementedException();
        }

        public IList<AccountDto> GetAccountsByCustomerCode(string code)
        {
            Contract.Requires<AccountServicesException>(code != null);
            return default(IList<AccountDto>);
        }

        public IList<AccountDto> GetAllAccounts()
        {
            throw new System.NotImplementedException();
        }

        public void CreateAccount(string accountName, int customerId, int roleId)
        {
            Contract.Requires<AccountServicesException>(accountName != null);
        }

        public decimal Balance(Account account)
        {
            Contract.Requires<AccountServicesException>(account != null);
            return default(decimal);
        }


        public IList<BalancePointDto> GetAccountBalanceEvolution(int accountId)
        {
            throw new System.NotImplementedException();
        }
    }
}
