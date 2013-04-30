using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Security
{
    public class AuthNonce
    {
        public virtual int Id { get; set; }
        public virtual String Context { get; set; }
        public virtual String Code { get; set; }
        public virtual DateTime? Timestamp { get; set; }
    }
}
