using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Dto
{
    public class BusinessPartnerDto
    {
        public int Id { get; set; }
        public String Iban { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public int CustomerId { get; set; }
    }
}
