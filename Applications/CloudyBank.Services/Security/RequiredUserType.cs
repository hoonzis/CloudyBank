using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Security;

namespace CloudyBank.Services.Security
{
    [AttributeUsage(AttributeTargets.Method)]
    sealed class RequiredUserType : Attribute
    {
        public UserType[] UserTypes {get;set;}

        public RequiredUserType(params UserType[] userTypes)
        {
            UserTypes = userTypes;
        }
    }
}
