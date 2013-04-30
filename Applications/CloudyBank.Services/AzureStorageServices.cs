using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using System.IO;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Security;
using System.Web;
using System.Security;
using CloudyBank.Dto;
using Common.Logging;
using CloudyBank.Services.FileGeneration;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace CloudyBank.Services
{
    public class AzureStorageServices : ICloudStorageServices
    {

        private IRepository _repository;
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private readonly ILog log = LogManager.GetLogger(typeof(AzureStorageServices));
        private bool _isAzure = false;

        public AzureStorageServices(IRepository repository)
        {
            _repository = repository;
            
            if (RoleEnvironment.IsAvailable)
            {
                //FromConfigurationSetting method executes the delegate passed to SetConfigurationSettingPublisher
                //call SetConfigurationSettingPublisher, passing in the logic to get connection string data from your custom source.
                CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                {
                    var connectionString = RoleEnvironment.GetConfigurationSettingValue(configName);
                    configSetter(connectionString); //configSetter is the delegate
                });

                _storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
                _blobClient = _storageAccount.CreateCloudBlobClient();
                _isAzure = true;
            }
        }

        private String ChechUserAndReturnContainerName(int userId)
        {
            var user = _repository.Load<UserIdentity>(userId);

            bool result = false;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.User.Identity.Name == user.Identification)
                {
                    result = true;
                }
            }

            if (!result)
            {
                throw new SecurityException();
            }

            return "cont" + userId.ToString();
        }

        public byte[] DownloadSmallFile(string fileUri, int userId)
        {
            String containerName = ChechUserAndReturnContainerName(userId);

            var blobContainer = _blobClient.GetContainerReference(containerName);
            blobContainer.CreateIfNotExists();

            var containerPermission = blobContainer.GetPermissions();
            containerPermission.PublicAccess = BlobContainerPublicAccessType.Container;
            blobContainer.SetPermissions(containerPermission);

            var blobReference = blobContainer.GetBlobReferenceFromServer(fileUri);

            var data = blobReference.DownloadByteArray();

            return data;
        }

        public bool UploadSmallFile(string fileUri, int userId, byte[] data)
        {
            if (!_isAzure) { return false; }
            String containerName = ChechUserAndReturnContainerName(userId);

            var blobContainer = _blobClient.GetContainerReference(containerName);
            blobContainer.CreateIfNotExist();

            var containerPermission = blobContainer.GetPermissions();
            containerPermission.PublicAccess = BlobContainerPublicAccessType.Container;
            blobContainer.SetPermissions(containerPermission);

            var blobReference = blobContainer.GetBlobReference(fileUri);
            blobReference.UploadByteArray(data);

            return true;
        }

        public List<FileDto> GetFileList(int userId)
        {
            if (!_isAzure) { return null; }

            String containerName = ChechUserAndReturnContainerName(userId);

            var blobContainer = _blobClient.GetContainerReference(containerName);
            blobContainer.CreateIfNotExist();

            BlobRequestOptions blobRequestOptions = new BlobRequestOptions();
            blobRequestOptions.BlobListingDetails = BlobListingDetails.Metadata;

            var blobs = blobContainer.ListBlobs(blobRequestOptions);

            List<FileDto> files = new List<FileDto>();
            foreach(var blobItem in blobs)
            {
                if(blobItem is CloudBlob)
                {
                    FileDto file = new FileDto();
                    var blob = blobItem as CloudBlob;
                    file.Url = blob.Uri.AbsoluteUri;

                    if(blob.Metadata.AllKeys.Contains("author"))
                    {
                        var author = blob.Metadata["author"];
                        if (author != null)
                        {
                            int authorId = Int32.Parse(blob.Metadata["author"]);
                            var user = _repository.Load<UserIdentity>(authorId);
                            try
                            {
                                file.Author = user.Email;
                            }
                            catch (Exception ex)
                            {
                                log.Error("Azure Storage - Data Inconsistency", ex);
                                //DATA inconsistency - ObjectNotFoundException raised by Hibernate
                            }
                        }
                    }
                    
                    file.LastModified = blob.Properties.LastModifiedUtc;

                    file.ContentType = blob.Properties.ContentType;
                    
                    files.Add(file);
                }
            }
            return files;
        }

        public bool PutBlock(string fileName, string blockId, byte[] data, int userId)
        {
            String containerName = ChechUserAndReturnContainerName(userId);
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                try
                {
                    CloudBlockBlob blob = GetBlockBlob(fileName, userId);
                    blob.PutBlock(blockId, memoryStream, null);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        public bool PutBlockList(string fileName, string[] blockIds, int userId)
        {
            try
            {
                CloudBlockBlob blob = GetBlockBlob(fileName, userId);
                blob.PutBlockList(blockIds);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public long FileSize(string fileName, int userId)
        {
            var blob = GetBlockBlob(fileName, userId);
            if (blob.Exists())
            {
                return blob.Properties.Length;
            }
            return 0;
        }

        public byte[] DownLoadBlock(string fileName, long offSet, int blockSize, int userId)
        {
            var blob = GetBlockBlob(fileName, userId);
            if (blob.Exists())
            {
                BlobStream reader = blob.OpenRead();
                reader.Seek(offSet, SeekOrigin.Begin);

                byte[] bufferBytes = new byte[blockSize];
                int total = reader.Read(bufferBytes, 0, blockSize);

                return bufferBytes;
            }
            return null;
        }

        public string GetBlobSignedSignature(int userId, string blobUri)
        {
            String containerName = ChechUserAndReturnContainerName(userId);

            var blob = _blobClient.GetContainerReference(containerName).GetBlobReference(blobUri);
            var sas = blob.GetSharedAccessSignature(new SharedAccessPolicy()
            {
                Permissions = SharedAccessPermissions.Read | SharedAccessPermissions.Write,
                SharedAccessExpiryTime = DateTime.UtcNow + TimeSpan.FromMinutes(5)
            });

            String sSignature = blob.Uri.AbsoluteUri + sas;

            return sSignature;

        }

        public String GetContainerUrl(int userId)
        {
            if (!_isAzure) { return null; }
            String containerName = ChechUserAndReturnContainerName(userId);
            var container = _blobClient.GetContainerReference(containerName);
            return container.Uri.AbsoluteUri;
        }

        public string GetContainerSignedSignature(int userId)
        {
            if (!_isAzure) { return null; }
            String containerName = ChechUserAndReturnContainerName(userId);

            var container = _blobClient.GetContainerReference(containerName);
            container.CreateIfNotExist();
            var expDate = DateTime.UtcNow + TimeSpan.FromMinutes(10);

            //turn off public access
            var permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Off;
            container.SetPermissions(permissions);

            //create shared access key for 10 minutes
            var sas = container.GetSharedAccessSignature(new SharedAccessPolicy()
            {
                Permissions = SharedAccessPermissions.Read | SharedAccessPermissions.Write 
                | SharedAccessPermissions.Delete | SharedAccessPermissions.List,
                SharedAccessExpiryTime = expDate
            });

            //String sSignature = container.Uri.AbsoluteUri + sas;
            return sas;
            //return sSignature;
        }

        //Private intern method to find the block of a file
        private CloudBlockBlob GetBlockBlob(string fileName, int userId)
        {
            String containerName = ChechUserAndReturnContainerName(userId);

            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            container.CreateIfNotExist();

            return container.GetBlockBlobReference(fileName);
        }

        public String SaveBytesToCloud(byte[] data,String fileName,int userId)
        {
            String containerName = ChechUserAndReturnContainerName(userId);

            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            container.CreateIfNotExist();

            var blobReference = container.GetBlobReference(fileName);
            blobReference.UploadByteArray(data);
            return blobReference.Uri.AbsoluteUri;

        }
    }
}
