using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Security;

namespace CloudyBank.Dto
{
    public class UserIdentityDto
    {
        public int Id { get; set; }
        public String Identification { get; set; }
        public UserType UserType { get; set; }
        public String Email { get; set; }
    }
}
