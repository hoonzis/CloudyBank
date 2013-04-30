using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Core.DataAccess;
using  CloudyBank.UnitTests.TestHelper;
using  CloudyBank.DataAccess.Repository;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Security;
using CloudyBank.CoreDomain;
using CloudyBank.CoreDomain.Customers;

namespace  CloudyBank.UnitTests.DataAccess
{
    /// <summary>
    /// Summary description for NhibernateConfigurationTest
    /// </summary>
    [TestClass]
    public class NhibernateConfigurationTest : DataAccessTestBase
    {
        readonly IRepository _repo = new Repository(NhibernateHelper.SessionFactory);

        [TestMethod]
        public void TestNhibernateConfigurationWithUserIdentity()
        {
            UserIdentity user = new UserIdentity
                            {
                                Identification = "Ident",
                                ValidityEndDate = DateTime.Now.AddDays(30),
                                ValidityStartDate = DateTime.Now
                            };

            using (TransactionScope scope = new TransactionScope())
            {
                _repo.Save(user);
                scope.Complete();
            }

            int userId = user.Id;
            _repo.Clear();

            UserIdentity userRetrieved = _repo.Get<UserIdentity>(userId);
            Assert.AreEqual(userId, userRetrieved.Id);

            using (TransactionScope scope = new TransactionScope())
            {
                _repo.Delete(userRetrieved);
                scope.Complete();
            }
            _repo.Clear();

            Assert.IsNull(_repo.Get<UserIdentity>(userId));
        }

        [TestMethod]
        public void TestNhibernateConfigurationWithAccount()
        {
            Customer iTP = new Customer
            {
                FirstName = "Simon",
                LastName = "Lehericey",
                Code = "ITP1",
                Email = "toto@tata.com",
                Password = "Toto",
                PasswordSalt = "sss"
            };


            Account account = new Account
            {
                Balance = 21,
                BalanceDate = DateTime.Now,
                Number = "n1",
                AuthorizeOverdraft = true,
                CreationDate = DateTime.Now, 
                Name = "super compte",
                NbOfDaysOverdraft=3,
                Iban = "1234"
            };

            using (TransactionScope scope = new TransactionScope())
            {
                _repo.Save(iTP);
                _repo.Save(account);
                scope.Complete();
            }

            int iTPId = iTP.Id;
            _repo.Clear();

            Customer iTPRetrieved = _repo.Get<Customer>(iTPId);

            Assert.AreEqual(iTPId, iTPRetrieved.Id);
            
            using (TransactionScope scope = new TransactionScope())
            {
                _repo.Delete(_repo.Get<Account>(account.Id));
                scope.Complete();
            }

            using (TransactionScope scope = new TransactionScope())
            {
                _repo.Delete(iTPRetrieved);
                scope.Complete();
            }
            _repo.Clear();

            Assert.IsNull(_repo.Get<Customer>(iTPId));
        }

        [TestMethod]
        public void TestNHibernateConfigurationWithOperation()
        {
            Customer iTP = new Customer
            {
                FirstName = "Simon",
                LastName = "Lehericey",
                Code = "ITP1",
                Email = "toto@tata.com",
                Password = "toto",
                PasswordSalt = "sss"
            };


            Account account = new Account
            {
                Balance = 21,
                BalanceDate = DateTime.Now,
                Number = "n1",
                Iban = "1234"
            };
           

            Operation operation = new Operation {Amount = 200, Direction=Direction.Credit, Account=account, TransactionCode="1234"};

            using (TransactionScope scope = new TransactionScope())
            {
                _repo.Save(iTP);
                _repo.Save(account);
                _repo.Save(operation);
                scope.Complete();
            }

            int opeId = operation.Id;

            Operation operationRetrieved = _repo.Get<Operation>(opeId);

            Assert.IsNotNull(operationRetrieved);


            using (TransactionScope scope = new TransactionScope())
            {
                _repo.Delete(operationRetrieved);
                _repo.Delete(account);
                _repo.Delete(iTP);
                scope.Complete();
            }

          
            _repo.Clear();

            Assert.IsNull(_repo.Get<Operation>(opeId));
        }


        private static void AreAccountEqual(Account expectedAccount, Account actualAccount)
        {
            Assert.AreEqual(expectedAccount.AuthorizeOverdraft ,actualAccount.AuthorizeOverdraft );
            Assert.AreEqual(expectedAccount.Balance ,actualAccount.Balance );
            Assert.AreEqual(expectedAccount.BalanceDate.ToString() ,actualAccount.BalanceDate.ToString() );
            Assert.AreEqual(expectedAccount.CreationDate.ToString() ,actualAccount.CreationDate.ToString() );
            Assert.AreEqual(expectedAccount.Name ,actualAccount.Name );
            Assert.AreEqual(expectedAccount.NbOfDaysOverdraft ,actualAccount.NbOfDaysOverdraft );
        }
    }
}
