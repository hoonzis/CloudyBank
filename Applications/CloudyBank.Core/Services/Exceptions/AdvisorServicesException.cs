using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Core.Services
{
    public class AdvisorServicesException : Exception
    {
        public AdvisorServicesException(string message) : base(message) { }
    }
}
