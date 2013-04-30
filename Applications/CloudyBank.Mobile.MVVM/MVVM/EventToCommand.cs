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
using System.Windows.Interactivity;

namespace CloudyBank.MVVM
{
    public class EventToCommand : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty BindParametersProperty = DependencyProperty.Register("BindParameter", typeof(bool), typeof(EventToCommand), null);
        
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventToCommand), null);
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(EventToCommand), null);

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }

            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return this.GetValue(CommandParameterProperty);
            }

            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        public bool BindParameters
        {
            get
            {
                return (bool)GetValue(BindParametersProperty);
            }
            set
            {
                SetValue(BindParametersProperty, value);
            }
        }

        protected override void Invoke(object parameter)
        {
            
            if (parameter != null && BindParameters)
            {
                CommandParameter = parameter;
            }


            if (Command != null)
            {
                Command.Execute(CommandParameter);
            }
        }
    }
}
