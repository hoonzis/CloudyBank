using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl;
using System.Collections;

namespace CloudyBank.Web.Ria.Components
{
    /// <summary>
    /// This class represents a map bindable to list of items.
    /// Each item is visualized on the map and can be selected.
    /// </summary>
    public partial class SelectableMap : UserControl
    {
        private Pushpin _selectedPushpin;

        
        public Object SelectedItem
        {
            get { return (Object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedProfile", typeof(Object), typeof(SelectableMap), null);



        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SelectableMap),
           new PropertyMetadata(ItemsSourcePropertyChanged));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sMap = d as SelectableMap;
            sMap.MapItems.ItemsSource = e.NewValue as IEnumerable;
            sMap.map.UpdateLayout();
        }

        public SelectableMap()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SelectableMap_Loaded);
        }

        void SelectableMap_Loaded(object sender, RoutedEventArgs e)
        {
            this.MapItems.ItemsSource = ItemsSource;
        }

        private void Pushpin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pushpin = sender as Pushpin;
            SelectedItem = pushpin.DataContext;

            if (_selectedPushpin != null)
            {
                _selectedPushpin.Content = null;
                _selectedPushpin.UpdateLayout();
            }
           
            _selectedPushpin = pushpin;
            _selectedPushpin.Content = new Ellipse() { Width = 13, Height = 13, Fill = new SolidColorBrush(Colors.Blue) };
            _selectedPushpin.UpdateLayout();

        }
    }
}
