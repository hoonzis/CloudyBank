using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;
using CloudyBank.Services.DataGeneration;

namespace CloudyBank.DbTool.DataGeneration
{
    public class AccountNumberSource :IDatasource<String>
    {
        public object Next(IGenerationSession session)
        {
            return GenerationUtils.RandomNumberString(11);
        }
    }
}
