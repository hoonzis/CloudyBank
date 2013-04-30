using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    
    public class OperationAmountSource : IDatasource<Decimal>
    {
        Random random = new Random();
        public object Next(IGenerationSession session)
        {
            return (decimal)random.Next(1000);
        }
    }
}
