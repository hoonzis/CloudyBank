using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using System.Transactions;
using CloudyBank.Core.DataAccess;
using CloudyBank.Core.Dto;
using CloudyBank.Services.Security;
using CloudyBank.CoreDomain.Security;
using CloudyBank.Services.FileGeneration;
using System.IO;
using CloudyBank.Services.Strings;
using LumenWorks.Framework.IO.Csv;
using System.Globalization;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.Services.Technical;
using CloudyBank.Services.Aggregations;
using CloudyBank.Core.Aggregations;


namespace CloudyBank.Services
{
    public class OperationServices : IOperationServices
    {
        private readonly IOperationRepository _operationRepository;
        private readonly IRepository _repository;
        private readonly IDtoCreator<Operation, OperationDto> _operationCreator;

        public OperationServices(IOperationRepository operationRepository, IRepository repository, IDtoCreator<Operation, OperationDto> operationDtoCreator)
        {
            _operationRepository = operationRepository;
            _repository = repository;
            _operationCreator = operationDtoCreator;
        }

        #region Transfer methods

        #region Transfer within two accounts of the bank
        [RequiredPermission(Permission.Transfer, 0)]
        [RequiredUserType(UserType.IndividualCustomer,UserType.CorporateCustomer)]
        public String MakeTransfer(int debitAccountId, int creditAccountId, Decimal amount, String motif)
        {
            Account debitAccount = _repository.Get<Account>(debitAccountId);
            Account creditAccount = _repository.Get<Account>(creditAccountId);

            if (debitAccount == null || creditAccount == null)
            {
                throw new OperationServicesException(StringResources.ErrorDebtOrCredAccountNull);
            }
            return Transfer(debitAccount, creditAccount, amount, motif);
        }
        
        public String Transfer(Account debitAccount, Account creditAccount, decimal amount, string motif)
        {
            if (!debitAccount.AuthorizeOverdraft && debitAccount.Balance < amount)
            {
                throw new OperationServicesException(StringResources.ErrorNotEnoughtMoney);
            }

            
            String transactionCode = Guid.NewGuid().ToString();
            Operation debitOperation = new Operation() { 
                Account = debitAccount, Direction = Direction.Debit, Amount = amount, Motif = motif, Date=DateTime.Now, 
                TransactionCode=transactionCode, Currency = debitAccount.Currency};
            Operation creditOperation = new Operation() { 
                Account = creditAccount, Direction = Direction.Credit, Amount = amount, Motif = motif, Date=DateTime.Now,TransactionCode=transactionCode, Currency = creditAccount.Currency};

            debitAccount.Operations.Add(debitOperation);
        
            //for the debit payment, the account number is set as the description of the payment
            //as well as the motif of the payment
            debitOperation.Description = String.Format("TRANSFER {0} {1}", creditAccount.Iban, motif);
            creditOperation.Description = String.Format("RECEIVED PAYMENT {0} {1}", debitAccount.Iban, motif);

            creditAccount.Operations.Add(creditOperation);
            debitAccount.Balance -= amount;
            creditAccount.Balance += amount;

            using (TransactionScope scope = new TransactionScope())
            {
                _repository.Save(debitOperation);
                _repository.Save(creditOperation);
                _repository.Save(debitAccount);
                _repository.Save(creditAccount);

                scope.Complete();
            }
            return transactionCode;
           
        }

        #endregion

        #region Transfer to external account
        [RequiredPermission(Permission.Transfer, 0)]
        [RequiredUserType(UserType.IndividualCustomer, UserType.CorporateCustomer)]
        public String MakeTransferToExternal(int debitAccountId, String creditAccountIban, Decimal amount, String motif)
        {
            Account debitAccount = _repository.Get<Account>(debitAccountId);
            if (debitAccount == null)
            {
                throw new OperationServicesException("debit account is null");
            }

            if (!debitAccount.AuthorizeOverdraft && debitAccount.Balance < amount)
            {
                throw new OperationServicesException("Not enough money in the debit account");
            }
            return TransferToExternal(debitAccount, creditAccountIban, amount, motif);
        }

        public String TransferToExternal(Account debitAccount, String creditAccountIban, decimal amount, String motif)
        {
            String transactionCode = Guid.NewGuid().ToString();
            Operation debitOperation = new Operation() { 
                Account = debitAccount, 
                Direction = Direction.Debit, 
                Amount = amount, 
                Motif = motif, 
                Date = DateTime.Now, 
                TransactionCode = transactionCode, 
                OppositeIban = creditAccountIban,
                Currency = debitAccount.Currency,
                Description = String.Format("TRANSFER {0} {1}",creditAccountIban,motif)
            };
            debitAccount.Operations.Add(debitOperation);
            
            
            //update of the balance - also of the evolution of the balance
            debitAccount.Balance -= amount;
            
            
            using (TransactionScope scope = new TransactionScope())
            {

                _repository.Save(debitOperation);
                _repository.Save(debitAccount);
                
                scope.Complete();
            }
            
            return transactionCode;
            //Here in real scenario I would send the transaction to CBS
        }

        #endregion
        #endregion

        #region Methods for simulating CBS integration
        //This is a method mainly created for the demonstration of the payment categorization system
        //in real world sceneration it would be called by CBS to insert new payment
        public void InsertOperationFromCBS(String iban, String description, Decimal amount, DateTime date)
        {
            var account = _repository.Find<Account>(x => x.Iban == iban).FirstOrDefault();
            if (account == null)
            {
                throw new OperationServicesException(String.Format("Could not find account with iban: {0}", iban));
            }

            var operation = new Operation
            {
                Account = account,
                Amount = Math.Abs(amount),
                Description = description,
                Direction = amount > 0 ? Direction.Credit : Direction.Debit
            };

            _repository.Save<Operation>(operation);
        }

        public void InsertOperationsBulkData(string data, String accountNumber)
        {
            var account = _repository.Find<Account>(x => x.Number == accountNumber).FirstOrDefault();
            if (account == null)
            {
                throw new Exception("Could not find specified account using the account number");
            }

            using (var reader = new StringReader(data))
            {
                var operations = CSVProcessing.GetTransactionsFromCSV(reader);


                using (TransactionScope scope = new TransactionScope())
                {
                    foreach (var operation in operations)
                    {
                        operation.Account = account;
                        _repository.Save<Operation>(operation);
                    }
                    scope.Complete();
                }
            }
        }
        #endregion

        #region Methods for obtaining operations
        [RequiredUserType(UserType.CorporateCustomer,UserType.IndividualCustomer,UserType.Advisor)]
        public IList<Operation> GetOperationsByCustomerID(int customerID)
        {
            var customer = _repository.Load<Customer>(customerID);
            var operations = customer.RelatedAccounts.SelectMany(x => _operationRepository.GetOperationsByAccount(x.Key.Id));
            return operations.ToList();
        }


        [RequiredPermission(Permission.Read, 0)]
        [RequiredUserType(UserType.IndividualCustomer, UserType.CorporateCustomer)]
        public IList<OperationDto> GetOperations(int accountId)
        {
            var operations = _operationRepository.GetOperationsByAccount(accountId);
            if (operations != null)
            {
                return operations.Select(_operationCreator.Create).ToList();
            }
            return null;
        }

        public OperationDto GetOperationById(int operationId)
        {
            Operation operation = _repository.Get<Operation>(operationId);
            if (operation != null)
            {
                return _operationCreator.Create(operation);
            }
            return null;
        }

        [RequiredPermission(Permission.Read, 0)]
        [RequiredUserType(UserType.Advisor, UserType.CorporateCustomer, UserType.IndividualCustomer)]
        public OperationDto GetOperationByAccountIdCode(int accountId, String transactionCode)
        {
            var operation = _repository.Find<Operation>(x => x.Account.Id == accountId && x.TransactionCode == transactionCode).SingleOrDefault();
            if (operation != null)
            {
                return _operationCreator.Create(operation);
            }

            return null;
        }

        #endregion


        public IList<OperationDto> GetOperationsDtoByCustomerID(int customerID)
        {
            return GetOperationsByCustomerID(customerID).Select(_operationCreator.Create).ToList();
        }
    }
}
