using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Core.DataAccess;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using CloudyBank.Services;
using CloudyBank.Core.Dto;
using Rhino.Mocks;
using CloudyBank.Services.DtoCreators;
using System;
using CloudyBank.UnitTests.Data;

namespace CloudyBank.UnitTests.Services
{
    [TestClass]
    public class OperationServicesTest
    {

        [TestMethod]
        public void GetOperations_AccountIdIsOk()
        {
            //arrange
            IOperationRepository operationRepository = MockRepository.GenerateMock<IOperationRepository>();
            IRepository repository = MockRepository.GenerateStub<IRepository>();
            IDtoCreator<Operation, OperationDto> operationCreator = new OperationDtoCreator();

            List<Operation> operations = new List<Operation>();
            var id = 2;
            operations.Add(new Operation { Id = 3});
            operationRepository.Expect(x=>x.GetOperationsByAccount(id)).Return(operations);
            
            
            //act
            OperationServices services = new OperationServices(operationRepository, repository, operationCreator);
            IList<OperationDto> retrievedOperations = services.GetOperations(id);

            //assert
            Assert.AreEqual(1, retrievedOperations.Count);
            Assert.AreEqual(3, retrievedOperations[0].Id);
            operationRepository.VerifyAllExpectations();
        }


        [TestMethod]
        [ExpectedException(typeof(OperationServicesException))]
        public void makeTransfer_DebitAccountNotFound()
        {
            //arrange
            IOperationRepository operationRepository = MockRepository.GenerateStub<IOperationRepository>();
            IRepository repository = MockRepository.GenerateStub<IRepository>();
            IDtoCreator<Operation, OperationDto> operationCreator = MockRepository.GenerateStub<IDtoCreator<Operation, OperationDto>>();

            Account creditAccount = new Account() { Id = 1 };
            Account debitAccount = new Account() { Id = 2 };

            repository.Expect(x=>x.Get<Account>(creditAccount.Id)).Return(creditAccount);

            //act
            OperationServices services = new OperationServices(operationRepository, repository,operationCreator );
            services.MakeTransfer(debitAccount.Id, creditAccount.Id, 0, null);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationServicesException))]
        public void makeTransfer_CreditAccountIsNull()
        {
            //arrange
            IOperationRepository operationRepository = MockRepository.GenerateStub<IOperationRepository>();
            IRepository repository = MockRepository.GenerateStub<IRepository>();
            IDtoCreator<Operation, OperationDto> operationCreator = MockRepository.GenerateStub<IDtoCreator<Operation, OperationDto>>();

            Account creditAccount = new Account() { Id = 1 };
            Account debitAccount = new Account() { Id = 2 };

            repository.Expect(x=>x.Get<Account>(debitAccount.Id)).Return(debitAccount);
            
            //act
            OperationServices services = new OperationServices(operationRepository, repository, operationCreator);
            services.MakeTransfer(debitAccount.Id, creditAccount.Id, 0, null);
        }

        [TestMethod]
        public void makeTransfer_Ok()
        {
            //arrange
            IOperationRepository operationRepository = MockRepository.GenerateStub<IOperationRepository>();
            IRepository repository = MockRepository.GenerateMock<IRepository>();
            IDtoCreator<Operation, OperationDto> operationCreator = MockRepository.GenerateStub<IDtoCreator<Operation, OperationDto>>();

            
            Account debitAccount = new Account() { Id = 1, Balance = 200 };
            Account creditAccount = new Account() { Id = 2, Balance = 300 };

            repository.Expect(x => x.Get<Account>(debitAccount.Id)).Return(debitAccount);
            repository.Expect(x => x.Get<Account>(creditAccount.Id)).Return(creditAccount);
            repository.Expect(x => x.Save(debitAccount));
            repository.Expect(x => x.Save(creditAccount));
            repository.Expect(x => x.Save(new Operation())).IgnoreArguments();
            repository.Expect(x => x.Save(new Operation())).IgnoreArguments();
            
            //act
            OperationServices services = new OperationServices(operationRepository, repository, operationCreator);
            services.MakeTransfer(debitAccount.Id, creditAccount.Id, 200, "transfer test");
            
            //assert
            Assert.AreEqual(0, debitAccount.Balance);
            Assert.AreEqual(500, creditAccount.Balance);

            Assert.AreEqual(1, debitAccount.Operations.Count);

            Operation debitOperation = debitAccount.Operations[0];
            Assert.AreEqual(debitAccount.Id, debitOperation.Account.Id);
            Assert.AreEqual(200, debitOperation.Amount);
            Assert.AreEqual(Direction.Debit, debitOperation.Direction);
            Assert.AreEqual("transfer test", debitOperation.Motif);

            Operation creditOperation = creditAccount.Operations[0];
            Assert.AreEqual(creditAccount.Id, creditOperation.Account.Id);
            Assert.AreEqual(200, creditOperation.Amount);
            Assert.AreEqual(Direction.Credit, creditOperation.Direction);
            Assert.AreEqual("transfer test", creditOperation.Motif);

            repository.VerifyAllExpectations();
        }

        [TestMethod]
        [ExpectedException(typeof(OperationServicesException))]
        public void makeTransfer_NotEnoughMoneyDebitAccount()
        {
            IOperationRepository operationRepository = MockRepository.GenerateStub<IOperationRepository>();
            IRepository repository = MockRepository.GenerateMock<IRepository>();
            IDtoCreator<Operation, OperationDto> operationCreator = MockRepository.GenerateStub<IDtoCreator<Operation, OperationDto>>();

            Account debitAccount = new Account() { Id = 1, Balance = 100 };
            Account creditAccount = new Account() { Id = 2, Balance = 300 };

            repository.Expect(x=>x.Get<Account>(debitAccount.Id)).Return(debitAccount);
            repository.Expect(x=>x.Get<Account>(creditAccount.Id)).Return(creditAccount);

            OperationServices services = new OperationServices(operationRepository, repository, operationCreator);
            services.MakeTransfer(debitAccount.Id, creditAccount.Id, 200, "");

            repository.VerifyAllExpectations();
        }

        [TestMethod]
        public void makeTransfer_NotEnoughMoneyDebitAccountButOverdraftAllowed()
        {
            //arrange
            IOperationRepository operationRepository = MockRepository.GenerateStub<IOperationRepository>();
            IRepository repository = MockRepository.GenerateMock<IRepository>();
            IDtoCreator<Operation, OperationDto> operationCreator = MockRepository.GenerateStub<IDtoCreator<Operation, OperationDto>>();

            Account debitAccount = new Account() { Id = 1, Balance = 0, AuthorizeOverdraft = true };
            Account creditAccount = new Account() { Id = 2, Balance = 300 };

            repository.Expect(x=>x.Get<Account>(debitAccount.Id)).Return(debitAccount);
            repository.Expect(x=>x.Get<Account>(creditAccount.Id)).Return(creditAccount);

            //act
            OperationServices services = new OperationServices(operationRepository, repository, operationCreator);
            services.MakeTransfer(debitAccount.Id, creditAccount.Id, 200, "");

            //assert
            Assert.AreEqual(-200, debitAccount.Balance);
            repository.VerifyAllExpectations();
        }

    }
}
