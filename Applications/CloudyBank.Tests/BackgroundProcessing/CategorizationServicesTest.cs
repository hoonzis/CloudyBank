using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CloudyBank.Core.DataAccess;
using CloudyBank.Core.Services;
using Rhino.Mocks;
using CloudyBank.CoreDomain.Bank;
using System.Collections.Generic;
using ml;
using CloudyBank.UnitTests.Data;
using CloudyBank.Services.Categorization;

namespace CloudyBank.UnitTests
{


    [TestClass()]
    public class CategorizationServicesTest
    {

        [TestMethod()]
        public void CategorizePaymentsTest()
        {
            //Arrange
            var operations = DataHelper.GetOperations();

            int id = 2;
            IRepository repository = MockRepository.GenerateStub<IRepository>();
            IOperationServices operationServices = MockRepository.GenerateMock<IOperationServices>();
            operationServices.Expect(x => x.GetOperationsByCustomerID(id)).Return(operations);
            
            
            //Act
            CategorizationServices target = new CategorizationServices(repository, operationServices); 
            var actual = target.CategorizePayments(id);
            
            //Assert
            Assert.AreEqual(true, actual);
        }
    }
}
