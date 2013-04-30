using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Security;

namespace CloudyBank.Services.Security
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    sealed class RequiredPermission : Attribute
    {
        public int AccountIdParamIndex { get; set; }
        public Permission Permission { get; set; }

        public RequiredPermission(Permission permission,int accountIdParamIndex)
        {
            Permission = permission;
            AccountIdParamIndex = accountIdParamIndex;
        }
    }
}
