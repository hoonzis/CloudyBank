using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Dto;
using CloudyBank.Dto;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.Core.Services;

namespace CloudyBank.Services.DtoCreators
{
    public class CustomerDtoCreator : IDtoCreator<Customer, CustomerDto>
    {
        public CustomerDto Create(Customer customer)
        {
            CustomerDto customerDto = new CustomerDto();
            customerDto.Title = String.Format("{0} {1}", customer.FirstName, customer.LastName);
            customerDto.Code = customer.Code;
            customerDto.Email = customer.Email;
            customerDto.Id = customer.Id;
            customerDto.PhoneNumber = customer.PhoneNumber;
            customerDto.Identification = customer.Identification;
            customerDto.Situation = customer.Situation;
            customerDto.FirstName = customer.FirstName;
            customerDto.LastName = customer.LastName;
            customerDto.BirthDate = customer.BirthDate;
            return customerDto;

        }
    }
}
