using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.CoreDomain.Bank
{
    public class PaymentEvent
    {
        public virtual int Id { get; set; }
        public virtual String Name { get; set; }
        public virtual String Description { get; set; }
        public virtual Operation Operation { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Account Account { get; set; }
        public virtual String PartnerIban { get; set; }
        public virtual BusinessPartner Partner { get; set; }
        public virtual bool Payed { get; set; }
        public virtual bool Regular { get; set; }
        public virtual decimal Amount { get; set; }
    }
}
