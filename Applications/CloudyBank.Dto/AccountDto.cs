using System;
using System.Runtime.Serialization;

namespace CloudyBank.Dto
{
    public class AccountDto
    {
        public int Id { get; set; }
        public Decimal Balance { get; set; }
        public DateTime? BalanceDate { get; set; }
        public String Number { get; set; }
        public String Title { get; set; }
        public String Iban { get; set; }
        public String Currency { get; set; }
        public bool AuthorizeOverdraft { get; set; }

        public AccountDto()
        {

        }
    }
}
