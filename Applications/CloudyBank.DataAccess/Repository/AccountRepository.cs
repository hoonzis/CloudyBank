using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;


namespace CloudyBank.DataAccess.Repository
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {

        public AccountRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

        /// <summary>
        /// Returns all accounts of the client
        /// </summary>
        /// <param name="customerId">ID of the customer</param>
        /// <returns></returns>
        public IList<Account> GetAccountsByCustomer(int customerId)
        {
            ISession session = SessionFactory.GetCurrentSession();
            Customer customer = session.Load<Customer>(customerId);
            return customer.RelatedAccounts.Select(x => x.Key).ToList();
        }


        
    }
}
