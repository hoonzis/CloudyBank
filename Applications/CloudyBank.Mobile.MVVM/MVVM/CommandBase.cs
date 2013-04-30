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
        /// <summary><see cref="ICommand.CanExecute"/></summary>
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

        /// <summary><see cref="ICommand.CanExecuteChanged"/></summary>
        public event EventHandler CanExecuteChanged;

        /// <summary><see cref="ICommand.Execute"/></summary>
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

        /// <summary><see cref="ICommand.Execute"/>, without parameter. Exclusive with <see cref="_ToExecuteWithParameter"/></summary>
        private Action _ToExecute;
        /// <summary><see cref="ICommand.CanExecute"/>, without parameter. Exclusive with <see cref="_ToExecute"/></summary>
        private Func<bool> _CanExecute;
        /// <summary><see cref="ICommand.Execute"/>, with parameter. Exclusive with <see cref="_ToExecuteWithParameter"/></summary>
        private Action<object> _ToExecuteWithParameter;
        /// <summary><see cref="ICommand.CanExecute"/>, with parameter. Exclusive with <see cref="_CanExecute"/></summary>
        private Func<object, bool> _CanExecuteWithParameter;

        /// <summary>
        /// Raise the <see cref="CanExecuteChanged"/> event.
        /// </summary>
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
