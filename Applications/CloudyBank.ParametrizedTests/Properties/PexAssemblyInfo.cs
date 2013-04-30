// <copyright file="PexAssemblyInfo.cs">Copyright ©  2010</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Moles;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;
using Microsoft.Pex.Linq;
using Microsoft.Pex.Framework.Suppression;
using CloudyBank.Services;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "VisualStudioUnitTest")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("CloudyBank.Services")]
[assembly: PexInstrumentAssembly("Common.Logging")]
[assembly: PexInstrumentAssembly("System.Web")]
[assembly: PexInstrumentAssembly("Spring.Aop")]
[assembly: PexInstrumentAssembly("Emgu.CV")]
[assembly: PexInstrumentAssembly("Microsoft.WindowsAzure.ServiceRuntime")]
[assembly: PexInstrumentAssembly("LumenWorks.Framework.IO")]
[assembly: PexInstrumentAssembly("System.Drawing")]
[assembly: PexInstrumentAssembly("CloudyBank.Dto")]
[assembly: PexInstrumentAssembly("CloudyBank.CoreDomain")]
[assembly: PexInstrumentAssembly("CloudyBank.Core")]
[assembly: PexInstrumentAssembly("System.Core")]
[assembly: PexInstrumentAssembly("System.Transactions")]
[assembly: PexInstrumentAssembly("PdfSharp")]
[assembly: PexInstrumentAssembly("Microsoft.WindowsAzure.StorageClient")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Common.Logging")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Web")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Spring.Aop")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Emgu.CV")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Microsoft.WindowsAzure.ServiceRuntime")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "LumenWorks.Framework.IO")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Drawing")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "CloudyBank.Dto")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "CloudyBank.CoreDomain")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "CloudyBank.Core")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Core")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Transactions")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "PdfSharp")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Microsoft.WindowsAzure.StorageClient")]

// Microsoft.Pex.Framework.Moles
[assembly: PexAssumeContractEnsuresFailureAtBehavedSurface]
[assembly: PexChooseAsBehavedCurrentBehavior]

// Microsoft.Pex.Linq
[assembly: PexLinqPackage]

[assembly: PexAssemblyUnderTest("CloudyBank.Core")]
[assembly: PexSuppressExplorableEvent(typeof(OperationServices))]
