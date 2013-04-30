using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    public class CorporateNameDataSource : IDatasource<String>
    {
        int count;
        String[] names = { "Coke", "First National", "World Best", "Tech", "Drink & Food", "Stores & Sales" };
        String[] ends = { "Inc.", "Corp.", "GMBQ", "Comp.", "Resources", "Technologies", "Consulting", "Solutions"};
        Random r = new Random();

        public object Next(IGenerationSession session)
        {
            
            return names[count++ % names.Length] + " " + ends[r.Next(0,ends.Length)];
        }
    }
}
