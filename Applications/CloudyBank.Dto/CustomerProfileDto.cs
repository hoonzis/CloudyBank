using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.Dto
{
    public class CustomerProfileDto
    {
        public int Id { get; set; }
        public int LowAge { get; set; }
        public int HighAge { get; set; }
        public FamilySituation Situation { get; set; }
    }
}
