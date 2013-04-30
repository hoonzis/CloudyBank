using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Dto;
using System.Diagnostics.Contracts;

namespace CloudyBank.Core
{
    [ContractClass(typeof(TagDepensesServicesContracts))]
    public interface ITagDepensesServices
    {
        IList<TagDepensesDto> GetTagDepensesForProfile(int profileId);

        IList<TagDepensesDto> GetTagDepensesForCustomer(int customerId);
    }

    [ContractClassFor(typeof(ITagDepensesServices))]
    public abstract class TagDepensesServicesContracts : ITagDepensesServices
    {

        public IList<TagDepensesDto> GetTagDepensesForProfile(int profileId)
        {
            throw new NotImplementedException();
        }

        public IList<TagDepensesDto> GetTagDepensesForCustomer(int customerId)
        {
            throw new NotImplementedException();
        }
    }
}
