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
using System.Threading;
using System.Globalization;
using CloudyBank.Web.Ria.Resources;

namespace CloudyBank.Web.Ria.ViewModels
{
    public class LanguageSwitcherViewModel : ViewModelBase
    {
        public string LanguageName
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            }
        }

        public void SwitchLanguage()
        {
            if (Thread.CurrentThread.CurrentUICulture.Name != "en-US")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            }
            OnPropertyChanged(() => this.LanguageName);

            LocalizedStrings strings = Application.Current.Resources["Strings"] as LocalizedStrings;
            if (strings != null)
                strings.OnChange();

        }

        private CommandBase _switchLanguageCommand;

        public CommandBase SwitchLanguageCommand
        {
            get
            {
                if (this._switchLanguageCommand == null)
                    this._switchLanguageCommand = new CommandBase(o => this.SwitchLanguage());
                return this._switchLanguageCommand;
            }
        }
    }
}
