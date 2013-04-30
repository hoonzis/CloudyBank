using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    class ValidityStartDateSource : IDatasource<DateTime>
    {
        public object Next(IGenerationSession session)
        {
            return DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0));
        }
    }
}
