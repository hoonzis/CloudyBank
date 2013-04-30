using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;
using CloudyBank.Services.DataGeneration;

namespace CloudyBank.DbTool.DataGeneration
{
    public class PhoneNumberSource : IDatasource<String>
    {
        public object Next(IGenerationSession session)
        {
            return GenerationUtils.RandomNumberString(3) + "-" + GenerationUtils.RandomNumberString(4) + "-" + GenerationUtils.RandomNumberString(4);
        }
    }
}
