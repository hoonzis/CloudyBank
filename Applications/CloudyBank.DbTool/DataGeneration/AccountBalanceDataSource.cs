using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    class AccountBalanceDataSource : IDatasource<Decimal>
    {
        Random random = new Random();
        public object Next(IGenerationSession session)
        {
            return (decimal)random.Next(-100000, 10000);
        }
    }
}
