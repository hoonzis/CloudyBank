using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Web;
using CloudyBank.Core.Services;
using CloudyBank.Services;
using CloudyBank.Dto;
using System.Security.Permissions;
using System.Threading;
using System.ServiceModel.Web;


namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "octo.files.service", Name="WCFFileService")]
    [ServiceBehavior(Namespace = "octo.files.port", Name = "WCFFilePort")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFFileService
    {
        public WCFFileService()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        private ICloudStorageServices _cloudServices;

        public ICloudStorageServices CloudServices
        {
            get {
                if (_cloudServices == null)
                {
                    _cloudServices = Global.GetObject<ICloudStorageServices>("AzureStorageServices");
                }
                return _cloudServices; 
            }
        }

        private IReportsServices _reportsServices;

        public IReportsServices ReportsServices
        {
            get {
                if (_reportsServices == null)
                {
                    _reportsServices = Global.GetObject<IReportsServices>("ReportsServices");
                }
                return _reportsServices; 
            }
            set { _reportsServices = value; }
        }

        //[OperationContract]
        //[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        //[WebGet(UriTemplate="files?uri={fileUri}&user={userId}")]
        //public byte[] DownloadSmallFile(String fileUri, int userId)
        //{
        //    return CloudServices.DownloadSmallFile(fileUri, userId);
        //}

        //[OperationContract]
        //[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        //[WebGet(UriTemplate = "files?uri={fileUri}&user={userId}&data={data}",BodyStyle=WebMessageBodyStyle.Bare)]
        //public bool UploadSmallFile(String fileUri, int userId, byte[] data)
        //{
        //    return CloudServices.UploadSmallFile(fileUri, userId, data);
        //}

        //[OperationContract]
        //[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        //public bool PutBlock(string fileName, string blockId, byte[] data,int userId)
        //{

        //    return CloudServices.PutBlock(fileName, blockId, data, userId);
        //}

        //[OperationContract]
        //[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        //public bool PutBlockList(string fileName, string[] blockIds,int userId)
        //{
        //    return CloudServices.PutBlockList(fileName, blockIds, userId);
        //}

        //[OperationContract]
        //[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        //public long FileSize(string fileName,int userId)
        //{
        //    return CloudServices.FileSize(fileName, userId);   
        //}

        //[OperationContract]
        //[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        //public byte[] DownLoadBlock(string fileName, long offSet, int blockSize,int userId)
        //{
        //    return CloudServices.DownLoadBlock(fileName, offSet, blockSize, userId);
        //}

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate = "fileList?id={userId}")]
        public List<FileDto> GetFileList(int userId)
        {
            return CloudServices.GetFileList(userId);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="containerSignature?id={userId}")]
        public String GetContainerSignedSignature(int userId)
        {
            return CloudServices.GetContainerSignedSignature(userId);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate = "blobSignature?id={userId}&file={fileName}", BodyStyle=WebMessageBodyStyle.Bare)]
        public String GetBlobSignedSignature(int userId, String fileName)
        {
            return CloudServices.GetBlobSignedSignature(userId, fileName);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate="containerUrl?user={userId}")]
        public String GetContainerUrl(int userId)
        {
            return CloudServices.GetContainerUrl(userId);
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        [WebGet(UriTemplate = "accountList?user={userId}")]
        public String GenerateAccountsList(int userId)
        {
            return ReportsServices.GenerateAccountsList(userId);
        }
    }
}
