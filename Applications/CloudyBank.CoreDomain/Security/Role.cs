using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Security
{
    public class Role
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
