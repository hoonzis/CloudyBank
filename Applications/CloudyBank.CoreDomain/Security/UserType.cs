using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Security
{
    public enum UserType
    {
        NoType = 0,
        IndividualCustomer = 1,
        CorporateCustomer=2,
        Advisor = 2,
        Administrator=4
        
    }
}
