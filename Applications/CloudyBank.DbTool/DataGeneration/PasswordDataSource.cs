using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    class PasswordDataSource : IDatasource<String>
    {
        private String pattern = "Pass";
        public object Next(IGenerationSession session)
        {
            string pass = pattern + GlobalCounter.passwordcounter;
            GlobalCounter.passwordcounter++;
            return pass;
        }
    }
}
