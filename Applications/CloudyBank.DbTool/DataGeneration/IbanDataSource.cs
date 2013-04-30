using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    class IbanDataSource : IDatasource<String>
    {

        int i;
        public object Next(IGenerationSession session)
        {
            String kk = "23";
            String bankCode = "13490";
            String accountNumber = "1234-1245-1235";
            String branchIdentifier = "PAR1";

            return GetCountry() + kk + bankCode + branchIdentifier + accountNumber;
        }

        public String GetCountry()
        {
            String[] countries = { "FR", "CZ", "US", "GB" };
            i++;
            return countries[i%countries.Length];
            
        }
    }
}
