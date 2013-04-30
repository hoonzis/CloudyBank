using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.Attributes;

namespace CloudyBank.CoreDomain.Bank
{
    public class Operation
    {
        public virtual int Id { get; set; } 


        public virtual Decimal Amount { get; set; }
        public virtual Direction Direction { get; set; }
        public virtual Account Account { get; set; }

        [StringFeature(SplitType = StringType.Word)]
        public virtual String Description { get; set; }
        public virtual String Motif { get; set; }
        
        [Label]
        public virtual Tag Tag { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual String TransactionCode { get; set; }
        public virtual String OppositeIban { get; set; }
        public virtual String Currency { get; set; }
        
        public virtual Decimal SignedAmount 
        {
            get { return (int)Direction * Amount; }
        }
    }
}
