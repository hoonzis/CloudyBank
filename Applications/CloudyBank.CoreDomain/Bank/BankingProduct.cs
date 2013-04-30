using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Bank
{
    public class BankingProduct
    {
        public virtual String Name { get; set; }
        
        //Changing the DateTime to nullable - nHiberante SqlTimeOverflow issue
        public virtual DateTime? CreationDate { get; set; }

    }
}
