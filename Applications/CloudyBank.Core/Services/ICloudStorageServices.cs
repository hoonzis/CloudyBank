using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Dto;
using System.Diagnostics.Contracts;

namespace CloudyBank.Core.Services
{
    [ContractClass(typeof(CloudStorageServicesContracts))]
    public interface ICloudStorageServices
    {
        
        byte[] DownloadSmallFile(String fileUri, int userId);
        bool UploadSmallFile(String fileUri, int userId, byte[] data);
        List<FileDto> GetFileList(int userId);


        bool PutBlock(string fileName, string blockId, byte[] data, int userId);
        
        bool PutBlockList(string fileName, string[] blockIds, int userId);
        long FileSize(string fileName, int userId);
        byte[] DownLoadBlock(string fileName, long offSet, int blockSize, int userId);
        
        
        String GetBlobSignedSignature(int userId, String blobName);
        String GetContainerSignedSignature(int userId);
        String GetContainerUrl(int userId);

        String SaveBytesToCloud(byte[] data, String fileName,int userId);
    }

    [ContractClassFor(typeof(ICloudStorageServices))]
    public abstract class CloudStorageServicesContracts : ICloudStorageServices
    {

        public byte[] DownloadSmallFile(string fileUri, int userId)
        {
            throw new NotImplementedException();
        }

        public bool UploadSmallFile(string fileUri, int userId, byte[] data)
        {
            throw new NotImplementedException();
        }

        public List<FileDto> GetFileList(int userId)
        {
            throw new NotImplementedException();
        }

        public bool PutBlock(string fileName, string blockId, byte[] data, int userId)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(fileName != null);
            Contract.Requires<ArgumentNullException>(blockId != null);
            return default(bool);
        }

        public bool PutBlockList(string fileName, string[] blockIds, int userId)
        {
            throw new NotImplementedException();
        }

        public long FileSize(string fileName, int userId)
        {
            throw new NotImplementedException();
        }

        public byte[] DownLoadBlock(string fileName, long offSet, int blockSize, int userId)
        {
            throw new NotImplementedException();
        }

        public string GetBlobSignedSignature(int userId, string blobName)
        {
            throw new NotImplementedException();
        }

        public string GetContainerSignedSignature(int userId)
        {
            throw new NotImplementedException();
        }

        public string GetContainerUrl(int userId)
        {
            throw new NotImplementedException();
        }

        public string SaveBytesToCloud(byte[] data, string fileName, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
