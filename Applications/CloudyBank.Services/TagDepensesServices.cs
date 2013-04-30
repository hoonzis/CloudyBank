using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core;
using CloudyBank.Dto;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Core.DataAccess;
using CloudyBank.Core.Dto;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.Services
{
    public class TagDepensesServices : ITagDepensesServices
    {
        private IRepository _repository;
        private IDtoCreator<TagDepenses, TagDepensesDto> _dtoCreator;

        public TagDepensesServices(IRepository repository, IDtoCreator<TagDepenses, TagDepensesDto> dtoCreator)
        {
            _repository = repository;
            _dtoCreator = dtoCreator;
        }

        public IList<TagDepensesDto> GetTagDepensesForProfile(int profileId)
        {
            CustomerProfile profile = _repository.Load<CustomerProfile>(profileId);
            if (profile.TagsRepartition != null)
            {
                return profile.TagsRepartition.Select(x => _dtoCreator.Create(x)).ToList();
            }
            return null;
        }

        public IList<TagDepensesDto> GetTagDepensesForCustomer(int customerId)
        {
            Customer customer = _repository.Load<Customer>(customerId);
            if (customer.TagDepenses != null)
            {
                return customer.TagDepenses.Select(x => _dtoCreator.Create(x)).ToList();
            }
            return null;
        }
    }
}
