using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Core.DataAccess;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using CloudyBank.Services;
using Rhino.Mocks;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Security;
using CloudyBank.Core.Dto;
using CloudyBank.Services.DtoCreators;
using System;


namespace CloudyBank.UnitTests.Services
{
    [TestClass]
    public class AccountServicesTest
    {

        [TestMethod]
        public void GetClientAccounts_ClientNotNull()
        {
            //arange
            IRepository repository = MockRepository.GenerateStub<IRepository>();
            IAccountRepository accountRepository = MockRepository.GenerateMock<IAccountRepository>();
            ICustomerRepository thirdPartyRepository = MockRepository.GenerateStub<ICustomerRepository>();
            IDtoCreator<Account, AccountDto> accountCreator = new AccountDtoCreator();

            Customer customer = new Customer { Id = 3 };
            IList<Account> accounts = new List<Account>();
            Account account = new Account() { Id = 1 };
            accounts.Add(account);

            accountRepository.Expect(x => x.GetAccountsByCustomer(customer.Id)).Return(accounts);

            //act
            AccountServices services = new AccountServices(repository, accountRepository, thirdPartyRepository,accountCreator);
            IList<AccountDto> retrievedAccounts = services.GetCustomerAccounts(customer.Id);

            //assert
            Assert.AreEqual(1, retrievedAccounts.Count);
            Assert.AreEqual(account.Id, retrievedAccounts[0].Id);
            accountRepository.VerifyAllExpectations();
        }

        [TestMethod]
        [ExpectedException(typeof(AccountServicesException))]
        public void CreateAccount_AccountDtoIsNull()
        {
            //arrange
            IRepository repository = MockRepository.GenerateStub<IRepository>();
            IAccountRepository accountRepository = MockRepository.GenerateStub<IAccountRepository>();
            ICustomerRepository thirdPartyRepository = MockRepository.GenerateStub<ICustomerRepository>();
            IDtoCreator<Account, AccountDto> accountCreator = new AccountDtoCreator();

            //act
            IAccountServices services = new AccountServices(repository, accountRepository, thirdPartyRepository, accountCreator);
            services.CreateAccount(null, 0,0);

        }

        [TestMethod]
        [ExpectedException(typeof(AccountServicesException))]
        public void CreateAccount_itNotFound()
        {
            //arrange
            IRepository repository = MockRepository.GenerateStub<IRepository>();
            IAccountRepository accountRepository = MockRepository.GenerateStub<IAccountRepository>();
            ICustomerRepository thirdPartyRepository = MockRepository.GenerateStub<ICustomerRepository>();
            IDtoCreator<Account, AccountDto> accountCreator = new AccountDtoCreator();

            string accountName = "Account Name";

            //act
            IAccountServices services = new AccountServices(repository, accountRepository, thirdPartyRepository, accountCreator);
            services.CreateAccount(accountName, 1,0);
        }

        [TestMethod]
        public void CreateAccount_ok()
        {
            //arrange
            IRepository repository = MockRepository.GenerateStub<IRepository>();
            IAccountRepository accountRepository = MockRepository.GenerateStub<IAccountRepository>();
            ICustomerRepository thirdPartyRepository = MockRepository.GenerateStub<ICustomerRepository>();
            IDtoCreator<Account, AccountDto> accountCreator = new AccountDtoCreator();

            string accountName = "name";
            Customer customer = new Customer() { Id = 1 };
            Role role = new Role { Id = 1 };

            repository.Expect(x => x.Get<Customer>(customer.Id)).Return(customer);
            repository.Expect(x => x.Get<Role>(role.Id)).Return(role);
            
            //act
            IAccountServices services = new AccountServices(repository, accountRepository, thirdPartyRepository, accountCreator);
            services.CreateAccount(accountName, customer.Id,role.Id);

            //assert
            repository.VerifyAllExpectations();
            repository.AssertWasCalled(x => x.SaveOrUpdate<Customer>(customer));
            repository.AssertWasCalled(x => x.Save<Account>(Arg<Account>.Is.NotNull));
        }

        [TestMethod]
        public void Balance_AccountOK()
        {
            //arrange
            Account account = new Account();
            account.Operations = new List<Operation>();
            account.Operations.Add(new Operation { Account = account, Amount = 10, Direction = Direction.Credit });
            account.Operations.Add(new Operation { Account = account, Amount = 20, Direction = Direction.Debit });
            account.Operations.Add(new Operation { Account = account, Amount = 0, Direction = Direction.Credit });

            
            IRepository repository = MockRepository.GenerateStub<IRepository>();
            ICustomerRepository customerRepository = MockRepository.GenerateStub<ICustomerRepository>();
            IAccountRepository accountRepository = MockRepository.GenerateStub<IAccountRepository>();
            IDtoCreator<Account, AccountDto> accountCreator = new AccountDtoCreator();

            //act
            AccountServices services = new AccountServices(repository, accountRepository, customerRepository, accountCreator);
            decimal balance = services.Balance(account);
            
            //assert
            Assert.AreEqual(balance, -10);
        }        
    }
}
