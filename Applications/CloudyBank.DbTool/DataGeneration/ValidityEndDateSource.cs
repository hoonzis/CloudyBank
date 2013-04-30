using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    class ValidityEndDateSource : IDatasource<DateTime>
    {

        public object Next(IGenerationSession session)
        {
            return DateTime.Now.AddDays(100);
        }
    }
}
