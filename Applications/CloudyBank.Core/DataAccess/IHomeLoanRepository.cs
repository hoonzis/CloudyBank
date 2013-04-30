using System.Collections.Generic;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.Core.DataAccess
{
    public interface IHomeLoanRepository
    {
        /// <summary>
        /// Returns the home loan of the client
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        IList<HomeLoan> GetHomeLoanByCustomer(Customer customer);

    }
}
