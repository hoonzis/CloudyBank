using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    class DecaDataSource : IDatasource<int>
    {

        public DecaDataSource(int start)
        {
            value = start;
        }

        public int value; 
        public object Next(IGenerationSession session)
        {
            var returnval = value;
            value +=10;
            return returnval;
            
        }
    }
}
