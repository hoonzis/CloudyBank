using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.Core.DataAccess
{
    public interface IPaymentEventRepository
    {
        IList<PaymentEvent> GetPaymentEventsForCustomer(int customerID);
    }
}
