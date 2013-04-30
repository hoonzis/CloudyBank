using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Core.Services
{
    public class TagServicesException : Exception
    {
        public TagServicesException(string message) : base(message) { }
    }
}
