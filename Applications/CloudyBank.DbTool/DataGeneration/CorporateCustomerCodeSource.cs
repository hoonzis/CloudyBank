using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;
using CloudyBank.Services.DataGeneration;

namespace CloudyBank.DbTool.DataGeneration
{
    class CorporateCustomerCodeSource : IDatasource<String>
    {
        public object Next(IGenerationSession session)
        {
            return "C1-" + GenerationUtils.RandomString(4) + "-" + GenerationUtils.RandomString(4);
        }
    }
}
