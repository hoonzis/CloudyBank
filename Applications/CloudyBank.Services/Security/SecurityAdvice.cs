using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spring.Aop;
using CloudyBank.CoreDomain.Security;
using System.Web;
using CloudyBank.Core.DataAccess;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Bank;
using System.Security;

namespace CloudyBank.Services.Security
{
    class SecurityAdvice : IMethodBeforeAdvice
    {
        IUserRepository _userRepository;

        public SecurityAdvice(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public void Before(System.Reflection.MethodInfo method, object[] args, object target)
        {
            String currentUser = HttpContext.Current.User.Identity.Name;

            object[] permissionAttributes = method.GetCustomAttributes(typeof(RequiredPermission), false);

            if (permissionAttributes.Count() == 1)
            {
                RequiredPermission permissionAttribute = permissionAttributes[0] as RequiredPermission;
                int accountId = (int)args[permissionAttribute.AccountIdParamIndex];


                Permission permission = _userRepository.GetUserPermission(accountId, currentUser);

                var result = permissionAttribute.Permission & permission;
                if (result!= permissionAttribute.Permission)
                {
                    throw new SecurityException("Not enough user rights");
                }
            }

            bool isUserGoodType = false;

            object[] userTypeAttributes = method.GetCustomAttributes(typeof(RequiredUserType), false);
            
            if (userTypeAttributes != null && userTypeAttributes.Count() == 1)
            {
                UserType currentUserType = _userRepository.GetUserType(currentUser);

                UserType[] userTypes = (userTypeAttributes[0] as RequiredUserType).UserTypes;
                foreach (UserType requestedUserType in userTypes)
                {
                    if (currentUserType == requestedUserType)
                    {
                        isUserGoodType = true;
                        break;
                    }
                }

                if (!isUserGoodType)
                {
                    throw new SecurityException(String.Format("This method is not allowed for user type: {0}", currentUserType));
                }
            }
        }
    }
}
