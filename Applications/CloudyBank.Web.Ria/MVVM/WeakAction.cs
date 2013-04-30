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

namespace CloudyBank.Web.Ria.MVVM
{
    /// <summary>
    /// This class is an implementation detail of the MessageToActionsMap class.
    /// </summary>
    internal class WeakAction
    {
        /// <summary>
        /// MethodInfo method
        /// </summary>
        private readonly MethodInfo _method;

        /// <summary>
        /// Delegate type
        /// </summary>
        private readonly Type _delegateType;

        /// <summary>
        /// Weak reference
        /// </summary>
        private readonly WeakReference _weakRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction"/> class. 
        /// Constructs a WeakAction
        /// </summary>
        /// <param name="target">
        /// The instance to be stored as a weak reference
        /// </param>
        /// <param name="method">
        /// The Method Info to create the action for
        /// </param>
        /// <param name="parameterType">
        /// The type of parameter to be passed in the action. Pass null if there is not a paramater
        /// </param>
        internal WeakAction(object target, MethodInfo method, Type parameterType)
        {
            // create a Weakefernce to store the instance of the target in which the Method resides
            _weakRef = new WeakReference(target);

            _method = method;

            // JAS - Added logic to construct callback type.
            _delegateType = parameterType == null ? typeof(Action) : typeof(Action<>).MakeGenericType(parameterType);
        }

        /// <summary>
        /// Gets a value indicating whether the target is still in memory
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return _weakRef.IsAlive;
            }
        }

        /// <summary>
        /// Creates a "throw away" delegate to invoke the method on the target
        /// </summary>
        /// <returns>The delegate</returns>
        internal Delegate CreateAction()
        {
            object target = _weakRef.Target;
            if (target != null)
            {
                // Rehydrate into a real Action
                // object, so that the method
                // can be invoked on the target.
                return Delegate.CreateDelegate(
                    _delegateType,
                    _weakRef.Target,
                    _method);
            }
            return null;
        }
    }
}
