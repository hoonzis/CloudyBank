using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Security;
using CloudyBank.CoreDomain.Customers;


namespace CloudyBank.CoreDomain.Advisors
{
    public class Advisor : UserIdentity
    {
        public virtual String FirstName { get; set; }
        public virtual String LastName { get; set; }
        public virtual IList<Task> Tasks { get; set; }
        public virtual IList<Customer> Customers { get; set; }
        public virtual Role Role { get; set; }

        public Advisor()
        {
            Customers = new List<Customer>();
            UserType = Security.UserType.Advisor;
        }
    }
}
