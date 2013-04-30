using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.CoreDomain.Bank
{
    public class BusinessPartner
    {
        public virtual int Id { get; set; }
        public virtual String Iban { get; set; }
        public virtual String Name { get; set; }
        public virtual String Description { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
