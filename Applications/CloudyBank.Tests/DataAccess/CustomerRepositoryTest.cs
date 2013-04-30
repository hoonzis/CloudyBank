using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Core.DataAccess;
using CloudyBank.DataAccess.Repository;
using CloudyBank.UnitTests.TestHelper;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;


namespace CloudyBank.UnitTests.DataAccess
{
    [TestClass]
    public class CustomerRepositoryTest : DataAccessTestBase
    {
        
        [TestInitialize]
        public void SetUp()
        {
            MyTestInitialize();
        }

        [TestMethod]
        public void GetIndividualCustomerByCode_NotFound()
        {
            ICustomerRepository thirdPartyRepository = new CustomerRepository(NhibernateHelper.SessionFactory);
            Repository repository = new Repository(NhibernateHelper.SessionFactory);

            Customer individualThirdParty1 = new Customer { Code = "tjdsklfs", Email = "youpala@youpi.com", FirstName = "Sim", LastName = "Lehericey", Password = "Toto", PasswordSalt = "sss" };

            using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
            {
                repository.Save(individualThirdParty1);
                repository.Flush();

                Customer individualThirdPartyRetrieved = thirdPartyRepository.GetCustomerByCode("a");
                Assert.IsNull(individualThirdPartyRetrieved);
            }
        }

        [TestMethod]
        public void GetIndividualCustomerByCode_Found()
        {
            ICustomerRepository customerRepository = new CustomerRepository(NhibernateHelper.SessionFactory);
            Repository repository = new Repository(NhibernateHelper.SessionFactory);

            Customer customer = new Customer { Code = "tjdsklfs", Email = "youpala@youpi.com", FirstName = "Sim", LastName = "Lehericey", Password = "Toto", PasswordSalt = "sss" };
            Account account1 = new Account { Balance = 201, BalanceDate = DateTime.Now, Number = "dsf1", Iban="1234"};

            Customer retrievedCustomer;

            using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
            {
                repository.Save(customer);
                repository.Save(account1);
                repository.Flush();

                retrievedCustomer = customerRepository.GetCustomerByCode(customer.Code);
                Assert.IsNotNull(retrievedCustomer);
            }
        }

        [TestMethod]
        public void FindIndividualCustomersByName()
        {
            ICustomerRepository customerRepository = new CustomerRepository(NhibernateHelper.SessionFactory);
            Repository repository = new Repository(NhibernateHelper.SessionFactory);

            Customer customer = new Customer { Code = "tjdsklfs", Email = "youpala@youpi.com", FirstName = "Sim", LastName = "Lehericey", Password = "Toto", PasswordSalt = "sss" };
            
            IList<Customer> customers1;
            IList<Customer> customers2;
            IList<Customer> customers3;

            using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
            {
                repository.Save(customer);
                //repository.Save(account1);
                repository.Flush();

                String[] names1 = { customer.FirstName };
                String[] names2 = { customer.LastName };
                String[] names3 = { customer.FirstName, customer.LastName };

                customers1 = customerRepository.FindCustomersByName(names1);
                customers2 = customerRepository.FindCustomersByName(names2);
                customers3 = customerRepository.FindCustomersByName(names3);

                Assert.IsNotNull(customers1);
                Assert.IsNotNull(customers2);
                Assert.IsNotNull(customers3);

            }
            Assert.AreEqual("youpala@youpi.com", customers1[0].Email);
            Assert.AreEqual("youpala@youpi.com", customers2[0].Email);
            Assert.AreEqual("youpala@youpi.com", customers3[0].Email);
        }

        [TestMethod]
        public void GetIndividualCustomerByIdentity_Found_NotFound()
        {
            ICustomerRepository customerRepository = new CustomerRepository(NhibernateHelper.SessionFactory);
            Repository repository = new Repository(NhibernateHelper.SessionFactory);

            Customer customer = new Customer { Code = "tjdsklfs", Email = "e@e.com", FirstName = "Sim", LastName = "Lehericey", Password = "Toto", Identification = "Ident", PasswordSalt = "sss" };

            Customer foundCustomer;
            Customer notFoundCustomer;

            using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
            {
                repository.Save(customer);
                repository.Flush();

                foundCustomer = customerRepository.GetCustomerByIdentity(customer.Identification);
                notFoundCustomer = customerRepository.GetCustomerByIdentity("a");
            }
            Assert.IsNull(notFoundCustomer);
            Assert.IsNotNull(foundCustomer);
            Assert.AreEqual("e@e.com", foundCustomer.Email);
        }

        [TestMethod]
        public void GetCustomerByIdentity_Found_NotFound()
        {
            ICustomerRepository customerRepository = new CustomerRepository(NhibernateHelper.SessionFactory);
            Repository repository = new Repository(NhibernateHelper.SessionFactory);

            Customer customer = new Customer { Code = "tjdsklfs", Email = "e@e.com", FirstName = "Sim", LastName = "Lehericey", Password = "Toto", Identification = "Ident", PasswordSalt = "sss" };

            Customer foundCustomer;
            Customer notFoundCustomer;

            using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
            {
                repository.Save(customer);
                repository.Flush();

                foundCustomer = customerRepository.GetCustomerByIdentity(customer.Identification);
                notFoundCustomer = customerRepository.GetCustomerByIdentity("a");
            }
            Assert.IsNull(notFoundCustomer);
            Assert.IsNotNull(foundCustomer);
            Assert.AreEqual("e@e.com", foundCustomer.Email);
        }

        [TestMethod]
        public void GetCustomerByCode_Found_NotFound()
        {
            ICustomerRepository customerRepository = new CustomerRepository(NhibernateHelper.SessionFactory);
            Repository repository = new Repository(NhibernateHelper.SessionFactory);

            Customer customer = new Customer { Code = "someCode", Email = "e@e.com", FirstName = "Sim", LastName = "Lehericey", Password = "Toto", Identification = "Ident", PasswordSalt = "sss" };

            Customer foundCustomer;
            Customer notFoundCustomer;

            using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
            {
                repository.Save(customer);
                repository.Flush();

                foundCustomer = customerRepository.GetCustomerByCode(customer.Code);
                notFoundCustomer = customerRepository.GetCustomerByCode("a");
            }
            Assert.IsNull(notFoundCustomer);
            Assert.IsNotNull(foundCustomer);
            Assert.AreEqual("e@e.com", foundCustomer.Email);
        }

    }
}
