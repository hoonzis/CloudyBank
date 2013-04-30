using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.DataAccess;
using CloudyBank.Dto;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain;
using CloudyBank.CoreDomain.Security;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Advisors;
using CloudyBank.Core.Dto;
using CloudyBank.Core.Security;
using CloudyBank.Services.DtoCreators;

namespace CloudyBank.Services
{
    public class UserServices : IUserServices
    {

        private IRepository _repository;
        private ICustomerRepository _customerRepository;
        private IHashProvider _hashProvider;
        private IDtoCreator<UserIdentity, UserIdentityDto> _userIdentityDtoCreator;
        private IUserRepository _userRepository;

        public UserServices(ICustomerRepository customerRepository, IRepository repository,IUserRepository userRepository,
            IHashProvider hashProvider, IDtoCreator<UserIdentity,UserIdentityDto> identityCreator)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _repository = repository;
            _hashProvider = hashProvider;
            _userIdentityDtoCreator = identityCreator;
        }

        public UserIdentityDto GetUserByIdentity(string identity)
        {
            UserIdentity user = _userRepository.GetUserByIdentity(identity);
            if (user != null)
            {
                UserIdentityDto userDto = new UserIdentityDto();
                userDto.Id = user.Id;
                userDto.Identification = user.Identification;
                userDto.UserType = user.UserType;
                return userDto;
            }
            return null;
        }

        public UserIdentityDto AuthenticateUser(string identity, string password)
        {
            Customer customer = _customerRepository.GetCustomerByIdentity(identity);
            if (customer != null)
            {
                String passwordHash = _hashProvider.Hash(password + customer.PasswordSalt);
                if (customer.Password == passwordHash)
                {
                    return _userIdentityDtoCreator.Create(customer);
                }
            }
            return null;
        }

        public Permission GetCustomerRights(Account account, Customer customer)
        {
            if (customer.RelatedAccounts.ContainsKey(account))
            {
                Role role = customer.RelatedAccounts[account];
                return role.Permission;
            }
            return 0;
        }

        public Permission GetAdvisorRights(Account account, Advisor advisor)
        {
            foreach (Customer customer in account.RelatedCustomers.Keys)
            {
                if(advisor.Customers.Contains(customer)){
                    return advisor.Role.Permission;
                }
            }
            return 0;
        }

        public Permission GetUserRights(Account account, UserIdentity user)
        {
            if (user.Type == "Advisor")
            {
                Advisor advisor = _repository.Get<Advisor>(user.Id);
                if (advisor != null)
                {
                    return GetAdvisorRights(account, advisor);
                }
                return Permission.No;

            }
            else if (user.Type == "Customer")
            {
                Customer customer = _repository.Get<Customer>(user.Id);
                if (customer != null)
                {
                    return GetCustomerRights(account, customer);
                }
                return Permission.No;
            }
            else
            {
                throw new UserServicesException("Not expected type of user");
            }
        }

        public Permission GetUserRights(int accountID, UserIdentity user)
        {
            Account account = _repository.Load<Account>(accountID);
            if (account != null)
            {
                return GetUserRights(account, user);
            }
            else
            {
                throw new Exception("Account not found");
            }
            
        }

        public void CreateNewCustomer(String password, String userName,  String email)
        {
            String salt = _hashProvider.CreateSalt(8);
            Customer customer = new Customer { Password = password, PasswordSalt = salt, Identification = userName, Email = email };
        }
    }
}
