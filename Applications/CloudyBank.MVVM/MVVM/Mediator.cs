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
using System.Reflection;

namespace CloudyBank.MVVM
{
    /// <summary>
    /// See : http://marlongrech.wordpress.com/2009/04/16/mediator-v2-for-mvvm-wpf-and-silverlight-applications/
    /// Provides loosely-coupled messaging between
    /// various colleagues.  All references to objects
    /// are stored weakly, to prevent memory leaks.
    /// </summary>
    public class Mediator
    {
        /// <summary>
        /// List invocation
        /// </summary>
        private readonly MessageToActionsMap _invocationList = new MessageToActionsMap();

        /// <summary>
        /// Register a ViewModel to the mediator notifications
        /// This will iterate through all methods of the target passed and will register all methods that are decorated with the MediatorMessageSink Attribute
        /// </summary>
        /// <param name="target">The ViewModel instance to register</param>
        public void Register(object target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            // Inspect the attributes on all methods and check if there are RegisterMediatorMessageAttribute
            foreach (var methodInfo in target.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                foreach (MediatorMessageSinkAttribute attribute in methodInfo.GetCustomAttributes(typeof(MediatorMessageSinkAttribute), true))
                    if (methodInfo.GetParameters().Length == 1)
                        _invocationList.AddAction(attribute.Message, target, methodInfo, attribute.ParameterType);
                    else
                        throw new InvalidOperationException("The registered method should only have 1 parameter since the Mediator has only 1 argument to pass");
        }

        /// <summary>
        /// Notify all registered parties that a specific message was broadcasted
        /// </summary>
        /// <typeparam name="T">The Type of parameter to be passed</typeparam>
        /// <param name="message">The message to broadcast</param>
        /// <param name="parameter">The parameter to pass together with the message</param>
        public void NotifyColleagues<T>(string message, T parameter)
        {
            var actions = _invocationList.GetActions(message);

            if (actions != null)
                actions.ForEach(action => action.DynamicInvoke(parameter));
        }

        /// <summary>
        /// Notify all registered parties that a specific message was broadcasted
        /// </summary>
        /// <param name="message">The message to broadcast</param>
        public void NotifyColleagues(string message)
        {
            var actions = _invocationList.GetActions(message);

            if (actions != null)
                actions.ForEach(action => action.DynamicInvoke());
        }
    }
}
