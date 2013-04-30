using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Security;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.Core.DataAccess
{
    public interface IUserRepository
    {
        UserIdentity GetUserByIdentity(String login);

        Role GetUserRole(String login);

        Permission GetUserPermission(int accountId, String login);

        UserType GetUserType(String login);
    }
}
