using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Security;

namespace CloudyBank.Core.DataAccess
{
    public interface IOAuthRepository
    {
        AuthToken GetToken(String token);
        AuthConsumer GetConsumer(String key);
    }
}
