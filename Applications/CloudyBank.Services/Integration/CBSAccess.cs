using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using System.Globalization;
using CloudyBank.Core.DataAccess;
using CloudyBank.Core.Integration;
using CloudyBank.Services.Technical;
using System.Transactions;

namespace CloudyBank.Services.Integration
{
    class CBSAccess : ICBSAccess
    {
        private IRepository _repository;

        public CBSAccess(IRepository repository)
        {
            _repository = repository;
        }

        public bool ProcessPayments()
        {
            var filePath = "/Data/CBSinput.txt";
            List<Operation> operations = new List<Operation>();

            using (var reader = new StreamReader(filePath))
            {
                var transactions = CSVProcessing.GetTransactionsFromCSV(reader);

                using (TransactionScope scope = new TransactionScope())
                {
                    foreach (var transaction in transactions)
                    {
                        _repository.Save(transaction);
                    }
                    scope.Complete();
                }
            }

            return true;
        }

        public bool SendOperationToCBS(Operation operation)
        {
            using(TextWriter writer = new StreamWriter(@"\Data\CBSOutput.csv")){ 
                writer.WriteLine(String.Format("{0},{1},{2},\"{3}\"", operation.Account.Iban, operation.Date, operation.Description, operation.Amount));
                return true;
            }
        }
    }
}
