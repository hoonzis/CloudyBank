using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Bank
{
    public class BalancePoint
    {
        public virtual int Id { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual decimal Balance { get; set; }
        public virtual Account Account { get; set; }
    }
}
