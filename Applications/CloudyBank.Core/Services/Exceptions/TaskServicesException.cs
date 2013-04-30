using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Core.Services
{
    public class TaskServicesException : Exception
    {
        public TaskServicesException(string message) : base(message) { }
    }
}
