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

namespace CloudyBank.Web.Ria.MVVM
{
    /// <summary>
    /// From Alex van Beek : http://blogs.infosupport.com/blogs/alexb/archive/2010/04/02/silverlight-4-using-the-visualstatemanager-for-state-animations-with-mvvm.aspx
    /// </summary>
    public static class VisualStates
    {
        public static readonly DependencyProperty CurrentStateProperty = DependencyProperty.RegisterAttached("CurrentState", typeof(String), typeof(VisualStates), new PropertyMetadata(TransitionToState));

        public static string GetCurrentState(DependencyObject obj)
        {
            return (string)obj.GetValue(CurrentStateProperty);
        }

        public static void SetCurrentState(DependencyObject obj, string value)
        {
            obj.SetValue(CurrentStateProperty, value);
        }

        private static void TransitionToState(object sender, DependencyPropertyChangedEventArgs args)
        {
            Control c = sender as Control;
            if (c != null)
                VisualStateManager.GoToState(c, (string)args.NewValue, true);
            else
                throw new ArgumentException("CurrentState is only supported on the Control type");
        }
    }
}
