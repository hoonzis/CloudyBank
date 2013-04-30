using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using CloudyBank.Dto;
using System.ServiceModel.Activation;

namespace CloudyBank.Web.OpenServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDataService" in both code and config file together.
    [ServiceContract]
    public interface IDataService
    {
        [OperationContract]
        [WebGet]
        IList<AccountDto> GetAccounts();

        [OperationContract]
        [WebGet]
        IList<OperationDto> GetOperations();

    }
}
