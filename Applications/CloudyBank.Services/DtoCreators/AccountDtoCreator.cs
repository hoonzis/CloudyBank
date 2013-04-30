using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Dto;
using CloudyBank.Dto;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Core.Services;
using System.Diagnostics.Contracts;

namespace CloudyBank.Services.DtoCreators
{
    public class AccountDtoCreator : IDtoCreator<Account, AccountDto>
    {
        public AccountDto Create(Account account)
        {
            AccountDto accountDto = new AccountDto();
            accountDto.Title = account.Name;
            accountDto.Balance = account.Balance;
            accountDto.Id = account.Id;
            accountDto.Number = account.Number;
            accountDto.Iban = account.Iban;
            accountDto.Currency = account.Currency;
            accountDto.AuthorizeOverdraft = account.AuthorizeOverdraft;
            return accountDto;
        }
    }
}
