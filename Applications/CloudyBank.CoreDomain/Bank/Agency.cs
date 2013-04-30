using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Bank
{
    public class Agency
    {
        public virtual int Id { get; set; }
        public virtual double Lat { get; set; }
        public virtual double Lng { get; set; }
        public virtual String Address { get; set; }
        public virtual DateTime? OpeningHour { get; set; }
        public virtual DateTime? ClosingHour { get; set; }
        
    }
}
