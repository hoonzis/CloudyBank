using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Bank
{
    public class TagDepenses
    {
        public virtual int Id { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual decimal Depenses { get; set; }
    }
}
