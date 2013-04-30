using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;

using CloudyBank.Core.DataAccess;
using CloudyBank.Dto;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Advisors;
using CloudyBank.Core.Dto;


namespace CloudyBank.Services
{
    public class AdvisorServices : IAdvisorServices
    {
        IRepository _repository;
        IAdvisorRepository _advisorRepository;
        IDtoCreator<Advisor, AdvisorDto> _advisorDtoCreator;

        public AdvisorServices(IRepository repository, IAdvisorRepository advisorRepository, IDtoCreator<Advisor, AdvisorDto> advisorDtoCreator)
        {
            _repository = repository;
            _advisorRepository = advisorRepository;
            _advisorDtoCreator = advisorDtoCreator;
        }

        public AdvisorDto GetAdvisorById(int id)
        {
            Advisor advisor = _repository.Get<Advisor>(id);
            if(advisor!=null){
                return _advisorDtoCreator.Create(advisor);
            }
            return null;
        }

        public AdvisorDto GetAdvisorByIdentity(string identity)
        {
            if (identity == null)
            {
                throw new AdvisorServicesException("Provided identity is null");
            }

            Advisor advisor = _advisorRepository.GetAdvisorByIdentity(identity);
            if (advisor != null)
            {
                return _advisorDtoCreator.Create(advisor);
            }
            return null;
        }
    }
}
