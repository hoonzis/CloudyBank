using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.CoreDomain.Bank
{
    public class CustomerProfile
    {
        public virtual int Id { get; set; }
        public virtual int LowAge { get; set; }
        public virtual int HighAge { get; set; }
        public virtual FamilySituation Situation { get; set; }

        public virtual IList<TagDepenses> TagsRepartition { get; set; }
        public virtual IList<Customer> Customers { get; set; }

        public CustomerProfile()
        {
            TagsRepartition = new List<TagDepenses>();
            Customers = new List<Customer>();
        }

    }
}
