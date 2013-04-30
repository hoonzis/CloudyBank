using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Bank
{
    public class Audit
    {
        public virtual int Id { get; set; }
        public virtual String TableName {get;set;}
        public virtual String Property { get; set; }
        public virtual String OldValue { get; set; }
        public virtual String NewValue { get; set; }
        public virtual DateTime ChangeTime { get; set; }
        public virtual String AffectedUser { get; set; }
    }
}
