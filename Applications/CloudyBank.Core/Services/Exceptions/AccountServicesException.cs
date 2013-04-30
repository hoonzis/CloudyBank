using System;

namespace CloudyBank.Core.Services
{
    public class AccountServicesException : Exception
    {
        public AccountServicesException() : base() { }
        public AccountServicesException(string message) : base(message) { }
        public AccountServicesException(string message, Exception inner) : base(message, inner) { }
    }
}
