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
using System.IO;
using CloudyBank.Web.Ria.Technical;

namespace CloudyBank.Web.Ria.ViewModels
{
    public class UserFileViewModel : ViewModelBase
    {
        private String _fileName;

        public String FileName
        {
            get { return _fileName; }
            set { 
                _fileName = value;
                OnPropertyChanged(() => FileName);
            }
        }

        private String _url;

        public String Url
        {
            get { return _url; }
            set { 
                _url = value;
                OnPropertyChanged(() => Url);
            }
        }

        private String _contentType;

        public String ContentType
        {
            get { return _contentType; }
            set { 
                _contentType = value;
                OnPropertyChanged(() => ContentType);
            }
        }

        private String _author;

        public String Author
        {
            get { return _author; }
            set { 
                _author = value;
                OnPropertyChanged(() => Author);
            }
        }

        private DateTime _lastModified;

        public DateTime LastModified
        {
            get { return _lastModified; }
            set { 
                _lastModified = value;
                OnPropertyChanged(() => LastModified);
            }
        }
    }
}
