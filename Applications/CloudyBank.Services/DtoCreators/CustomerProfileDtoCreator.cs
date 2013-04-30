using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using CloudyBank.Core.Dto;

namespace CloudyBank.Services.DtoCreators
{
    class CustomerProfileDtoCreator : IDtoCreator<CustomerProfile, CustomerProfileDto>
    {
        public CustomerProfileDto Create(CustomerProfile poco)
        {
            CustomerProfileDto profile = new CustomerProfileDto();
            profile.HighAge = poco.HighAge;
            profile.LowAge = poco.LowAge;
            profile.Id = poco.Id;
            profile.Situation = poco.Situation;

            return profile;
        }
    }
}
