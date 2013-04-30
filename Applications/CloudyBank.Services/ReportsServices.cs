using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.Services.FileGeneration;

namespace CloudyBank.Services
{
    public class ReportsServices : IReportsServices
    {
        ICloudStorageServices _storageServices;
        IRepository _repository;

        public ReportsServices(ICloudStorageServices storageServices, IRepository repository)
        {
            _repository = repository;
            _storageServices = storageServices;
        }

        public string GenerateAccountsList(int userId)
        {
            var customerProxy = _repository.Load<Customer>(userId);
            var accounts = customerProxy.RelatedAccounts;

            var name = GetName(customerProxy);
            byte[] pdfData = PdfLibrary.GetClientAccountList(accounts.Keys, name, customerProxy.Code);
            var uri = _storageServices.SaveBytesToCloud(pdfData, "accounts.pdf", userId);
            return uri;
        }

        public string GetName(Customer customer)
        {
            return String.Format("{0} {1}", customer.FirstName, customer.LastName);
        }
    }
}
