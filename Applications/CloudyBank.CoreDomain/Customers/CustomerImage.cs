using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.CoreDomain.Customers
{
    public class CustomerImage
    {
        public virtual int Id { get; set; }
        public virtual byte[] Data { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
