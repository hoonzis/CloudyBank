using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    class RandomIntegerDataSource : IDatasource<int>
    {
        public RandomIntegerDataSource(int min, int max)
        {
            _min = min;
            _max = max;
        }

        Random random = new Random();
        int _min, _max;
        public object Next(IGenerationSession session)
        {
            return random.Next(_min, _max);
        }
    }
}
