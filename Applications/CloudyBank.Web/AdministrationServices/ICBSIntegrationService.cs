using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CloudyBank.Web.AdministrationServices
{
    [ServiceContract]
    public interface ICBSIntegrationService
    {
        [OperationContract]
        void InsertPayment(String iban, DateTime date, String description, Decimal amount);

        [OperationContract]
        void InserBulkPayments(String data, String accountNumber);

        [OperationContract]
        bool CategorizePayments(int customerID);
    }
}
