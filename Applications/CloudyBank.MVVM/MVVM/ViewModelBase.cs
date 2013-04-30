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

namespace CloudyBank.MVVM
{
    /// <summary>
    /// Base class for all view models. This is Silverlight version, it implements IDataErrorInfo.
    /// The version of WP7, adds the methods, but does not implement the interface - the only way to 
    /// reuse the ViewModels
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
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

        #region IDataErrorInfo

        public readonly Dictionary<string, string> Errors = new Dictionary<string, string>();

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get {
                if(Errors.ContainsKey(columnName)){
                    return Errors[columnName];
                }
                return null;
            }
        }

        public void AddError(string propertyName, string message)
        {
            if (!Errors.ContainsKey(propertyName))
            {
                Errors.Add(propertyName,message);
            }
        }
 
        public void RemoveErrors(string propertyName)
        {
            Errors.Remove(propertyName);
        }
 
        public string GetErrorMessageForProperty(string propertyName)
        {
            string message;
            Errors.TryGetValue(propertyName, out message);
            return message;
        }
 
        public bool HasErrors
        {
            get
            {
                return Errors.Count != 0;
            }
        }


        #endregion

        public void DelegateAction(Action action){
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }
        
    }
}
