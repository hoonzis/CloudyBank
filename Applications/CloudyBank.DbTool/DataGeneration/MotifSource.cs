using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    class MotifSource : IDatasource<String>
    {
        int count;
        String[] descr = { "Payment for Internet", 
                            "Invoice for gas",
                            "Car insurance",
                            "House insurance",
                            "Invoice for electricity", 
                            "Donnation",
                            "Rent for flat",
                            "Invoice for phone",
                            "Transportation fees"

                         };

        public object Next(IGenerationSession session)
        {
            return descr[count++ % descr.Length];
        }
    }
}
