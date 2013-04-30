using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Core.DataAccess;
using CloudyBank.Dto;
using CloudyBank.Services;
using CloudyBank.Core.DataAccess.Moles;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain;
using CloudyBank.CoreDomain.Security;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.Services.DtoCreators;
using CloudyBank.Core.Dto;
using System.Collections.Generic;
using System.Linq;
using CloudyBank.UnitTests.Data;
using CloudyBank.Core.Security.Moles;

namespace CloudyBank.Services
{
    [PexClass(typeof(UserServices))]
    [TestClass]
    public partial class UserServicesTest
    {
        private IList<Customer> _customers;

        public UserServicesTest()
        {
            _customers = DataHelper.GetCustomers();
        }

        [PexMethod, PexAllowedException(typeof(UserServicesException))]
        public UserIdentityDto AuthenticateIndividualCustomer(string identity, string password)
        {
            var repository = new SIRepository();
            var customerRepository = new SICustomerRepository();
            var userRepository = new SIUserRepository();
            var hashProvider = new SIHashProvider();

            IDtoCreator<UserIdentity, UserIdentityDto> identityCreator = new UserIdentityDtoCreator();
            hashProvider.HashString = (x) => x;

            customerRepository.GetCustomerByIdentityString = (x) => _customers.SingleOrDefault(c => c.Identification == x);

            //act
            UserServices userServices = new UserServices(customerRepository, repository,null,hashProvider, identityCreator);
            UserIdentityDto result = userServices.AuthenticateUser(identity, password);

            //assert
            PexAssert.Case(identity == _customers[0].Identification && password == _customers[0].Password)
                    .Implies(() => result.Email == _customers[0].Email);
            return result;
        }

    }
}
