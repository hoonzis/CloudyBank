using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using System.Globalization;
using CloudyBank.Core.DataAccess;
using System.Diagnostics.Contracts;

namespace CloudyBank.Services.Technical
{
    public static class CSVProcessing
    {
        public static char CSV_SEPARATOR = ',';
        private static IFormatProvider _provider = CultureInfo.GetCultureInfo("FR-fr");

        public static IEnumerable<Operation> GetTransactionsFromCSV(TextReader reader)
        {
            using (CsvReader csv = new CsvReader(reader, true, CSV_SEPARATOR))
            {
                int fieldCount = csv.FieldCount;

                string[] headers = csv.GetFieldHeaders();
                while (csv.ReadNextRecord())
                {
                    var timeString = csv[0];
                    var description = csv[1];
                    var amount = csv[2];
                    
                    var operation = ComposeOperation(timeString, ref amount, description);

                    yield return operation;
                }
            }
        }

        public static IList<Operation> GetCategorizedTransactionsCreateAndStoreTags(TextReader reader, IRepository repository, Dictionary<string, StandardTag> tagList)
        {
            var operations = new List<Operation>();
            using (CsvReader csv = new CsvReader(reader, true, CSV_SEPARATOR))
            {
                int fieldCount = csv.FieldCount;

                string[] headers = csv.GetFieldHeaders();
                while (csv.ReadNextRecord())
                {
                    var tagName = csv[3];

                    if (String.IsNullOrWhiteSpace(tagName))
                    {
                        tagName = "Not set";
                    }

                    StandardTag tag = null;
                    if (tagList.ContainsKey(tagName))
                    {
                        tag = tagList[tagName];
                    }
                    else
                    {
                        tag = new StandardTag();

                        tag.Name = tagName;
                        tag.Description = tagName;
                        tagList.Add(tag.Name, tag);
                        repository.Save(tag);
                    }

                    var timeString = csv[0];
                    var description = csv[1];
                    var amount = csv[2];

                    if (String.IsNullOrEmpty(timeString) || String.IsNullOrEmpty(amount) || String.IsNullOrEmpty(description))
                    {
                        break;
                    }

                    Operation operation = ComposeOperation(timeString, ref amount, description);
                    if (operation != null)
                    {
                        operation.Tag = tag;
                        operations.Add(operation);
                    }
                }
                return operations;
            }
        }

        
        public static Operation ComposeOperation(string timeString, ref string amount, string description)
        {
            Contract.Requires<ArgumentNullException>(timeString != null);
            Contract.Requires<ArgumentNullException>(amount != null);
            Contract.Requires<ArgumentNullException>(description != null);


            Operation operation = new Operation();

            DateTime date;
            DateTime.TryParse(timeString,_provider,DateTimeStyles.None, out date);

            operation.Date = date;
            operation.Description = description;
            operation.TransactionCode = Guid.NewGuid().ToString();
            operation.Currency = "EUR";

            //because the transactions used as testing data are in the european format
            //than I have to force this to europearn culture
            //cannot use current culture, because Azure runs in US culture.
            amount = amount.Replace(".", String.Empty);

            if (amount != String.Empty)
            {
                var value = 0m;
                var styles = NumberStyles.Any;
                
                Decimal.TryParse(amount, styles,_provider , out value);
                if (value > 0)
                {
                    operation.Direction = Direction.Credit;
                }
                else
                {
                    operation.Direction = Direction.Debit;
                }

                operation.Amount = Math.Abs(value);
            }
            if (operation.Amount == 0m || operation.Date == null)
            {
                return null;
            }
            return operation;
        }
    }
}
