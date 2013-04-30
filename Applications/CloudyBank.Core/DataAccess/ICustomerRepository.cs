using System;
using System.Collections.Generic;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Advisors;

namespace CloudyBank.Core.DataAccess
{
    public interface ICustomerRepository
    {
        /// <summary>
        /// Find all clients where names are maching
        /// </summary>
        /// <param name="names">1 or 2 names</param>
        /// <returns></returns>
        IList<Customer> FindCustomersByName(string[] names);

        /// <summary>
        /// Returns the customer identified by its login
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        Customer GetCustomerByIdentity(string identity);

        /// <summary>
        /// Returns customer identified by its code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Customer GetCustomerByCode(string code);
    }
}
