using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Services;
using CloudyBank.Core.DataAccess.Moles;
using CloudyBank.Services.DtoCreators;
using System.Collections.Generic;
using CloudyBank.UnitTests.Data;
using CloudyBank.Core.Services;
using System.Linq;
using Microsoft.Moles.Framework;
using System.Moles;

namespace CloudyBank.Services
{
    [PexClass(typeof(OperationServices))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class OperationServicesTest
    {

        IList<Account> _accounts;
        public OperationServicesTest()
        {
            _accounts = DataHelper.GetAccounts();
        }

        [PexMethod, PexAllowedException(typeof(OperationServicesException))]
        public void MakeTransfer(int debitAccountId, int creditAccountId, decimal amount, string motif)
        {
            PexAssume.IsTrue(amount > 100);

            var repository = new SIRepository();
            repository.GetObject<Account>((x) => _accounts.SingleOrDefault(a => a.Id == (int)x));

            var operationRepository = new SIOperationRepository();
            var operationCreator = new OperationDtoCreator();

            //act
            var operationServices = new OperationServices(operationRepository, repository, operationCreator);
            operationServices.MakeTransfer(debitAccountId, creditAccountId, amount, motif);       
        }

        [PexMethod]
        public string Transfer(
            Account debitAccount,
            Account creditAccount,
            decimal amount,
            string motif
        )
        {
            MGuid.NewGuid = () => new Guid("64d80a10-4f21-4acb-9d3d-0332e68c4394");

            PexAssume.IsNotNull(debitAccount);
            PexAssume.IsNotNull(creditAccount);
            PexAssume.IsTrue(creditAccount != debitAccount);
            var repository = new SIRepository();
            var operationRepository = new SIOperationRepository();
            var operationCreator = new OperationDtoCreator();

            //act
            var operationServices = new OperationServices(operationRepository, repository, operationCreator);
            operationServices.Transfer(debitAccount, creditAccount, amount, motif);

            string result = operationServices.Transfer(debitAccount, creditAccount, amount, motif);
            PexAssert.IsNotNullOrEmpty(result);
            return result;
        }
    }
}
