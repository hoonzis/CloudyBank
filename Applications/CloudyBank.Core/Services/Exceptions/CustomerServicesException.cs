using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Core.Services
{
    public class CustomerServicesException : Exception
    {
        public CustomerServicesException() : base() { }
        public CustomerServicesException(string message) : base(message) { }
        public CustomerServicesException(string message, Exception inner) : base(message, inner) { }
    }
}
