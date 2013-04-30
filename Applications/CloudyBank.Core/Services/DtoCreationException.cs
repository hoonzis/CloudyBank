using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Core.Services
{
    public class DtoCreationException : Exception
    {
        public DtoCreationException(String message) : base(message) { }
    }
}
