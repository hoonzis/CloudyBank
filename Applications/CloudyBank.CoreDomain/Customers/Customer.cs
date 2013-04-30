using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Advisors;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Security;

namespace CloudyBank.CoreDomain.Customers
{
    public class Customer : UserIdentity
    {
        public virtual String Code { get; set; }
        public virtual String PhoneNumber { get; set; }
        public virtual Advisor Advisor { get; set; }
        public virtual CustomerProfile CustomerProfile { get; set; }
        public virtual String FirstName { get; set; }
        public virtual String LastName { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public virtual FamilySituation Situation { get; set; }


        public virtual String Password{get;set;}
        public virtual String PasswordSalt {get;set;}

        public virtual IDictionary<Account,Role> RelatedAccounts { get; set; }
        public virtual IList<UserTag> Tags { get; set; }
        public virtual IList<PaymentEvent> PaymentEvents { get; set; }
        public virtual IList<BusinessPartner> Partners { get; set; }
        public virtual IList<TagDepenses> TagDepenses { get; set; }
        public virtual IList<CustomerImage> Images { get; set; }
        public virtual IList<AuthToken> Tokens { get; set; }


        public Customer() {
            this.RelatedAccounts = new Dictionary<Account, Role>();
            this.PaymentEvents = new List<PaymentEvent>();
            this.Tags = new List<UserTag>();
            this.Partners = new List<BusinessPartner>();
            UserType = Security.UserType.IndividualCustomer;
            Situation = FamilySituation.NotSet;
        }

        public virtual int GetAge()
        {
            return (int)(DateTime.Now.Subtract(BirthDate).TotalDays / 365);
        }
    }
}
