using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Security;
using CloudyBank.Dto;

namespace CloudyBank.Core.Services
{
    public interface IOAuthServices
    {
        AuthConsumer GetConsumer(String key);
        AuthToken GetRequestToken(String token);
        AuthToken GetAccessToken(String token);
        void UpdateToken(AuthToken token);
        void StoreNewRequestToken(AuthToken token);
        AuthToken GetToken(String token);
        bool InvalidateToken(int tokenID);
        IList<TokenDto> GetCustomersTokens(int customerID);
    }
}
