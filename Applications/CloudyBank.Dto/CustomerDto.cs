using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Security;

namespace CloudyBank.Dto
{
    public class CustomerDto
    {
        public int Id { get; set; } 
        public String Email { get; set; }
        public String Code { get; set; }
        public String PhoneNumber { get; set; }
        public String Title { get; set; }
        public virtual DateTime BirthDate { get; set; }
        //public virtual String Login { get; set; }
        public String Identification { get; set; }
        public String UserCode { get; set; }

        public String FirstName { get; set; }
        public String LastName { get; set; }
        public FamilySituation Situation { get; set; }
        public UserType Type { get; set; }
    }
}
