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
using CloudyBank.MVVM;
using CloudyBank.PortableServices.Tags;


namespace CloudyBank.Web.Ria.ViewModels
{
    public class TagViewModel : ViewModelBase
    {
        TagDto _tag;

        #region Ctors
        public TagViewModel()
        {
            Tag = new TagDto();
            IsNew = true;
        }

        public TagViewModel(TagDto tag)
        {
            Tag = tag;
            IsNew = false;
        }

        #endregion

        private OperationViewModel _parentOperation;

        public OperationViewModel ParentOperation
        {
            get { return _parentOperation; }
            set { 
                _parentOperation = value;
                OnPropertyChanged(() => ParentOperation);
            }
        }

        public TagDto Tag
        {
            get {
                if (_tag == null)
                {
                    _tag = new TagDto();
                }
                return _tag;
            }
            internal set { 
                _tag = value;
                OnPropertyChanged(() => Tag);
            }
        }

        public int Id
        {
            get { return _tag.Id; }
        }
        public String Title
        {
            get { return _tag.Title; }
            set
            {
                CheckName(value);
                _tag.Title = value;
                OnPropertyChanged(() => Title);
            }
        }

        private void CheckName(String name)
        {
            RemoveErrors("Name");
            if (String.IsNullOrWhiteSpace(name))
            {
                AddError("Name", ViewModelsResources.NameEmpty);
            }
        }

        public String Description
        {
            get
            {
                return _tag.Description;
            }
            set
            {
                _tag.Description = value;
                OnPropertyChanged(() => Description);
            }
        }

        public bool IsNew { get; private set; }
    }
}
