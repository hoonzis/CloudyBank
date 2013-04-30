using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.CoreDomain.Security
{
    public class AuthToken
    {
        public virtual int Id { get; set; }
        public virtual AuthConsumer AuthConsumer { get; set; }
        public virtual AuthTokenState State { get; set; }
        public virtual DateTime IssueDate { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual String TokenSecret { get; set; }
        public virtual String Token { get; set; }
        public virtual String Version { get; set; }
        public virtual String VerificationCode { get; set; }
        public virtual DateTime? ExpirationDate { get; set; }
        public virtual String[] Roles { get; set; }
        public virtual String Callback { get; set; }

        //scopeName is human readable name which correponds to the URL
        public virtual String ScopeName { get; set; }

        //scope is the URL which the client application will have right to access
        public virtual String Scope { get; set; }
    }
}
