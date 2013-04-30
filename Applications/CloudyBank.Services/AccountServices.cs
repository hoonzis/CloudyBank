using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using CloudyBank.Core.DataAccess;

using CloudyBank.CoreDomain.Security;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.Core.Dto;

namespace CloudyBank.Services
{

    public class AccountServices : IAccountServices
    {
        private readonly IRepository _repository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IDtoCreator<Account, AccountDto> _accountDtoCreator;

        public AccountServices(IRepository repository, IAccountRepository accountRepository, ICustomerRepository customerRepository,
            IDtoCreator<Account, AccountDto> accountDtoCreator)
        {
            _repository = repository;
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _accountDtoCreator = accountDtoCreator;
        }

        public IList<AccountDto> GetCustomerAccounts(int customer)
        {
            var accounts = _accountRepository.GetAccountsByCustomer(customer);
            if (accounts != null)
            {
                return accounts.Select(x => _accountDtoCreator.Create(x)).ToList();
            }
            return null;
        }
       
        public Decimal Balance(Account account)
        {
            if (account.Operations != null)
            {
                return account.Operations.Sum(x => x.SignedAmount);
            }
            return 0;
        }

        public IList<AccountDto> GetAccountsByCustomerCode(String code)
        {
            throw new NotImplementedException();
        }

        public void CreateAccount(String accountName, int customerID, int roleID)
        {
            Customer customer = _repository.Get<Customer>(customerID);
            if (customer == null)
            {
                throw new AccountServicesException("No customer found for the ID: " + customerID);
            }

            Role role = _repository.Get<Role>(roleID);
            if (role == null)
            {
                throw new AccountServicesException("No role found for the ID: " + roleID);
            }

            Account account = new Account();
            account.Name = accountName;
            customer.RelatedAccounts.Add(account, role);
            account.RelatedCustomers.Add(customer, role);
            
            using (TransactionScope scope = new TransactionScope())
            {
                _repository.Save<Account>(account);
                _repository.SaveOrUpdate<Customer>(customer);
                scope.Complete();
            }
        }



        public IList<BalancePointDto> GetAccountBalanceEvolution(int accountId)
        {
            Account account = _repository.Get<Account>(accountId);
            if (account != null)
            {
                return account.BalancePoints.Select(x => CreateBalancePoint(x)).ToList();
            }
            return null;
        }

        private BalancePointDto CreateBalancePoint(BalancePoint point)
        {
            var dto = new BalancePointDto();
            dto.Balance = point.Balance;
            dto.Date = point.Date;
            dto.AccountId = point.Account.Id;
            dto.Id = point.Id;

            return dto;
        }
    }
}
