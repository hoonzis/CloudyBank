using System;
using System.Runtime.Serialization;

namespace CloudyBank.Core.DataAccess
{
    [Serializable]
    public class DataAccessException : Exception
    {
        public DataAccessException() : base() { }
        public DataAccessException(string message) : base(message) { }
        public DataAccessException(string message, Exception inner) : base(message, inner) { }

        protected DataAccessException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

