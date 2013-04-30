using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Dto;
using CloudyBank.CoreDomain;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Security;
using CloudyBank.CoreDomain.Advisors;
using System.Diagnostics.Contracts;

namespace CloudyBank.Core.Services
{
    [ContractClass(typeof(UserServicesContracts))]
    public interface IUserServices
    {
        UserIdentityDto AuthenticateUser(String identity, String password);

        UserIdentityDto GetUserByIdentity(String identity);

        Permission GetCustomerRights(Account account, Customer customer);

        Permission GetAdvisorRights(Account account, Advisor advisor);

        Permission GetUserRights(Account account, UserIdentity user);

        Permission GetUserRights(int accountID, UserIdentity user);
    }

    [ContractClassFor(typeof(IUserServices))]
    public abstract class UserServicesContracts : IUserServices
    {

        public UserIdentityDto AuthenticateUser(String identity, string password)
        {
            Contract.Requires<UserServicesException>(identity != null);
            Contract.Requires<UserServicesException>(password != null);
            return default(UserIdentityDto);
        }

        public UserIdentityDto GetUserByIdentity(String identity)
        {
            Contract.Requires<UserServicesException>(identity != null);
            return default(UserIdentityDto);
        }

        public Permission GetCustomerRights(Account account, Customer customer)
        {
            Contract.Requires<UserServicesException>(account != null);
            Contract.Requires<UserServicesException>(customer != null);
            return default(Permission);
        }

        public Permission GetAdvisorRights(Account account, Advisor advisor)
        {
            Contract.Requires<UserServicesException>(account != null);
            Contract.Requires<UserServicesException>(advisor != null);
            return default(Permission);
        }

        public Permission GetUserRights(Account account, UserIdentity user)
        {
            Contract.Requires<UserServicesException>(account != null);
            Contract.Requires<UserServicesException>(user != null);
            return default(Permission);
        }

        public Permission GetUserRights(int accountID, UserIdentity user)
        {
            Contract.Requires<UserServicesException>(user != null);
            return default(Permission);
        }
    }
}

