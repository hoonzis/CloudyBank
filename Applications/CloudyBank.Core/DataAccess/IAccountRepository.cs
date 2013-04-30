using System.Collections.Generic;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.Core.DataAccess
{
    public interface IAccountRepository
    {
        /// <summary>
        /// Returns all accounts of the client
        /// </summary>
        /// <param name="customerId">ID of the customer</param>
        /// <returns></returns>
        IList<Account> GetAccountsByCustomer(int customerId);


    }
}
