// <copyright file="AdvisorServicesTest.cs">Copyright ©  2010</copyright>
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
using CloudyBank.CoreDomain.Advisors;
using System.Collections.Generic;
using System.Linq;
using CloudyBank.Core.Dto;
using CloudyBank.Services.DtoCreators;
using CloudyBank.UnitTests.Data;


namespace CloudyBank.Services
{
    /// <summary>This class contains parameterized unit tests for AdvisorServices</summary>
    [PexClass(typeof(AdvisorServices))]
    [TestClass]
    public partial class AdvisorServicesTest
    {
        private IList<Advisor> _advisors;

        public AdvisorServicesTest()
        {
            _advisors = DataHelper.GetAdvisors();
        }

        [PexMethod]
        internal AdvisorDto GetAdvisorById(int id)
        {
            SIAdvisorRepository advisorRepository = new SIAdvisorRepository();
            SIRepository repository = new SIRepository();
            IDtoCreator<Advisor,AdvisorDto> advisorDtoCreator = new AdvisorDtoCreator();

            repository.GetObject<Advisor>((x) => _advisors.SingleOrDefault(a => a.Id == (int)x));
            
            AdvisorServices advisorServices = new AdvisorServices(repository, advisorRepository, advisorDtoCreator);
            var result = advisorServices.GetAdvisorById(id);
            return result;
        }

        [PexMethod, PexAllowedException(typeof(AdvisorServicesException))]
        internal AdvisorDto GetAdvisorByIdentity(string identity)
        {
            //arrange
            SIAdvisorRepository advisorRepository = new SIAdvisorRepository();
            SIRepository repository = new SIRepository();
            IDtoCreator<Advisor, AdvisorDto> advisorDtoCreator = new AdvisorDtoCreator();

            advisorRepository.GetAdvisorByIdentityString = (x) => _advisors.SingleOrDefault(a => a.Identification == x);
            
            //act
            AdvisorServices services = new AdvisorServices(repository, advisorRepository, advisorDtoCreator);
            AdvisorDto result = services.GetAdvisorByIdentity(identity);
            return result;
        }
    }
}
