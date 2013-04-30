using System.Collections.Generic;
using System.Linq;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Core.DataAccess.Moles;
using CloudyBank.Core.Dto;
using CloudyBank.Core.Dto.Moles;
using CloudyBank.Core.Services;
using CloudyBank.Core.Services.Moles;
using CloudyBank.CoreDomain.Advisors;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Security;
using CloudyBank.Dto;
using CloudyBank.Services.DtoCreators;
using CloudyBank.UnitTests.Data;
using System;

namespace CloudyBank.Services
{
    [TestClass]
    [PexClass(typeof(CustomerServices))]
    public partial class CustomerServicesTest
    {
        private IList<Advisor> _advisors;
        private IList<Customer> _customers;
        
        public CustomerServicesTest()
        {
            _advisors = DataHelper.GetAdvisors();
            _customers = DataHelper.GetCustomers();

        }


        [PexMethod, PexAllowedException(typeof(CustomerServicesException))]
        public void SaveCustomer(CustomerDto customerDto)
        {
            SIRepository repository = new SIRepository();
            repository.GetObject((x) => { return new Customer(); });
            SIAccountServices accountServices = new SIAccountServices();
            IDtoCreator<Customer, CustomerDto> custCreator = new CustomerDtoCreator();

            SICustomerRepository customerRepository = new SICustomerRepository();
            CustomerServices services = new CustomerServices(customerRepository, repository, accountServices, custCreator);
            services.SaveCustomer(customerDto);
        }


        [PexMethod]
        public IList<CustomerDto> GetCustomersForAdvisor(int advisorID)
        {
            SIRepository repository = new SIRepository();
            SICustomerRepository customerRepository = new SICustomerRepository();
            SIAccountServices accountServices = new SIAccountServices();
            IDtoCreator<Customer, CustomerDto> custCreator = new CustomerDtoCreator();

            
            repository.GetObject<Advisor>((x) => _advisors.SingleOrDefault(a => a.Id == (int)x));

            //act
            CustomerServices customerServices = new CustomerServices(customerRepository, repository,accountServices,custCreator);
            IList<CustomerDto> result = customerServices.GetCustomersForAdvisor(advisorID);

            PexAssert.Case(advisorID == _advisors[0].Id).Implies(() => result.Count == _advisors[0].Customers.Count);
            return result;
        }

        [PexMethod, PexAllowedException(typeof(CustomerServicesException))]
        public CustomerDto GetCustomerByIdentity(string identity)
        {
            //arrange
            SIRepository repository = new SIRepository();
            SICustomerRepository customerRepository = new SICustomerRepository();
            SIAccountServices accountServices = new SIAccountServices();
            IDtoCreator<Customer, CustomerDto> custCreator = new CustomerDtoCreator();

            
            customerRepository.GetCustomerByIdentityString = (x) => _customers.SingleOrDefault(c=>c.Identification == x);
            
            //act
            CustomerServices services = new CustomerServices(customerRepository, repository,accountServices,custCreator);
            var result = services.GetCustomerByIdentity(identity);

            //assert
            PexAssert.Case(identity == _customers[0].Identification).Implies(()=>result.Email == _customers[0].Email);
            return result;
        }

        [PexMethod]
        public CustomerDto GetCustomerById(int id)
        {
            var repository = new SIRepository();
            var customerRepository = new SICustomerRepository();
            var accountServices = new SIAccountServices();
            var custCreator = new CustomerDtoCreator();

            repository.GetObject<Customer>((x) => _customers.SingleOrDefault(c=>c.Id == (int)x));

            CustomerServices services = new CustomerServices(customerRepository, repository,accountServices,custCreator);
            var result = services.GetCustomerById(id);
            //asert

            PexAssert.Case(id == _customers[0].Id).Implies(() => result.Email == _customers[0].Email);            
            return result;
        }

        [PexMethod, PexAllowedException(typeof(CustomerServicesException))]
        public decimal CustomerBalance(Customer customer)
        {
            //assume
            Dictionary<Account, Role> accounts = new Dictionary<Account, Role>();
            accounts.Add(new Account { Id = 1, Balance = 100 }, new Role());
            accounts.Add(new Account { Id = 2, Balance = -200 }, new Role());

            PexAssume.Implies(customer != null, () => customer.RelatedAccounts = accounts);
            
            //arrange
            SIRepository repository = new SIRepository();
            SICustomerRepository customerRepository = new SICustomerRepository();
            SIAccountServices accountServices = new SIAccountServices();
            IDtoCreator<Customer,CustomerDto> custCreator = new CustomerDtoCreator();

            accountServices.BalanceAccount = (x) => x.Balance;

            CustomerServices services = new CustomerServices(customerRepository, repository, accountServices,custCreator);
            var result = services.CustomerBalance(customer);
            PexAssert.Case(customer.RelatedAccounts == accounts).Implies(() => result == -100);
            return result;
        }

        [PexMethod, PexAllowedException(typeof(CustomerServicesException))]
        public void CreateIndividualCustomer(string firstName, string lastName, string email, string phoneNumber)
        {
            SIRepository repository = new SIRepository();
            SICustomerRepository customerRepository = new SICustomerRepository();
            SIAccountServices accountServices = new SIAccountServices();
            IDtoCreator<Customer, CustomerDto> custCreator = new CustomerDtoCreator();

            CustomerServices customerServices = new CustomerServices(customerRepository, repository,accountServices,custCreator);
            customerServices.CreateCustomer( firstName,  lastName,  email, phoneNumber);
        }

        [PexMethod, PexAllowedException(typeof(CustomerServicesException))]
        public CustomerDto GetCustomerByCode(string code)
        {
            //arrange
            SIRepository repository = new SIRepository();
            SICustomerRepository customerRepository = new SICustomerRepository();
            SIAccountServices accountServices = new SIAccountServices();
            IDtoCreator<Customer, CustomerDto> custCreator = new CustomerDtoCreator();

            customerRepository.GetCustomerByCodeString = (x) => _customers.SingleOrDefault(a => a.Code == x);
            //act
            CustomerServices custonerServices = new CustomerServices(customerRepository, repository,accountServices,custCreator);
            CustomerDto result = custonerServices.GetCustomerByCode(code);

            //assert
            PexAssert.Case(code == _customers[0].Code).Implies(()=>result.Email == _customers[0].Email);
            return result;
        }
    }
}
