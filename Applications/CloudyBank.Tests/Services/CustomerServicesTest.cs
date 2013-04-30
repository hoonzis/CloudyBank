using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Core.DataAccess;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using CloudyBank.Services;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Advisors;
using CloudyBank.CoreDomain.Security;
using CloudyBank.Services.DtoCreators;
using CloudyBank.Core.Dto;



namespace CloudyBank.UnitTests.Services
{
    [TestClass]
    public class CustomerServicesTest
    {

        [TestMethod]
        public void Balance_CustomerOK()
        {
            //arrange
            Account account = new Account();
            Account account2 = new Account();
            Customer customer = new Customer();

            customer.RelatedAccounts.Add(account, new Role());
            customer.RelatedAccounts.Add(account2, new Role());

            IRepository repository = MockRepository.GenerateMock<IRepository>();
            ICustomerRepository customerRepository = MockRepository.GenerateMock<ICustomerRepository>();
            IAccountRepository accountRepository = MockRepository.GenerateMock<IAccountRepository>();
            IDtoCreator<Customer, CustomerDto> custCreator = new CustomerDtoCreator();
            

            IAccountServices accountServices = MockRepository.GenerateMock<IAccountServices>(); //(repository, accountRepository,customerRepository);
            accountServices.Expect(x => x.Balance(account2)).Return(-10);
            accountServices.Expect(x => x.Balance(account)).Return(-20);

            
            //act
            CustomerServices services = new CustomerServices(customerRepository, repository, accountServices, custCreator);
            decimal balance = services.CustomerBalance(customer);

            //assert
            Assert.AreEqual(balance, -30);
            accountServices.VerifyAllExpectations();
        }        


        #region CreateIndividualCustomer Tests

        [TestMethod]
        public void CreateIndividualCustomer_CustomerDtoOk()
        {
            //arrange
            ICustomerRepository thirdPartyRepository = MockRepository.GenerateMock<ICustomerRepository>();
            IRepository repository = MockRepository.GenerateMock<IRepository>();
            IAccountServices accountServices = MockRepository.GenerateStub<IAccountServices>();
            IDtoCreator<Customer, CustomerDto> custCreator = new CustomerDtoCreator();
            
            //act
            CustomerServices services = new CustomerServices(thirdPartyRepository, repository,accountServices,custCreator);
            services.CreateCustomer("toto", "titi", "email","123");

            //assert
            repository.AssertWasCalled(x => x.Save<Customer>(Arg<Customer>.Is.NotNull));
            repository.AssertWasCalled(x => x.Flush());
        }
        #endregion

        #region GetCustomersForAdvisor

        [TestMethod]
        public void TestGetCustomersForAdvisor()
        {
            //arrange
            IRepository repository = MockRepository.GenerateStub<IRepository>();
            ICustomerRepository customerRepository = MockRepository.GenerateStub<ICustomerRepository>();
            IAccountServices accountServices = MockRepository.GenerateStub<IAccountServices>();
            IDtoCreator<Customer, CustomerDto> custCreator = new CustomerDtoCreator();
            
            Advisor advisor1 = new Advisor { Id = 1, FirstName = "Ad1"};
            Advisor advisor2 = new Advisor {Id=2,FirstName = "Ad2"};
            List<Customer> customers = new List<Customer>();
            customers.Add(new Customer { Advisor = advisor1, Id = 1});
            customers.Add(new Customer {Advisor = advisor2, Id = 2});
            advisor1.Customers = customers;
            //repository.Expect(x=>x.GetAll<Customer>()).Return(customers);
            repository.Expect(x => x.Get<Advisor>(advisor1.Id)).Return(advisor1);
            
            //act
            CustomerServices services = new CustomerServices(customerRepository, repository,accountServices,custCreator);
            List<CustomerDto> recieved = (List<CustomerDto>)services.GetCustomersForAdvisor(advisor1.Id);
            
            //assert
            Assert.AreEqual(recieved[0].Id, 1);
            repository.VerifyAllExpectations();
        }
        #endregion
    }
}
