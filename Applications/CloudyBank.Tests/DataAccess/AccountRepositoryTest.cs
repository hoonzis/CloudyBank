using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Core.DataAccess;
using CloudyBank.DataAccess.Repository;
using CloudyBank.UnitTests.TestHelper;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Security;


namespace CloudyBank.UnitTests.DataAccess
{
    [TestClass]
    public class AccountRepositoryTest : DataAccessTestBase
    {
        [TestInitialize]
        public void SetUp()
        {
            MyTestInitialize();
        }
        
        [TestMethod]
        public void GetAccountByCustomerNoAccountFound()
        {
            IAccountRepository accountRepository = new AccountRepository(NhibernateHelper.SessionFactory);
            Repository repository = new Repository(NhibernateHelper.SessionFactory);

            Customer thirdParty1 = new Customer { Code = "tjdsklfs", Email = "youpala@youpi.com", LastName = "Roux", FirstName = "Olivier", Password = "Pass1", PasswordSalt = "sss" };
            Customer thirdParty2 = new Customer { Code = "topsecret", Email = "blalba@loulou.com", LastName = "Roux2", FirstName = "Olivier", Password = "Pass2", PasswordSalt = "sss" };
            Account account1 = new Account { Balance = 201, BalanceDate = DateTime.Now, Number = "dsf1", Iban="12349340943"};

            using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
            {
                repository.Save(thirdParty1);
                repository.Save(thirdParty2);
                repository.Save(account1);

                repository.Flush();

                IList<Account> accounts = accountRepository.GetAccountsByCustomer(thirdParty2.Id);
                Assert.AreEqual(0, accounts.Count);
            }
        }

        [TestMethod]
        public void GetAccountsByCustomerSomeAccountsFound()
        {
            IAccountRepository accountRepository = new AccountRepository(NhibernateHelper.SessionFactory);
            Repository repository = new Repository(NhibernateHelper.SessionFactory);

            Customer thirdParty1 = new Customer { Code = "tjdsklfs", Email = "youpala@youpi.com", LastName = "roux", FirstName = "Olivier", Password = "Pass", PasswordSalt = "sss" };
            Customer thirdParty2 = new Customer { Code = "topsecret", Email = "blalba@loulou.com", LastName = "roux2", FirstName = "Olivier", Password = "Pass2", PasswordSalt = "sss" };
            
            Account account1 = new Account { Balance = 201, BalanceDate = DateTime.Now, Number = "dsf1", Iban="12354"};
            Account account2 = new Account { Balance = 202, BalanceDate = DateTime.Now, Number = "dsf2", Iban="12435"};

            Role role = new Role{Id=1};
            thirdParty1.RelatedAccounts.Add(account1, role);
            thirdParty1.RelatedAccounts.Add(account2, role);

            using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
            {
                repository.Save(thirdParty1);
                repository.Save(thirdParty2);
                repository.Save(account1);
                repository.Save(account2);

                repository.Flush();

                IList<Account> accounts = accountRepository.GetAccountsByCustomer(thirdParty1.Id);
                Assert.AreEqual(2, accounts.Count);
            }
        }

    }
}
