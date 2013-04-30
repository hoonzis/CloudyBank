using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.UnitTests.TestHelper;
using CloudyBank.Core.DataAccess;
using CloudyBank.DataAccess.Repository;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Security;
using CloudyBank.CoreDomain.Customers;



namespace CloudyBank.UnitTests.DataAccess
{
    [TestClass]
    public class UserRepositoryTest : DataAccessTestBase
    {
        [TestInitialize]
        public void SetUp()
        {
            MyTestInitialize();
        }

        [TestMethod]
        public void GetUserByIdentity_Found_NotFound()
        {
            IUserRepository userRepository = new UserRepository(NhibernateHelper.SessionFactory);
            Repository repository = new Repository(NhibernateHelper.SessionFactory);

            Customer customer = new Customer { Id = 1, Identification = "Ident", Email = "Test", Password = "Pass", FirstName = "Name", LastName = "Last", Code = "Code", PasswordSalt = "sss" };
            
            using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
            {
                repository.Save(customer);
                repository.Flush();
                UserIdentity user = userRepository.GetUserByIdentity(customer.Identification);
                UserIdentity user2 = userRepository.GetUserByIdentity("test");
                Assert.AreEqual(user.Email, customer.Email);
                Assert.AreEqual(user2, null);
            }
        }
    }
}
