using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    class DoubleFromRangeDataSource : IDatasource<double>
    {
        Random _random = new Random();
        double _low;
        double _high;
        double _add;

        public DoubleFromRangeDataSource(double low, double high)
        {
            _low = low;
            _high = high;
            _add = high - low;
        }

        
        public object Next(IGenerationSession session)
        {
            return _low  + _add * _random.NextDouble();
        }
    }
}
