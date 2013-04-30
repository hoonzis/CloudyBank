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
using System.Windows.Data;

namespace CloudyBank.MVVM
{
    public class ViewModelLocator : FrameworkElement, INotifyPropertyChanged
    {
        public ViewModelLocator()
        {
            this.Loaded += new RoutedEventHandler(ViewModelLocator_Loaded);
        }

        void ViewModelLocator_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadValue();
        }


        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        
        public void ReloadValue()
        {
            if(ViewModelName != null)
            {
                Binding binding = new Binding();
                binding.Source = this.WalkUpTreeTillVM(ViewModelName);
                this.SetBinding(DataProperty, binding);   
            }
        }

        public String ViewModelName
        {
            get;
            set;
            //get { return (String)GetValue(ViewModelNameProperty); }
            //set { SetValue(ViewModelNameProperty, value); }
        }

        public static readonly DependencyProperty ViewModelNameProperty =
            DependencyProperty.Register("ViewModelName", typeof(String), typeof(ViewModelLocator), new PropertyMetadata(ViewModelNameChanged));

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Object), typeof(ViewModelLocator), null);

        public static void ViewModelNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as ViewModelLocator;
            owner.ReloadValue();
        }

        
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
