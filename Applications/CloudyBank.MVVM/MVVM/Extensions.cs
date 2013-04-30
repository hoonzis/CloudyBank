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
using System.Diagnostics;

namespace CloudyBank.MVVM
{
    public static class Extensions
    {
        public static T WalkUpTreeTillType<T>(this FrameworkElement obj) where T : class
        {
            FrameworkElement parent = obj;
            while (parent != null)
            {
                T fe = parent as T;

                if (fe != null)
                    return fe;

                FrameworkElement oldParent = parent;
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;

                // Accounts for Popups.
                if (parent == null)
                {
                    parent = oldParent.Parent as FrameworkElement;
                }
            }
            return null;
        }

        public static object WalkUpTreeTillVM(this FrameworkElement obj, string modelviewname)
        {
            FrameworkElement parent = obj;
            while (parent != null)
            {
                var context = parent.DataContext;

                if (context != null && context.GetType().Name == modelviewname)
                {
                    return context;
                }

                FrameworkElement oldParent = parent;
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;

                // Accounts for Popups.
                if (parent == null)
                {
                    parent = oldParent.Parent as FrameworkElement;
                }
            }
            return null;
        }

        public static object WalkUpTreeTillModelViewProperty(this FrameworkElement obj,string modelviewname, string propertyname)
        {
            FrameworkElement parent = obj;
            while (parent != null)
            {
                var context = parent.DataContext;

                if (context != null && context.GetType().Name == modelviewname)
                {
                    var property = context.GetType().GetProperty(propertyname);
                    if (property != null)
                    {
                        return property.GetValue(context, null);
                    }
                }

                FrameworkElement oldParent = parent;
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;

                // Accounts for Popups.
                if (parent == null)
                {
                    parent = oldParent.Parent as FrameworkElement;
                }
            }
            return null;
        }

        //public static object WalkUpTreeTillModelView(this FrameworkElement obj, string modelviewname, string commandName)
        //{
        //    FrameworkElement parent = obj;
        //    while (parent != null)
        //    {
        //        var context = parent.DataContext;

        //        if (context != null && context.GetType().Name == modelviewname)
        //        {
        //            var property = context.GetType().GetProperty(propertyname);
        //            if (property != null)
        //            {
        //                return property.GetValue(context, null);
        //            }
        //        }

        //        FrameworkElement oldParent = parent;
        //        parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;

        //        // Accounts for Popups.
        //        if (parent == null)
        //        {
        //            parent = oldParent.Parent as FrameworkElement;
        //        }
        //    }
        //    return null;
        //}
    }
}
