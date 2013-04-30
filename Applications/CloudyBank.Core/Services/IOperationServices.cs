using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using System.Diagnostics.Contracts;

namespace CloudyBank.Core.Services
{
    [ContractClass(typeof(IOperationServiceContracts))]
    public interface IOperationServices
    {
        IList<OperationDto> GetOperations(int AccountID);

        String MakeTransfer(int debitAccountId, int creditAccountId, Decimal amount, String motif);

        String Transfer(Account debitAccount, Account creditAccount, Decimal amount, String motif);

        String MakeTransferToExternal(int debitAccountId, String creditAccountIban, Decimal amount, String motif);

        String TransferToExternal(Account debitAccount, String creditAccountIban, Decimal amount, String motif);

        OperationDto GetOperationById(int operationId);

        OperationDto GetOperationByAccountIdCode(int accountId, String transactionCode);

        void InsertOperationFromCBS(String iban, String description, Decimal amount, DateTime date);

        void InsertOperationsBulkData(String data, String accountID);

        IList<Operation> GetOperationsByCustomerID(int customerID);

        IList<OperationDto> GetOperationsDtoByCustomerID(int customerID);
    }

    [ContractClassFor(typeof(IOperationServices))]
    public abstract class IOperationServiceContracts : IOperationServices
    {

        public IList<OperationDto> GetOperations(int AccountID)
        {
            throw new NotImplementedException();
        }

        public String MakeTransfer(int debitAccountId, int creditAccountId, decimal amount, string motif)
        {
            Contract.Requires<OperationServicesException>(debitAccountId != creditAccountId);
            Contract.Requires<OperationServicesException>(amount > 0);
            return default(String);
        }

        public String Transfer(Account debitAccount, Account creditAccount, decimal amount, string motif)
        {
            Contract.Requires<OperationServicesException>(amount != null);
            Contract.Requires<ArgumentNullException>(debitAccount != null);
            Contract.Requires<ArgumentNullException>(creditAccount != null);
            Contract.Requires<OperationServicesException>(debitAccount != creditAccount);
            Contract.Requires<OperationServicesException>(amount > 0);

            return default(String);
        }

        public string MakeTransferToExternal(int debitAccountId, String creditAccountIban, decimal amount, string motif)
        {
            Contract.Requires<OperationServicesException>(amount > 0);
            Contract.Requires<OperationServicesException>(!string.IsNullOrEmpty(creditAccountIban));
            return default(String);
        }

        public string TransferToExternal(Account debitAccount, String creditAccountIban, decimal amount, string motif)
        {
            Contract.Requires<OperationServicesException>(amount > 0);
            Contract.Requires<OperationServicesException>(debitAccount != null);
            Contract.Requires<OperationServicesException>(!string.IsNullOrEmpty(creditAccountIban));
            return default(String);
        }

        public OperationDto GetOperationById(int operationId)
        {
            throw new NotImplementedException();
        }


        public OperationDto GetOperationByAccountIdCode(int accountId, string transactionCode)
        {
            Contract.Requires<OperationServicesException>(transactionCode != null);
            return default(OperationDto);
        }
    

        public void  InsertOperationFromCBS(string iban, string description, decimal amount, DateTime date)
        {
            Contract.Requires<OperationServicesException>(iban != null);
            Contract.Requires<OperationServicesException>(description != null);
        }
    

        public void  InsertOperationsBulkData(string data, String accountNumber)
        {
 	        Contract.Requires<OperationServicesException>(data!=null);
            Contract.Requires<OperationServicesException>(data!=String.Empty);
            Contract.Requires<OperationServicesException>(data.Length > 4);
            Contract.Requires<OperationServicesException>(accountNumber != null);
            Contract.Requires<OperationServicesException>(accountNumber != String.Empty);
        }


        public IList<Operation> GetOperationsByCustomerID(int customerID)
        {
            throw new NotImplementedException();
        }


        public IList<OperationDto> GetOperationsDtoByCustomerID(int customerID)
        {
            throw new NotImplementedException();
        }
    }
}
