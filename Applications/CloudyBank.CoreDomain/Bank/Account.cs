using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Security;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.CoreDomain.Bank
{
    public class Account : BankingProduct
    {
        public virtual int Id { get; set; }
        public virtual Decimal Balance { get; set; }
        public virtual DateTime? BalanceDate { get; set; }
        public virtual String Number { get; set; }
        public virtual IList<Operation> Operations { get; set; }
        public virtual int NbOfDaysOverdraft { get; set; }
        public virtual bool AuthorizeOverdraft { get; set; }
        public virtual IDictionary<Customer,Role> RelatedCustomers { get; set; }
        public virtual String Iban { get; set; }
        public virtual String Currency { get; set; }
        public virtual IList<BalancePoint> BalancePoints { get; set; }
        public virtual IList<TagDepenses> TagDepenses { get; set; }

        
        public Account()
        {
            Operations = new List<Operation>();
            RelatedCustomers = new Dictionary<Customer, Role>();
            BalancePoints = new List<BalancePoint>();
        }
    }
}
