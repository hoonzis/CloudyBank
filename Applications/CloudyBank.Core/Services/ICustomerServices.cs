using System.Collections.Generic;
using CloudyBank.Dto;
using CloudyBank.CoreDomain.Bank;
using System.Diagnostics.Contracts;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.Core.Services
{
    [ContractClass(typeof(ICustomerServicesContracts))]
    public interface ICustomerServices
    {
        void CreateCustomer(string firstName, string lastName, string email, string phoneNumber);
        void CreateCustomer(string firstName, string lastName, string email, string phoneNumber,string code);
        IList<CustomerDto> GetCustomersForAdvisor(int advisorID);
        void SaveCustomer(CustomerDto customer);
        CustomerDto GetCustomerById(int id);
        CustomerDto GetCustomerByIdentity(string identity);
        CustomerDto GetCustomerByCode(string code);
        decimal CustomerBalance(Customer customer);
    }

    [ContractClassFor(typeof(ICustomerServices))]
    abstract class ICustomerServicesContracts : ICustomerServices
    {
        public void CreateCustomer(string firstName, string lastName, string email, string phoneNumber)
        {
            Contract.Requires<CustomerServicesException>(!string.IsNullOrEmpty(firstName));
            Contract.Requires<CustomerServicesException>(!string.IsNullOrEmpty(lastName));
            Contract.Requires<CustomerServicesException>(!string.IsNullOrEmpty(email));
            Contract.Requires<CustomerServicesException>(!string.IsNullOrEmpty(phoneNumber));
        }

        public IList<CustomerDto> GetCustomersForAdvisor(int advisorID)
        {
            throw new System.NotImplementedException();
        }

        public void SaveCustomer(CustomerDto customer)
        {
            Contract.Requires<CustomerServicesException>(customer != null);           
            Contract.Requires<CustomerServicesException>(customer.Email != null);
        }

        public CustomerDto GetCustomerById(int id)
        {
            throw new System.NotImplementedException();
        }

        public CustomerDto GetCustomerByIdentity(string identity)
        {
            Contract.Requires<CustomerServicesException>(identity != null);
            return default(CustomerDto);
        }

        public CustomerDto GetCustomerByCode(string code)
        {
            Contract.Requires<CustomerServicesException>(code != null);
            return default(CustomerDto);
        }

        public decimal CustomerBalance(Customer customer){
            Contract.Requires<CustomerServicesException>(customer != null);
            return default(decimal);
        }

        public void CreateCustomer(string firstName, string lastName, string email, string phoneNumber, string code)
        {
            Contract.Requires<CustomerServicesException>(!string.IsNullOrEmpty(firstName));
            Contract.Requires<CustomerServicesException>(!string.IsNullOrEmpty(lastName));
            Contract.Requires<CustomerServicesException>(!string.IsNullOrEmpty(email));
            Contract.Requires<CustomerServicesException>(!string.IsNullOrEmpty(phoneNumber));
            Contract.Requires<CustomerServicesException>(!string.IsNullOrEmpty(code));
            
        }
    }
}
