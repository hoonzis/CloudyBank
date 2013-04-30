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
using Rhino.Mocks;
using CloudyBank.Web.Ria.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CloudyBank.PortableServices.Accounts;
using CloudyBank.PortableServices.Customers;

namespace CloudyBank.Web.Ria.UnitTests.ViewModels
{
    [TestClass]
    public class CustomerViewModelTest
    {
        private MockRepository _mocks;

        [TestInitialize]
        public void TestInitialize()
        {
            _mocks = new MockRepository();
        }

        [TestMethod]
        public void TestLoadAccounts()
        {
            //Given
            CustomerViewModel vm = new CustomerViewModel();
            WCFAccountService accountService = _mocks.StrictMock<WCFAccountService>();
            
            vm.AccountService = accountService;

            //create the individualcustomer
            CustomerDto customer = new CustomerDto() { Id = 1 };
            vm.Customer = customer;

            //When
            vm.LoadAccounts();

            //Then
            Assert.IsTrue(vm.InProgress);
        }

        [TestMethod]
        public void TestEndLoadAccounts()
        {
            //Given
            WCFAccountService accountService = _mocks.StrictMock<WCFAccountService>();
            IAsyncResult result = _mocks.Stub<IAsyncResult>();

            CustomerViewModel vm = new CustomerViewModel();
            vm.AccountService = accountService;

            //mock the collection returned by ThirdPartyService
            AccountDto account = new AccountDto { Balance = 100, BalanceDate = DateTime.Now, Id = 1, Title = "Account 1", Number = "123" };
            List<AccountDto> accounts = new List<AccountDto>();
            accounts.Add(account);

            Expect.Call(accountService.EndGetAccountsByCustomer(result)).Return(accounts);

            _mocks.ReplayAll();

            //When
            vm.EndLoadAccounts(result);

            Assert.IsFalse(vm.InProgress);
            Assert.Equals(vm.Accounts.Count, accounts.Count);
        }
    }
}
