using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Web.Ria.ViewModels;
using System.Collections.ObjectModel;
using CloudyBank.PortableServices.Accounts;
using System.Linq;
using Rhino.Mocks;
using System.Collections.Generic;
using CloudyBank.PortableServices.Operations;

namespace CloudyBank.Web.Ria.UnitTests.ViewModels
{
    [TestClass]
    public class AccountViewModelTest
    {
        private MockRepository _mocks;

        [TestInitialize]
        public void TestInitialize()
        {
            _mocks = new MockRepository();
            ServicesFactory.Creator = new ServiceForFixturesCreator();
        }

        [TestMethod()]
        public void TestUpdateFilteredBalance()
        {
            var accountVM = new AccountViewModel();
            accountVM.BalanceEvolution = new ObservableCollection<BalancePointViewModel>();
            accountVM.BalanceEvolution.Add(new BalancePointViewModel { Balance = 100, Date = DateTime.Now, Id = 0 });
            accountVM.BalanceEvolution.Add(new BalancePointViewModel { Balance = 130, Date = DateTime.Now.AddDays(3), Id = 1 });
            accountVM.BalanceEvolution.Add(new BalancePointViewModel { Balance = 100, Date = DateTime.Now.AddDays(4), Id = 2 });
            accountVM.BalanceEvolution.Add(new BalancePointViewModel { Balance = 100, Date = DateTime.Now.AddDays(5), Id = 2 });
            accountVM.BalanceEvolution.Add(new BalancePointViewModel { Balance = 100, Date = DateTime.Now.AddDays(6), Id = 2 });

            accountVM.CompareDate = DateTime.Now.AddDays(3);

            accountVM.UpdateFilteredBalance();

            Assert.AreEqual(accountVM.FilteredBalanceEvolution.First().Balance, 100);
        }

        [TestMethod()]
        public void TestLoadOperations()
        {
            //Given
            WCFOperationService operationService = _mocks.StrictMock<WCFOperationService>();
            IAsyncResult result = _mocks.Stub<IAsyncResult>();

            AccountViewModel vm = new AccountViewModel();
            vm.OperationsService = operationService;


            List<OperationDto> operations = new List<OperationDto>();
            operations.Add(new OperationDto { Amount = 123, Date = DateTime.Now, Currency = "EUR", Description = "CARTE PAYMENT",TagName = "Supermarket" });
            operations.Add(new OperationDto{ Amount = -123.34m, Date = DateTime.Now.Subtract(TimeSpan.FromDays(3)), Currency = "EUR", Description = "SNCF VOYAGE",TagName="Travelling"});
            operations.Add(new OperationDto { Amount = -123.34m, Date = DateTime.Now.Subtract(TimeSpan.FromDays(3)), Currency = "EUR", Description = "SNCF VOYAGE 2", TagName = "Travelling" });
            
            Expect.Call(operationService.EndGetOperationsByAccount(result)).Return(operations);
            _mocks.ReplayAll();
                
            vm.EndLoadOperations(result);

            vm.UpdateTagChartData();

            Assert.AreEqual(vm.TagChartData["Travelling"],2*123.34);
            Assert.IsFalse(vm.TagChartData.ContainsKey("Supermarket"));
            Assert.AreEqual(vm.Operations.Last().Amount, operations.Last().Amount);
        }
    }
}
