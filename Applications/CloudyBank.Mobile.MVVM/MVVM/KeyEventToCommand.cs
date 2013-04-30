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

namespace CloudyBank.MVVM
{
    public class KeyEventToCommand : EventToCommand
    {
        protected override void Invoke(object parameter)
        {
            KeyEventArgs args = parameter as KeyEventArgs;
            if (args.Key.ToString().ToLower() == Key.ToLower())
            {
                base.Invoke(parameter);
            }
        }

        public String Key
        {
            get;
            set;
        }
    }
}
