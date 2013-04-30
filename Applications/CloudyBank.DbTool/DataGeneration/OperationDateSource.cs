using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    class OperationDateSource : IDatasource<DateTime>
    {
        Random rand = new Random();
        public object Next(IGenerationSession session)
        {
            int days = rand.Next(40);
            return DateTime.Now.Subtract(new TimeSpan(days,0,0,0,0));
        }
    }
}
