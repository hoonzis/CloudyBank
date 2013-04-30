using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Browser;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using CloudyBank.MVVM;
using CloudyBank.Web.Ria.Technical;
using CloudyBank.PortableServices.Files;
using System.Threading;

namespace CloudyBank.Web.Ria.ViewModels
{
    public class VaultViewModel : ViewModelBase
    {

        private volatile Dictionary<Guid, FileInfo> _filesDictionary = new Dictionary<Guid, FileInfo>();
        private String _containerSharedSignature;
        private DateTime _expirationDate;
        private String _containerUrl;
        private int _userId;
        private bool _isStorageDisponible;
        HttpWebRequest _webRequest;

        public bool IsStorageDisponible
        {
            get { return _isStorageDisponible; }
            set { 
                _isStorageDisponible = value;
                OnPropertyChanged(() => IsStorageDisponible);
                DelegateAction(() =>
                {
                    GeneratePdfCommand.RaiseCanExecuteChanged();
                    UploadDirectCommand.RaiseCanExecuteChanged();
                    RemoveFileCommand.RaiseCanExecuteChanged();
                    FileDropCommand.RaiseCanExecuteChanged();
                });
            }
        }

        #region Constructor

        public VaultViewModel(int customerId)
        {
            _userId = customerId;
            UpdateAllFiles();
        }

        #endregion

        #region Services
        WCFFileService _fileService;

        public WCFFileService FileService
        {
            get {
                if (_fileService == null)
                {
                    _fileService = new WCFFileServiceClient();
                }
                return _fileService; }
            set { 
                _fileService = value;
            }
        }


        private IOService _ioService;

        public IOService IoService
        {
            get {
                if (_ioService == null)
                {
                    _ioService = new FDService();
                }
                return _ioService; 
            }
            set { _ioService = value; }
        }
        #endregion

        #region UpdateAllFiles

        private void UpdateAllFiles()
        {
            InProgress = true;
            FileService.BeginGetContainerSignedSignature(_userId, EndGetSharedKey, null);
        }

        public void EndGetSharedKey(IAsyncResult result)
        {
            try
            {
                if (result.IsCompleted)
                {
                    _containerSharedSignature = FileService.EndGetContainerSignedSignature(result);

                    if (_containerSharedSignature != null)
                    {
                        InProgress = true;

                        IsStorageDisponible = true;

                        Regex regex = new Regex("\\?se=(?<year>\\d*)-(?<month>\\d*)-(?<day>\\d*)T(?<h>\\d*)%3A(?<m>\\d*)%3A(?<s>\\d*)");
                        var match = regex.Match(_containerSharedSignature);


                        _expirationDate = new DateTime(
                            Convert.ToInt32(match.Groups["year"].Value),
                            Convert.ToInt32(match.Groups["month"].Value),
                            Convert.ToInt32(match.Groups["day"].Value),
                            Convert.ToInt32(match.Groups["h"].Value),
                            Convert.ToInt32(match.Groups["m"].Value),
                            Convert.ToInt32(match.Groups["s"].Value)
                         );
                        
                        FileService.BeginGetContainerUrl(_userId, EndGetContainerUrl, null);
                    }
                    //if the cloud services are not disponible on the server side
                    else
                    {
                        _isStorageDisponible = false;

                        ErrorMessage = "Cloud Services are not disponible.";
                        IsError = true;
                        
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                IsError = true;
            }
            finally
            {
                InProgress = false;
            }
            
        }

        private void EndGetContainerUrl(IAsyncResult result)
        {
            _containerUrl = FileService.EndGetContainerUrl(result);

            //get the files directly from Rest API
            ListFiles();

        }
        #endregion

        #region FileList
        private ObservableCollection<UserFileViewModel> _fileList;

        public ObservableCollection<UserFileViewModel> FileList
        {
            get { return _fileList; }
            set { 
                _fileList = value;
                OnPropertyChanged(() => FileList);
            }
        }

        //Listing files calling directly the Rest API
        private void ListFiles()
        {

            InProgress = true;
            

            //The signture is normaly in the form ?se=.... because it is added to the BLOBs URLs as parameter.
            //for the case of listing it should be added without the "?" and with & to look like
            //http://storageg/container?restype=container&comp=list&se=...

            var trimed = _containerSharedSignature.TrimStart("?".ToCharArray()).Insert(0, "&");
            var uri = String.Format("{0}{1}{2}", _containerUrl, "?restype=container&comp=list&include=snapshots&include=metadata", trimed);

            _webRequest = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(uri));
            _webRequest.BeginGetResponse(EndListFiles, Guid.NewGuid().ToString());
        }

        private void EndListFiles(IAsyncResult result)
        {
            var doc = _webRequest.EndGetResponse(result);
           
            var xDoc = XDocument.Load(doc.GetResponseStream());
            var blobs = from blob in xDoc.Descendants("Blob")
                        select ConvertToUserFile(blob);

            FileList = new ObservableCollection<UserFileViewModel>(blobs);

            
            InProgress = false;
        }

        private UserFileViewModel ConvertToUserFile(XElement blob)
        {
            UserFileViewModel file = new UserFileViewModel();
            file.FileName = blob.Element("Name").Value;
            file.Url = blob.Element("Url").Value + _containerSharedSignature;
            var metadata = blob.Element("Metadata");
            if (metadata != null)
            {
                file.Author = metadata.Element("author").Value;
            }
            return file;
        }

        #endregion

        #region Methods to upload and download throught WCF services
        //these services are not used - azure storage is accessed directly

        //private void LoadFileListByWCF()
        //{
        //    FileService.BeginGetFileList(_userId,EndGetFileList, null);
        //}

        //private void EndGetFileList(IAsyncResult result)
        //{
        //    FileList = new ObservableCollection<UserFileViewModel>(FileService.EndGetFileList(result).Select(
        //        x=>new UserFileViewModel{
        //            FileName = x.Url.TrimStart(_containerUrl.ToCharArray()),
        //            Url=x.Url + _containerSharedSignature,
        //            Author = x.Author,
        //            LastModified = x.LastModified
        //        }));
        //}

        //#region UploadLargeFile

        //public void UploadLargeFile()
        //{
        //    var fileInfo = IoService.OpenFileInfo();

        //    String fileName = fileInfo.Name;
        //    Stream fileStream = fileInfo.OpenRead();

        //    //create uploader for blocks of 100kb
        //    FileUploader uploader = new FileUploader(fileStream, fileName,1024*100,_userId);
        //}

        //private ICommand _uploadLargeFileCommand;

        //public ICommand UploadLargeFileCommand
        //{
        //    get {
        //        if (_uploadLargeFileCommand == null)
        //        {
        //            _uploadLargeFileCommand = new CommandBase(UploadLargeFile, () => true);
        //        }
        //        return _uploadLargeFileCommand; 
        //    }
        //}
        //#endregion

        //#region UploadCommand
        //private ICommand _uploadFileCommand;

        //public ICommand UploadFileCommand
        //{
        //    get
        //    {
        //        if (_uploadFileCommand == null)
        //        {
        //            _uploadFileCommand = new CommandBase(UploadFile, () => true);
        //        }
        //        return _uploadFileCommand;
        //    }
        //}

        //public void UploadFile()
        //{
        //    var fileInfo = IoService.OpenFileInfo();
        //    Stream fileStream = fileInfo.OpenRead();

        //    byte[] data = new byte[fileStream.Length];
        //    fileStream.Read(data, 0, data.Length);

        //    FileService.BeginUploadSmallFile(fileInfo.Name + Guid.NewGuid(), _userId, data, new AsyncCallback(EndUploadFile), null);
        //}

        //public void EndUploadFile(IAsyncResult result)
        //{
        //    bool success = FileService.EndUploadSmallFile(result);
        //    LoadFileList();
        //}
        //#endregion

        #endregion

        #region UploadDirect

        private CommandBase _uploadDirectCommand;

        public CommandBase UploadDirectCommand
        {
            get {
                if (_uploadDirectCommand == null)
                {
                    _uploadDirectCommand = new CommandBase(UploadDirect, ()=>true);
                }
                return _uploadDirectCommand;
            }
        }

        public void UploadFile(FileInfo fileInfo)
        {
            InProgress = true;

            if (fileInfo != null)
            {
                if (_expirationDate.CompareTo(DateTime.UtcNow.AddMinutes(3)) > 0)
                {
                    //if the token did not yet expire
                    AzureUploaderViewModel uploader = new AzureUploaderViewModel(_containerUrl, _containerSharedSignature, fileInfo.OpenRead(), fileInfo.Name, _userId);
                    uploader.UploadFinished += new EventHandler(uploader_UploadFinished);
                    Uploads.Add(uploader);
                    uploader.StartUpload();
                }
            }
        }

        public void UploadDirect()
        {
            //Guid fileGuid = Guid.NewGuid();
            var fileInfo = IoService.OpenFileInfo();
            UploadFile(fileInfo);
        }


        void uploader_UploadFinished(object sender, EventArgs e)
        {
            //One of the uploads had finished - reload
            UpdateAllFiles();
        }

        #endregion

        #region Drop Files
        private CommandBaseGeneric<DragEventArgs> _fileDropCommand;

        public CommandBaseGeneric<DragEventArgs> FileDropCommand
        {
            get {
                if (_fileDropCommand == null)
                {
                    _fileDropCommand = new CommandBaseGeneric<DragEventArgs>(DropFiles, (args) => true);
                }
                return _fileDropCommand; }
            
        }

        private void DropFiles(DragEventArgs e)
        {
            //VaultViewModel
            if (e.Data != null)
            {
                FileInfo[] files = e.Data.GetData(DataFormats.FileDrop) as FileInfo[];

                foreach (FileInfo fi in files)
                {
                    UploadFile(fi);
                }
            }
        }

        #endregion DropFiles

        #region Remove File

        private CommandBaseGeneric<UserFileViewModel> _removeFileCommand;

        public CommandBaseGeneric<UserFileViewModel> RemoveFileCommand
        {
            get
            {
                if (_removeFileCommand == null)
                {
                    _removeFileCommand = new CommandBaseGeneric<UserFileViewModel>(RemoveFile, CanRemoveFile);
                }
                return _removeFileCommand;
            }

        }

        private void RemoveFile(UserFileViewModel file)
        {
            InProgress = true;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(file.Url));
            webRequest.Method = "DELETE";
            webRequest.BeginGetResponse(EndDeteleFileResponse, webRequest);

        }

        private bool CanRemoveFile(UserFileViewModel file)
        {

            return file != null;
        }

        private void EndDeteleFileResponse(IAsyncResult result)
        {
            HttpWebRequest webRequest = result.AsyncState as HttpWebRequest;
            HttpWebResponse response = (HttpWebResponse)webRequest.EndGetResponse(result);
            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                //synchronise content with cloud
                UpdateAllFiles();
            }
            else
            {
                ErrorMessage = response.StatusCode.ToString();
                IsError = true;
            }
        }

        #endregion Remove File

        #region Generate PDF with account overview

        private CommandBase _generatePdfCommand;

        public CommandBase GeneratePdfCommand
        {
            get {
                if (_generatePdfCommand == null)
                {
                    _generatePdfCommand = new CommandBase(GeneratePdf, () => true);
                }
                return _generatePdfCommand; }
            
        }

        private void GeneratePdf()
        {
            InProgress = true;
            FileService.BeginGenerateAccountsList(_userId, EndGeneratePdf, null);
        }

        private void EndGeneratePdf(IAsyncResult result)
        {
            String fileUrl = FileService.EndGenerateAccountsList(result);
            UpdateAllFiles();
        }

        #endregion

        ObservableCollection<AzureUploaderViewModel> _uploads;

        public ObservableCollection<AzureUploaderViewModel> Uploads
        {
            get {
                if (_uploads == null)
                {
                    _uploads = new ObservableCollection<AzureUploaderViewModel>();
                }
                return _uploads;
            }
            set {
                _uploads = value;
                OnPropertyChanged(() => Uploads);
            }
        }

        private bool _inProgress;

        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                _inProgress = value;
                OnPropertyChanged(() => InProgress);
            }
        }

        private String _errorMessage;

        public String ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(() => ErrorMessage);
            }
        }

        private bool _isError;

        public bool IsError
        {
            get { return _isError; }
            set
            {
                _isError = value;
                OnPropertyChanged(() => IsError);
            }
        }

    }
}
