using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.DataAccess;
using NHibernate;
using NHibernate.Linq;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.DataAccess.Repository
{
    public class TagRepository : BaseRepository, ITagRepository
    {
        public TagRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }
        public IList<UserTag> GetTagsForCustomer(int customerID)
        {
            ISession session = SessionFactory.GetCurrentSession();
            var customer = session.Load<Customer>(customerID);
            return customer.Tags;
        }
    }
}
