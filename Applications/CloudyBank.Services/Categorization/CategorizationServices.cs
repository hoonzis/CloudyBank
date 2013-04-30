using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.Supervised.NaiveBayes;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Core.Services;
using CloudyBank.Core.DataAccess;
using CloudyBank.Core.Categorization;
using System.Diagnostics;
using System.Transactions;
using CloudyBank.DataAccess.Repository;

namespace CloudyBank.Services.Categorization
{
    public class CategorizationServices : ICategorizationServices
    {
        IRepository _repository;
        IOperationServices _operationServices;

        public CategorizationServices(IRepository repository, IOperationServices operationServices)
        {
            _repository = repository;
            _operationServices = operationServices;
        }

        public bool CategorizePayments(int customerID)
        {
            NaiveBayesModel<Operation> model = new NaiveBayesModel<Operation>();
            
            var operations = _operationServices.GetOperationsByCustomerID(customerID);
            
            
            var notCategorized = operations.Where(x => x.Tag == null || x.Tag.Name == "NotSet");
            var categorized = operations.Where(x => x.Tag != null && x.Tag.Name!="NotSet");

            var predictor = model.Generate(categorized);

            foreach (var operation in notCategorized)
            {

                var newOperation = predictor.Predict(operation);
                Debug.Write(newOperation.Tag.Name);
                using (TransactionScope scope = new TransactionScope())
                {
                    _repository.Update<Operation>(newOperation);
                    scope.Complete();
                }
            }

            return true;
            
        }

    }
}
