using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CloudyBank.CoreDomain.Advisors
{
    public class Task
    {
        public virtual int Id { get; set; }
        public virtual String Title { get; set; }
        public virtual String Descritpion { get; set; }
        public virtual Advisor Advisor { get; set; }
    }
}
