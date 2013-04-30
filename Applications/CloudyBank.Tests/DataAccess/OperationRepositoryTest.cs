using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Core.DataAccess;
using CloudyBank.UnitTests.TestHelper;
using CloudyBank.DataAccess.Repository;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;


namespace CloudyBank.UnitTests.DataAccess
{
    public class OperationRepositoryTest : DataAccessTestBase
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
            public void GetOperationsByAccount_NoOperationFound()
            {
                IOperationRepository operationRepository = new OperationRepository(NhibernateHelper.SessionFactory);
                Repository repository = new Repository(NhibernateHelper.SessionFactory);

                Customer thirdParty1 = new Customer { Code = "tjdsklfs", Email = "youpala@youpi.com", FirstName = "Olivier", LastName = "Roux", Password = "Toto", PasswordSalt = "sss" };
                Account account1 = new Account { Balance = 201, BalanceDate = DateTime.Now, Number = "dsf1", Iban="1242255"};
                Account account2 = new Account { Balance = 201, BalanceDate = DateTime.Now, Number = "dsf1", Iban="1240940594"};
                Operation operation1 = new Operation { Account = account1, Amount = 200, Direction = Direction.Credit, Motif = "blabla", TransactionCode="1245" };
                Operation operation2 = new Operation { Account = account1, Amount = 654, Direction = Direction.Debit, Motif = "blibli", TransactionCode="12356" };

                using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
                {
                    repository.Save(thirdParty1);
                    repository.Save(account1);
                    repository.Save(account2);
                    repository.Save(operation1);
                    repository.Save(operation2);
                    repository.Flush();
                    IList<Operation> operations = operationRepository.GetOperationsByAccount(account2.Id);
                    Assert.AreEqual(0, operations.Count);
                }
            }

            [TestMethod]
            public void GetOperationsByAccount_SomeOperationsFound()
            {
                IOperationRepository operationRepository = new OperationRepository(NhibernateHelper.SessionFactory);
                Repository repository = new Repository(NhibernateHelper.SessionFactory);

                Customer thirdParty1 = new Customer { Code = "tjdsklfs", Email = "sf@rze.com", FirstName = "Olivier", LastName = "Roux", Password = "Toto", PasswordSalt = "sss" };
                Account account1 = new Account { Balance = 201, BalanceDate = DateTime.Now, Number = "dsf1", Iban = "1242255" };
                Account account2 = new Account { Balance = 201, BalanceDate = DateTime.Now, Number = "dsf1", Iban = "1242255" };
                Operation operation1 = new Operation { Account = account1, Amount = 200, Direction = Direction.Credit, Motif = "blabla", TransactionCode = "1245" };
                Operation operation2 = new Operation { Account = account1, Amount = 654, Direction = Direction.Debit, Motif = "blibli", TransactionCode = "1245" };

                using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
                {
                    repository.Save(thirdParty1);
                    repository.Save(account1);
                    repository.Save(account2);
                    repository.Save(operation1);
                    repository.Save(operation2);
                    repository.Flush();
                    IList<Operation> operations = operationRepository.GetOperationsByAccount(account1.Id);
                    Assert.AreEqual(2, operations.Count);
                }
            }


        }
    }
}
