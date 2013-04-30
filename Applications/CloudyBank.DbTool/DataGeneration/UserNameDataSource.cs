using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    public class UserNameDataSource : IDatasource<String>
    {
        
        private String pattern = "User";
        public object Next(IGenerationSession session)
        {
            String username = pattern + GlobalCounter.usercounter;
            GlobalCounter.usercounter++;
            return username;
        }
    }
}
