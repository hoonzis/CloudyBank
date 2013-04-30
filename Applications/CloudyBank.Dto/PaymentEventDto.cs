using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Dto
{
    public class PaymentEventDto
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public DateTime Date { get; set; }
        public String Description { get; set; }
        public int AccountId { get; set; }
        public int PartnerId { get; set; }
        public String PartnerName { get; set; }
        public String PartnerIban { get; set; }
        public bool Payed { get; set; }
        public decimal Amount { get; set; }
        public bool Regular { get; set; }
    }
}
