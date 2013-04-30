// <copyright file="AccountServicesTest.cs">Copyright ©  2010</copyright>
using System;
using System.Collections.Generic;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using CloudyBank.Services;
using CloudyBank.Core.Services;
using CloudyBank.Core.DataAccess.Moles;
using System.Linq;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Security;
using CloudyBank.Core.Dto;
using CloudyBank.Services.DtoCreators;
using CloudyBank.UnitTests.Data;

namespace CloudyBank.Services
{
    /// <summary>This class contains parameterized unit tests for AccountServices</summary>
    [PexClass(typeof(AccountServices))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class AccountServicesTest
    {
        IList<Customer> customers = new List<Customer>();
        
        public AccountServicesTest()
        {
            customers =  DataHelper.GetCustomers();
        }

        [PexMethod, PexAllowedException(typeof(AccountServicesException))]
        public decimal Balance(Account account)
        {
            SIRepository repository = new SIRepository();
            SIAccountRepository accountRepository = new SIAccountRepository();
            SICustomerRepository customerRepository = new SICustomerRepository();
            IDtoCreator<Account, AccountDto> accountCreator = new AccountDtoCreator();

            //act
            var accountServices = new AccountServices(repository, accountRepository, customerRepository, accountCreator);
            var result = accountServices.Balance(account);
            return result;
        }

    }
}
