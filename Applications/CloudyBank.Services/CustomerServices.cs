using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using CloudyBank.Core.DataAccess;

using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Advisors;
using CloudyBank.Core.Dto;
using CloudyBank.Services.Bank;
using CloudyBank.CoreDomain.Security;
using System.Diagnostics.Contracts;
using CloudyBank.Services.Technical;
using CloudyBank.Services.DataGeneration;


namespace CloudyBank.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IRepository _repository;
        private readonly IAccountServices _accountServices;
        private readonly IDtoCreator<Customer,CustomerDto> _customerDtoCreator;
        

        public CustomerServices(ICustomerRepository customerRepository, IRepository repository, IAccountServices accountServices,
           IDtoCreator<Customer, CustomerDto> customerDtoCreator)
        {
            _customerRepository = customerRepository;
            _repository = repository;
            _accountServices = accountServices;
            _customerDtoCreator = customerDtoCreator;
        }

        public Decimal CustomerBalance(Customer customer)
        {
            if (customer.RelatedAccounts == null)
            {
                return 0;
            }

            return customer.RelatedAccounts.Sum(x =>_accountServices.Balance(x.Key));
        }

        public void CreateCustomer(string firstName, string lastName, string email, string phoneNumber, string code)
        {
            Customer customer = new Customer();
            customer.Code = code;
            customer.Email = email;
            customer.FirstName = firstName;
            customer.LastName = lastName;
            customer.PhoneNumber = phoneNumber;
            customer.Password = GenerationUtils.RandomString(8);
            customer.PasswordSalt = GenerationUtils.RandomString(5);

            using (TransactionScope scope = new TransactionScope())
            {
                _repository.Save<Customer>(customer);
                _repository.Flush();
                scope.Complete();
            }
        }

        public void CreateCustomer(String firstName, String lastName, String email, String phoneNumber)
        {
            var code  = BankMethods.GenerateCustomerCode(UserType.IndividualCustomer);
            CreateCustomer(firstName, lastName, email, phoneNumber, code);
        }

        public IList<CustomerDto> GetCustomersForAdvisor(int advisorID)
        {
            Advisor advisor = _repository.Get<Advisor>(advisorID);
            if (advisor == null)
            {
                return null;
            }

            if (advisor.Customers != null)
            {
                return advisor.Customers.Select(_customerDtoCreator.Create).ToList();
            }
            return null;
        }

        public void SaveCustomer(CustomerDto customerDto)
        {
            Customer customer = _repository.Load<Customer>(customerDto.Id);
            
            if (customer == null)
            {
                throw new CustomerServicesException(String.Format("Could not find customer in the DB: {0}", customerDto.Id));
            }

            Update(ref customer, customerDto);

            using (TransactionScope scope = new TransactionScope())
            {
                _repository.Update<Customer>(customer);
                _repository.Flush();
                scope.Complete();
            }
        }

        public CustomerDto GetCustomerById(int id)
        {
            var customer = _repository.Get<Customer>(id);

            if (customer == null)
            {
                return null;
            }
            return _customerDtoCreator.Create(customer);

        }

        public CustomerDto GetCustomerByIdentity(string identity)
        {
            Customer customer = _customerRepository.GetCustomerByIdentity(identity);
            if (customer != null)
            {
                return _customerDtoCreator.Create(customer);
            }
            return null;
        }

        public CustomerDto GetCustomerByCode(string code)
        {
            Customer customer = _customerRepository.GetCustomerByCode(code);
            if (customer != null)
            {
                return _customerDtoCreator.Create(customer);
            }
            return null;
        }

        
        
        public void Update(ref Customer customer, CustomerDto customerDto)
        {
            customer.Email = customerDto.Email;
            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.PhoneNumber = customerDto.PhoneNumber;
            customer.Situation = customerDto.Situation;
        }
       
    }
}
