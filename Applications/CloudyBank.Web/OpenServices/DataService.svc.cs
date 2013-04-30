using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CloudyBank.CoreDomain.Security;
using CloudyBank.Core.Services;
using CloudyBank.Dto;
using CloudyBank.Core.DataAccess;
using System.ServiceModel.Activation;

namespace CloudyBank.Web.OpenServices
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DataService : IDataService
    {
        private IAccountServices _accountServices;

        public IAccountServices AccountServices
        {
            get {
                if (_accountServices == null)
                {
                    _accountServices = Global.GetObject<IAccountServices>("AccountServices");
                }
                return _accountServices;
            }
        }

        private IOperationServices _operationServices;

        public IOperationServices OperationServices
        {
            get {
                if (_operationServices == null)
                {
                    _operationServices = Global.GetObject<IOperationServices>("OperationServices");
                }
                return _operationServices; 
            }
            set { _operationServices = value; }
        }

        private static IUserRepository _userRepository;

        public static IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = Global.GetObject<IUserRepository>("UserRepository");
                }
                return _userRepository;
            }
        }

        private UserIdentity User
        {
            get {
                return UserRepository.GetUserByIdentity(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name);
            }
        }

        public IList<AccountDto> GetAccounts()
        {
            return AccountServices.GetCustomerAccounts(User.Id);
        }

        public IList<OperationDto> GetOperations()
        {
            return OperationServices.GetOperationsDtoByCustomerID(User.Id);
        }
    }
}
