using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Dto;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;

namespace CloudyBank.Services.DtoCreators
{
    public class PaymentEventDtoCreator : IDtoCreator<PaymentEvent, PaymentEventDto>
    {
        public PaymentEventDto Create(PaymentEvent poco)
        {
            PaymentEventDto dto = new PaymentEventDto();
            dto.Title = poco.Name;
            dto.Description = poco.Description;
            if (poco.Partner != null)
            {
                dto.PartnerIban = poco.Partner.Iban;
                dto.PartnerName = poco.Partner.Name;
                dto.PartnerId = poco.Partner.Id;
            }
            else
            {
                dto.PartnerIban = poco.PartnerIban;
            }
            dto.Payed = poco.Payed;
            dto.Date = poco.Date.Date;
            dto.AccountId = poco.Account.Id;
            dto.Id = poco.Id;
            dto.Amount = poco.Amount;
            return dto;
        }
    }
}
