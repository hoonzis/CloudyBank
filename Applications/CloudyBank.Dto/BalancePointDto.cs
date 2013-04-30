using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Dto
{
    public class BalancePointDto
    {
        public int Id { get; set; }
        public Decimal Balance { get; set; }
        public DateTime Date { get; set; }
        public int AccountId { get; set; }
    }
}
