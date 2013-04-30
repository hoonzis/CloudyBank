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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

namespace CloudyBank.Web.Ria.MVVM
{
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region INotifyPropertyChanged

        public Action RunWhenErrorsChange;
        
        /// <summary>
        /// This methods allows raising the PropertyChanged event based on the lambda expression containing the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        protected void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            var expression = property.Body as MemberExpression;
            var member = expression.Member;


            //This is important - make a local copy of the PropertyChanged handler
            //The event could be disconnected before it is called
            //by copying the handler the reference is cached just in case some other thread nulls it
            var handler = PropertyChanged;

            if (handler != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    handler(this, new PropertyChangedEventArgs(member.Name));
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


        #region INotifyDataError

        private Dictionary<String, List<String>> _errorsDictionary = new Dictionary<string, List<String>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (_errorsDictionary.ContainsKey(propertyName))
            {
                return _errorsDictionary[propertyName];
            }
            else
            {
                return null;
            }

        }

        public bool HasErrors
        {
            get
            {
                return _errorsDictionary.Count > 0;
            }
        }

        protected void AddErrorToProperty(String property, String error)
        {
            if(!_errorsDictionary.ContainsKey(property)){
                _errorsDictionary.Add(property,new List<string>());
            }
            
            _errorsDictionary[property].Add(error);
            FireErrorsChanged(property);
        }

        protected void ClearPropertyErrors(String property)
        {
            if(_errorsDictionary.ContainsKey(property)){
                _errorsDictionary.Remove(property);
                FireErrorsChanged(property);
                //TODO: check this implementation of update
                RunWhenErrorsChange();
            }
        }

        void FireErrorsChanged(string property)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(property));
            }
        }
        #endregion
    }
}
