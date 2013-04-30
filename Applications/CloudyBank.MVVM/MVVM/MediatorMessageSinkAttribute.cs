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
    /// Attribute to decorate a method to be registered to the Mediator
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MediatorMessageSinkAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediatorMessageSinkAttribute"/> class. 
        /// Constructs a method
        /// </summary>
        /// <param name="message">
        /// The message to subscribe to
        /// </param>
        public MediatorMessageSinkAttribute(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets The message to register to 
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets or sets The type of parameter for the Method
        /// </summary>
        public Type ParameterType { get; set; }
    }
}
