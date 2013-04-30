using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Core.Services
{
   public class OperationServicesException : Exception
    {
        public OperationServicesException() : base() { }
        public OperationServicesException(string message) : base(message) { }
        public OperationServicesException(string message, Exception inner) : base(message, inner) { }

    }
}
