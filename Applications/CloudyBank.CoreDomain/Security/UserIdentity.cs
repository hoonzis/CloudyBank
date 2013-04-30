using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Security
{
    public class UserIdentity
    {
        public virtual int Id { get; set; }
        public virtual String Identification { get; set; }
        public virtual String Type { get; set; }
        public virtual DateTime ValidityEndDate { get; set; }
        public virtual DateTime ValidityStartDate { get; set; }
        public virtual String Email { get; set; }
        public virtual UserType UserType { get; set; }
    }
}
