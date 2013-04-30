using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.Core.Integration
{
    public interface ICBSAccess
    {
        bool ProcessPayments();

        bool SendOperationToCBS(Operation operation);
    }
}
