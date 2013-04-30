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
using CloudyBank.PortableServices.OAuthTokens;
using CloudyBank.MVVM;
using CloudyBank.Web.Ria.Technical.XamlSerialization;

namespace CloudyBank.Web.Ria.ViewModels
{ 
    public class TokenViewModel : ViewModelBase
    {
        #region Services
        private WCFOAuthManagementService _oAuthService;
        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFOAuthManagementService OAuthService
        {
            get
            {
                if (_oAuthService == null)
                {
                    _oAuthService = ServicesFactory.GetObject<WCFOAuthManagementService>(); //GetObject<WCFOAuthManagementService, WCFOAuthManagementServiceClient>();
                }
                return _oAuthService;
            }
        }

        #endregion

        public TokenViewModel()
        {
            
        }

        public TokenViewModel(TokenDto token)
        {
            Scope = token.Scope;
            _id = token.Id;
            ApplicationName = token.ApplicationName;
        }

        private String _scope;

        public String Scope
        {
            get { return _scope; }
            set { 
                _scope = value;
                OnPropertyChanged(() => Scope);
            }
        }

        private String _applicationName;

        public String ApplicationName
        {
            get { return _applicationName; }
            set { 
                _applicationName = value;
                OnPropertyChanged(() => ApplicationName);
            }
        }

        private int _id;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(() => Id);
            }
        }


        public void RemoveToken()
        {
            OAuthService.BeginInvalidateToken(this.Id, EndRemoveToken, null);
        }

        public void EndRemoveToken(IAsyncResult result)
        {
            var suc = OAuthService.EndInvalidateToken(result);
            if (suc)
            {
                SingletonMediator.Instance.NotifyColleagues("RemovedToken", Id);
            }
        }

        private CommandBase _removeTokenCommand;

        public CommandBase RemoveTokenCommand
        {
            get
            {
                if (_removeTokenCommand == null)
                {
                    _removeTokenCommand = new CommandBase(RemoveToken, () => true);
                }
                return _removeTokenCommand;
            }
        }
    }
}
