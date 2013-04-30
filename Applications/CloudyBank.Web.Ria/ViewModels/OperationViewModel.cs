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
using System.Collections.ObjectModel;
using CloudyBank.MVVM;
using System.Linq;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Security;
using CloudyBank.Web.Ria.Technical.XamlSerialization;
using CloudyBank.PortableServices.Accounts;
using CloudyBank.PortableServices.Operations;
using CloudyBank.PortableServices.Tags;
using CloudyBank.PortableServices.Customers;


namespace CloudyBank.Web.Ria.ViewModels
{
    public class OperationViewModel : ViewModelBase
    {
        #region Ctors
        public OperationViewModel()
        {
            Operation = new OperationDto();
        }

        public OperationViewModel(OperationDto operation)
        {
            Operation = operation;
        }

        #endregion

        #region Services
        private WCFAccountService _accountService;
        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFAccountService AccountService
        {
            get
            {
                if (_accountService == null)
                {
                    _accountService = new WCFAccountServiceClient();
                }
                return _accountService;
            }
            set { _accountService = value; }
        }


        private WCFOperationService _operationService;
        
        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFOperationService OperationService
        {
            get
            {
                if (_operationService == null)
                {
                    _operationService = new WCFOperationServiceClient();
                }

                return _operationService;
            }
            set { _operationService = value; }
        }

        private WCFTagService _tagService;

        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFTagService TagService
        {
            get {
                if (_tagService == null)
                {
                    _tagService = new WCFTagServiceClient();
                }
                return _tagService; 
            }
        }


        #endregion

        #region Operation

        private OperationDto _operation;
        public OperationDto Operation
        {
            get
            {
                if (_operation == null)
                {
                    _operation = new OperationDto();
                }
                return _operation;
            }

            set
            {
                _operation = value;
                OnPropertyChanged(() => Operation);
            }
        }

        public decimal Amount
        {
            get { return Operation.Amount; }
            set { 
                Operation.Amount = value;
                OnPropertyChanged(() => Amount);
            }
        }

        public String Motif
        {
            get { return Operation.Motif; }
            set { 
                Operation.Motif = value;
                OnPropertyChanged(() => Motif);
            }
        }

        public DateTime Date
        {
            get { return Operation.Date; }
            set
            {
                Operation.Date = value;
                OnPropertyChanged(() => Date);
            }
        }

        public String Currency
        {
            get { return Operation.Currency; }
        }
    
        public String TagName
        {
            get {
                if (Operation.TagName == null)  
                {
                    //grouping is done on tagnane, cannot have tagname=null
                    return String.Empty;
                }
                return Operation.TagName;
            }
            set { 
                Operation.TagName = value;
                OnPropertyChanged(() => TagName);
            }
        }

        public bool Positive
        {
            get { return Amount >= 0; }
        }

        public String Description
        {
            get { return Operation.Description; }
        }

        /// <summary>
        /// Update the values of this operation from backend
        /// </summary>
        public void Update()
        {
            OperationService.BeginGetOperationById(Operation.Id, EndGetOperationById, null);
        }

        private void EndGetOperationById(IAsyncResult result)
        {
            var operation = OperationService.EndGetOperationById(result);
            Operation = operation;
        }

        #endregion

        #region TagCommand
        private CommandBaseGeneric<TagViewModel> _tagCommand;

        public CommandBaseGeneric<TagViewModel> TagCommand
        {
            get {
                if (_tagCommand == null)
                {
                    _tagCommand = new CommandBaseGeneric<TagViewModel>(TagOperation, CanTagOperation);
                }
                return _tagCommand; 
            } 
        }

        public void TagOperation(TagViewModel tag)
        {
            _newTag = tag.Tag;
            
            TagService.BeginTagOperation(Operation.Id, tag.Tag.Id,EndTagOperation,null);
        }

        private TagDto _newTag;

        public void EndTagOperation(IAsyncResult result)
        {
            try
            {
                TagService.EndTagOperation(result);
                
                
                this.TagName = _newTag.Title;
                this.Operation.TagId = _newTag.Id;

                //send information to other view-models - which should be able to update the operation
                SingletonMediator.Instance.NotifyColleagues("OperationTagged", Operation.Id);
            }
            catch (FaultException)
            {
                ErrorMessage = ViewModelsResources.ErrorTaggingOperations;
            }
        }

        public bool CanTagOperation(TagViewModel tag)
        {
            return true;
        }
        #endregion

        private String _errorMessage;
        public String ErrorMessage
        {
            get { return _errorMessage; }
            set { 
                _errorMessage = value;
                OnPropertyChanged(() => ErrorMessage);
            }
        }

    }
}
