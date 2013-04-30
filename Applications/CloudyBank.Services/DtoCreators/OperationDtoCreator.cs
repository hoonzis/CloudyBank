using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Dto;
using CloudyBank.Dto;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.Services.DtoCreators
{
    public class OperationDtoCreator : IDtoCreator<Operation,OperationDto>
    {
        public OperationDto Create(Operation operation)
        {
            OperationDto operationDto = new OperationDto();
            operationDto.Amount = operation.SignedAmount;
            operationDto.Id = operation.Id;
            operationDto.Motif = operation.Motif;

            if (operation.Tag != null)
            {
                operationDto.TagId = operation.Tag.Id;
                operationDto.TagName = operation.Tag.Name;
            }

            operationDto.TransactionCode = operation.TransactionCode;
            operationDto.OppositeIban = operationDto.OppositeIban;
            operationDto.Date = operation.Date;
            operationDto.Currency = operation.Currency;
            operationDto.Description = operation.Description;
            return operationDto;
        }
    }
}
