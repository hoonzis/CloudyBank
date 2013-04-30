using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Dto
{
    public class AgencyDto
    {
        public int Id { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public String Address { get; set; }
        public DateTime OpeningHour { get; set; }
        public DateTime ClosingHour { get; set; }
    }
}
