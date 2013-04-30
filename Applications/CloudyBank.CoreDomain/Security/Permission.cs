using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Security
{
    [Flags]
    public enum Permission
    {
        No = 0,
        Write = 1,
        Read = 2,
        TagOperations = 4,
        Modify = 8,
        Transfer = 16
    }
}
