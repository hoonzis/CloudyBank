using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Bank
{
    public class SimpleAudit
    {
        public virtual int Id { get; set; }
        public virtual String MethodName { get; set; }
        public virtual String ServiceName { get; set; }
        public virtual String CalledBy { get; set; }
        public virtual DateTime CalledAt { get; set; }
    }
}
