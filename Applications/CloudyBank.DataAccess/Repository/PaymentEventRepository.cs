using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Bank;
using NHibernate;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.DataAccess.Repository
{
    class PaymentEventRepository : BaseRepository, IPaymentEventRepository
    {
        public PaymentEventRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

        public IList<PaymentEvent> GetPaymentEventsForCustomer(int customerID)
        {
            Customer customer = SessionFactory.GetCurrentSession().Load<Customer>(customerID);
            if (customer == null)
            {
                return null;
            }
            return customer.PaymentEvents;
        }
    }
}
