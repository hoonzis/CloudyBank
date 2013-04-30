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
    public class MethodStarter : FrameworkElement
    {
        public MethodStarter()
        {
            Loaded += new RoutedEventHandler(MethodStarter_Loaded);
        }

        void MethodStarter_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext;
            if (vm != null)
            {
                MethodInfo mInfo = vm.GetType().GetMethod(MethodName);
                mInfo.Invoke(vm, null);
            }
        }

        public String MethodName
        {
            get { return (String)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register("MethodName", typeof(String), typeof(MethodStarter), new PropertyMetadata(MethodNameChanged));

        public static void MethodNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           //do something??
        }
    }
}
