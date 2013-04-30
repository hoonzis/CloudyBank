using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Dto;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;

namespace CloudyBank.Services.DtoCreators
{
    public class BusinessPartnerDtoCreator : IDtoCreator<BusinessPartner, BusinessPartnerDto>
    {

        public BusinessPartnerDto Create(BusinessPartner poco)
        {
            BusinessPartnerDto dto = new BusinessPartnerDto();
            if(poco.Customer!=null){
                dto.CustomerId = poco.Customer.Id;
            }
            dto.Title = poco.Name;
            dto.Iban = poco.Iban;
            dto.Description = poco.Description;
            dto.Id = poco.Id;
            return dto;
        }
    }
}
