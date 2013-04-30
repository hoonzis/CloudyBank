using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.DataAccess.Repository
{
    public class CustomerRepository : BaseRepository,ICustomerRepository
    {
        public CustomerRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }
        
        public IList<Customer> FindCustomersByName(string[] names)
        {
            ISession session = SessionFactory.GetCurrentSession();

            var Itps = from itp in session.Query<Customer>() select itp;

            if (names.Length == 1)
                Itps = Itps.Where(i => i.FirstName == names[0] || i.LastName == names[0]);
            else if (names.Length == 2)
                Itps = Itps.Where(i => i.FirstName == names[0] && i.LastName == names[1] || i.FirstName == names[1] && i.LastName == names[0]);

            return Itps.ToList();
        }

        public Customer GetCustomerByIdentity(string identity)
        {
            ISession session = SessionFactory.GetCurrentSession();
            return session.Query<Customer>().SingleOrDefault(x => x.Identification == identity);
        }

        public Customer GetCustomerByCode(string code)
        {
            ISession session = SessionFactory.GetCurrentSession();
            return session.Query<Customer>().SingleOrDefault(x => x.Code == code);
        }
    }
}
