using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Dto
{
    public class TokenDto
    {
        public int Id { get; set; }
        public String ApplicationName { get; set; }
        public String Scope { get; set; }
        public String ApplicationDescription { get; set; }
    }
}
