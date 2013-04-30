using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;

namespace CloudyBank.Services
{
    public class AgencyServices : IAgencyServices
    {
        IRepository _repository;

        public AgencyServices(IRepository repository)
        {
            _repository = repository;
        }

        public IList<AgencyDto> GetAcencies()
        {
            var agencies = _repository.GetAll<Agency>();
            if (agencies != null)
            {
                return agencies.Select(x => ToDto(x)).ToList();
            }
            return null;
        }

        public AgencyDto ToDto(Agency agency)
        {
            AgencyDto dto = new AgencyDto();
            dto.Address = agency.Address;
            dto.Id = agency.Id;
            dto.Lat = agency.Lat;
            dto.Lng = agency.Lng;
            dto.OpeningHour = (DateTime)agency.OpeningHour;
            dto.ClosingHour = (DateTime)agency.ClosingHour;

            return dto;
        }
    }
}
