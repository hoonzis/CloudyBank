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
using CloudyBank.Web.Ria.ViewModels;
using System.Collections.Generic;
using System.Net.Browser;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using CloudyBank.MVVM;

namespace CloudyBank.Web.Ria.Technical
{
    public class AzureUploaderViewModel : ViewModelBase
    {

        private int _userId;

        private String _containerUrl;
        private String _containerSignature;

        private long _dataLength;
        private long _dataSent;

        private long ChunkSize = 4194304;
        private string UploadUrl;
        private bool UseBlocks;


        private string currentBlockId;
        private List<string> blockIds = new List<string>();

        private Stream _fileStream;
        
        private String _fileName;

        public String FileName
        {
            get { return _fileName; }
            set { 
                _fileName = value;
                OnPropertyChanged(() => FileName);
            }
        }

        private FileStates _state;

        public FileStates State
        {
            get { return _state; }
            set { 
                _state = value;
                if (_state == FileStates.Finished || _state == FileStates.Error)
                {
                    if (UploadFinished != null)
                    {
                        UploadFinished(this, null);
                    }
                }
                OnPropertyChanged(() => State);
            }
        }


        private String _errorMessage;

        public String ErrorMessage
        {
            get { return _errorMessage; }
            set { 
                _errorMessage = value;
                OnPropertyChanged(() => ErrorMessage);
            }
        }

        private byte _done;

        public byte Done
        {
            get { return _done; }
            set { 
                _done = value;
                OnPropertyChanged(() => Done);
            }
        }


        public event EventHandler UploadFinished;

        public AzureUploaderViewModel(string containerUrl, String containerSignature,Stream fileStream, String fileName, int userId)
        {
            //set 0 to percent of download done
            Done = 0;

            _fileStream = fileStream;
            _fileName = fileName;

            _dataLength = _fileStream.Length;
            _dataSent = 0;

            _containerSignature = containerSignature;
            _containerUrl = containerUrl;

            _userId = userId;

            // upload the blob in smaller blocks if it's a "big" file
            if (_dataLength > ChunkSize)
            {
                UseBlocks = true;
            }
            else
            {
                UseBlocks = false;
            }

            // uploadContainerUrl has a Shared Access Signature already
            var uriBuilder = new UriBuilder(containerUrl + containerSignature);
            uriBuilder.Path += string.Format("/{0}", _fileName);
            UploadUrl = uriBuilder.Uri.AbsoluteUri;

            var sasBlobUri = uriBuilder.Uri;

        }

        public void StartUpload()
        {
            long dataToSend = _dataLength - _dataSent;

            var uriBuilder = new UriBuilder(UploadUrl);

            if (UseBlocks)
            {
                // encode the block name and add it to the query string
                currentBlockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                uriBuilder.Query = uriBuilder.Query.TrimStart('?') +
                    string.Format("&comp=block&blockid={0}", currentBlockId);
            }

            // with or without using blocks, we'll make a PUT request with the data
            HttpWebRequest webRequest = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uriBuilder.Uri);
            webRequest.Method = "PUT";
            webRequest.Headers["x-ms-meta-comment"] = "my comment";
            webRequest.Headers["x-ms-meta-author"] = Convert.ToString(_userId);
            webRequest.BeginGetRequestStream(WriteToStreamCallback, webRequest);
        }

        // write up to ChunkSize of data to the web request
        private void WriteToStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            Stream requestStream = webRequest.EndGetRequestStream(asynchronousResult);
            byte[] buffer = new Byte[4096];
            int bytesRead = 0;
            int tempTotal = 0;

            _fileStream.Position = _dataSent;

            while ((bytesRead = _fileStream.Read(buffer, 0, buffer.Length)) != 0 && tempTotal + bytesRead < ChunkSize && State != FileStates.Error)
            {
                requestStream.Write(buffer, 0, bytesRead);
                requestStream.Flush();

                _dataSent += bytesRead;
                tempTotal += bytesRead;

                Done = Convert.ToByte(100 * _dataSent / _dataLength);

            }

            requestStream.Close();

            webRequest.BeginGetResponse(ReadHttpResponseCallback, webRequest);
        }


        private void ReadHttpResponseCallback(IAsyncResult asynchronousResult)
        {
            bool error = false;

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());

                string responsestring = reader.ReadToEnd();
                reader.Close();
            }
            catch(Exception ex)
            {
                error = true;
                ErrorMessage = ex.Message;
                State = FileStates.Error;
            }

            if (!error)
            {
                blockIds.Add(currentBlockId);
            }

            // if there's more data, send another request
            if (_dataSent < _dataLength)
            {
                if (State != FileStates.Error && !error)
                {
                    StartUpload();
                }
            }
            else // all done
            {
                _fileStream.Close();
                _fileStream.Dispose();

                if (UseBlocks)
                {
                    PutBlockList(); // commit the blocks into the blob
                }
                else
                {
                    State = FileStates.Finished;
                }
            }
        }

        private void PutBlockList()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(
                new Uri(string.Format("{0}&comp=blocklist", UploadUrl)));
            webRequest.Method = "PUT";
            webRequest.Headers["x-ms-version"] = "2009-09-19"; // x-ms-version is required for put block list!
            webRequest.BeginGetRequestStream(BlockListWriteToStreamCallback, webRequest);
        }

        private void BlockListWriteToStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            Stream requestStream = webRequest.EndGetRequestStream(asynchronousResult);

            var document = new XDocument(
                new XElement("BlockList",
                    from blockId in blockIds
                    select new XElement("Uncommitted", blockId)));
            var writer = XmlWriter.Create(requestStream, new XmlWriterSettings() { Encoding = Encoding.UTF8 });
            document.Save(writer);
            writer.Flush();

            requestStream.Close();

            webRequest.BeginGetResponse(BlockListReadHttpResponseCallback, webRequest);
        }

        private void BlockListReadHttpResponseCallback(IAsyncResult asynchronousResult)
        {
            bool error = false;

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());

                string responsestring = reader.ReadToEnd();
                reader.Close();
            }
            catch(Exception ex)
            {
                error = true;
                State = FileStates.Error;
                ErrorMessage = ex.Message;
                
            }

            if (!error)
            {
                State = FileStates.Finished;
                Done = 100;
            }
        }

        private void SetMetadata(List<KeyValuePair<String, String>> metadata)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(
                new Uri(string.Format("{0}&comp=blocklist", UploadUrl)));
            webRequest.Method = "PUT";
            webRequest.Headers["x-ms-version"] = "2009-09-19"; // x-ms-version is required for put block list!
            webRequest.BeginGetRequestStream(BlockListWriteToStreamCallback, webRequest);
        }
    }
}
