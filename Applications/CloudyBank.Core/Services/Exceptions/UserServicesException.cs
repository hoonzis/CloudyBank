using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Core.Services
{
    public class UserServicesException : Exception
    {
        public UserServicesException(string message) : base(message) { }
    }
}
