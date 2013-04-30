using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using System.Diagnostics.Contracts;
using CloudyBank.Dto;

namespace CloudyBank.Core.Services
{
    [ContractClass(typeof(CustomerProfileServiceContracts))]
    public interface ICustomerProfileServices
    {
        IList<CustomerProfileDto> GetAllCustomerProfiles();
    }

    [ContractClassFor(typeof(ICustomerProfileServices))]
    public abstract class CustomerProfileServiceContracts : ICustomerProfileServices
    {

        public IList<CustomerProfileDto> GetAllCustomerProfiles()
        {
            throw new NotImplementedException();
        }
    }
}
