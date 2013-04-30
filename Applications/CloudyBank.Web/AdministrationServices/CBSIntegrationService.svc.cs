using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using CloudyBank.Core.Services;
using CloudyBank.Services.Categorization;

namespace CloudyBank.Web.AdministrationServices
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CBSIntegrationService : ICBSIntegrationService
    {
        private IOperationServices _operationServices;

        public IOperationServices OperationServices
        {
            get {
                if (_operationServices == null)
                {
                    _operationServices = Global.GetObject<IOperationServices>("OperationServices");
                }
                return _operationServices; 
            }
        }

        private CategorizationServices _categorizationServices;

        public CategorizationServices CategorizationServices
        {
            get {
                if (_categorizationServices == null)
                {
                    _categorizationServices = Global.GetObject<CategorizationServices>("CategorizationServices");
                }
                
                return _categorizationServices;
            }
        }

        public void InsertPayment(String iban, DateTime date, String description, Decimal amount)
        {
            OperationServices.InsertOperationFromCBS(iban, description, amount, date);
        }


        public void InserBulkPayments(string data, String accountNumber)
        {
            OperationServices.InsertOperationsBulkData(data, accountNumber);
        }


        public bool CategorizePayments(int customerID)
        {
            return CategorizationServices.CategorizePayments(customerID);
        }
    }
}
