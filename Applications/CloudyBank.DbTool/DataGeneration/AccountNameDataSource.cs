using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    public class AccountNameDataSource : IDatasource<String>
    {
        int count;
        String[] accounts = { "Savings Account", "Credit Card Account", "Home Loan Account", "Debit Account", "Life Time Account", "Retirement Account" };
        
        public object Next(IGenerationSession session)
        {
            return accounts[count++ % accounts.Length];
        }
    }
}
