using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.CoreDomain.Bank
{
    public class UserTag : Tag
    {
        public virtual Customer Customer { get; set; }
    }
}
