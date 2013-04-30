using System;
using Microsoft.Pex.Framework;
using CloudyBank.CoreDomain.Bank;
using System.Collections.Generic;
using CloudyBank.UnitTests.Data;
using System.Linq;

namespace CloudyBank.CoreDomain.Bank
{
    /// <summary>A factory for CloudyBank.CoreDomain.Bank.Account instances</summary>
    public static partial class AccountFactory
    {
        /// <summary>A factory for CloudyBank.CoreDomain.Bank.Account instances</summary>
        [PexFactoryMethod(typeof(Account))]
        public static Account Create(
            int value_i,
            decimal value_d,
            DateTime? value_null,
            string value_s,
            IList<Operation> value_iList,
            int value_i1,
            bool value_b,
            string value_s1,
            string value_s2,
            IList<TagDepenses> value_iList1_,
            string value_s3,
            DateTime? value_null1
        )
        {
            //return DataHelper.GetAccounts().FirstOrDefault(x => x.Id == value_i);
            //PexAssume.IsNotNullOrEmpty<Operation>(value_iList);

            Account account = new Account();
            account.Id = value_i;
            account.Balance = value_d;
            account.BalanceDate = value_null;
            account.Number = value_s;
            account.Operations = DataHelper.GetOperations();
            account.NbOfDaysOverdraft = value_i1;
            account.AuthorizeOverdraft = value_b;
            account.Iban = value_s1;
            account.Currency = value_s2;
            account.TagDepenses = value_iList1_;
            ((BankingProduct)account).Name = value_s3;
            ((BankingProduct)account).CreationDate = value_null1;
            return account;

            
        }
    }
}
