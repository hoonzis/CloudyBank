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
    /// <summary>
    /// Base class for all commands.
    /// </summary>
    public class CommandBase : ICommand
    {
        
        public bool CanExecute(object parameter)
        {
            if (this._CanExecute != null)
            {
                return this._CanExecute();
            }
            if (this._CanExecuteWithParameter != null)
            {
                return this._CanExecuteWithParameter(parameter);
            }
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (this._ToExecute != null)
            {
                this._ToExecute();
            }
            if (this._ToExecuteWithParameter != null)
            {
                this._ToExecuteWithParameter(parameter);
            }
        }

        private Action _ToExecute;
        
        private Func<bool> _CanExecute;
        
        private Action<object> _ToExecuteWithParameter;
        
        private Func<object, bool> _CanExecuteWithParameter;

        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="CommandBase"/>
        /// </summary>
        /// <param name="toExecute">Method to call when the command is called.</param>
        /// <param name="canExecute">Method used to check if the command is available.</param>
        public CommandBase(Action toExecute, Func<bool> canExecute = null)
        {
            this._ToExecute = toExecute;
            this._CanExecute = canExecute;
            this._ToExecuteWithParameter = null;
            this._CanExecuteWithParameter = null;
        }

        /// <summary>
        /// Creates an instance of <see cref="CommandBase"/>
        /// </summary>
        /// <param name="toExecute">Method to call when the command is called.</param>
        /// <param name="canExecute">Method used to check if the command is available.</param>
        public CommandBase(Action<object> toExecute, Func<object, bool> canExecute = null)
        {
            this._ToExecute = null;
            this._CanExecute = null;
            this._ToExecuteWithParameter = toExecute;
            this._CanExecuteWithParameter = canExecute;
        }

    }
}
