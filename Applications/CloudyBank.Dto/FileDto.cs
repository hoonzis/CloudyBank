using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Dto
{
    public class FileDto
    {
        public String Url { get; set; }
        public String Author { get; set; }
        public DateTime LastModified { get; set; }
        public String ContentType { get; set; }
    }
}
