using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Security
{
    public class AuthConsumer
    {
        public virtual int Id { get; set; }
        public virtual String Name { get; set; }
        public virtual String Description { get; set; }
        public virtual String ConsumerKey { get; set; }
        public virtual String Secret { get; set; }
        public virtual String Callback { get; set; }
        public virtual int VerificationCodeLength { get; set; }
        public virtual int VerificationCodeFormat { get; set; }
    }
}
