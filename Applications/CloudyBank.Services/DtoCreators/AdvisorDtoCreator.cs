using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Dto;
using CloudyBank.Core.Services;
using CloudyBank.Dto;
using CloudyBank.CoreDomain.Advisors;

namespace CloudyBank.Services.DtoCreators
{
    public class AdvisorDtoCreator : IDtoCreator<Advisor, AdvisorDto>
    {
        public AdvisorDto Create(Advisor advisor)
        {
            AdvisorDto advisorDto = new AdvisorDto();
            advisorDto.Id = advisor.Id;
            advisorDto.Identification = advisor.Identification;
            advisorDto.LastName = advisor.LastName;
            advisorDto.FirstName = advisor.FirstName;
            return advisorDto;
        }
    }
}
