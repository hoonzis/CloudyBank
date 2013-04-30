using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using System.Diagnostics.Contracts;

namespace CloudyBank.Core.Services
{
    [ContractClass(typeof(IAdvisorServicesContracts))]
    public interface IAdvisorServices
    {
        AdvisorDto GetAdvisorById(int id);

        AdvisorDto GetAdvisorByIdentity(String identity);
    }

    [ContractClassFor(typeof(IAdvisorServices))]
    abstract class IAdvisorServicesContracts : IAdvisorServices
    {

        public AdvisorDto GetAdvisorById(int id)
        {
            throw new NotImplementedException();
        }

        public AdvisorDto GetAdvisorByIdentity(string identity)
        {
            Contract.Requires<AdvisorServicesException>(identity!=null);
            return default(AdvisorDto);
        }
    }
}
