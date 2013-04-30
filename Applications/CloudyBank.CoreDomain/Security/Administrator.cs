using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Security
{
    public class Administrator : UserIdentity
    {
        public Administrator()
        {
            UserType = Security.UserType.Administrator;
        }
    }
}
