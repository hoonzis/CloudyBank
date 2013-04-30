using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    public class RandomEnumerationDataSource<T> : IDatasource<T>
    {
        Random random = new Random();
        T[] values = (T[])Enum.GetValues(typeof(T));
    
        public object  Next(IGenerationSession session)
        {
            return values[random.Next(0,values.Length)];
        }
    }
}
