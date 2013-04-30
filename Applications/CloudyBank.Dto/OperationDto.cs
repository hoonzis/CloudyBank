using System;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.Dto
{
    public class OperationDto
    {
        public int Id { get; set; }
        public Decimal Amount { get; set; }
        public String Direction { get; set; }
        public String Motif { get; set; }
        public String TagName { get; set; }
        public DateTime Date { get; set; }
        public int TagId { get; set; }
        public String OppositeIban { get; set; }
        public String TransactionCode { get; set; }
        public String Currency { get; set; }
        public String Description { get; set; }

        public OperationDto()
        {

        }
    }
}
