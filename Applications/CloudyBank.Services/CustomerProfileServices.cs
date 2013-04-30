using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;
using CloudyBank.Core.DataAccess;
using CloudyBank.Core.Dto;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;

namespace CloudyBank.Services
{
    public class CustomerProfileServices : ICustomerProfileServices
    {
        private IRepository _repository;
        private IDtoCreator<CustomerProfile, CustomerProfileDto> _dtoCreator;

        public CustomerProfileServices(IRepository repository, IDtoCreator<CustomerProfile, CustomerProfileDto> dtoCreator)
        {
            _repository = repository;
            _dtoCreator = dtoCreator;
        }

        public IList<CustomerProfileDto> GetAllCustomerProfiles()
        {
            var profiles = _repository.GetAll<CustomerProfile>();
            if (profiles != null)
            {
                return profiles.Select(x => _dtoCreator.Create(x)).ToList();
            }
            return null;
        }
    }
}
