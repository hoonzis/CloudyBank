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

namespace CloudyBank.Web.Ria.UserControls
{
    public partial class AgencyMenuItem : UserControl
    {


        public String MenuText
        {
            get { return (String)GetValue(MenuTextProperty); }
            set { SetValue(MenuTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MenuText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuTextProperty =
            DependencyProperty.Register("MenuText", typeof(String), typeof(AgencyMenuItem), new PropertyMetadata(MenuTextChanged));

        public static void MenuTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as AgencyMenuItem;
            item.MenuTextBox.Text = e.NewValue as String;
        }
        
        //public String MenuText { get; set; }
        public String ImageSrc { get; set; }
        public String NavigateUri { get; set; }
        public String TargetName { get; set; }

        

        public AgencyMenuItem()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(AgencyMenuItem_Loaded);
        }

        void AgencyMenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            MenuTextBox.Text = MenuText;
            MenuImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(ImageSrc, UriKind.Relative));
            MenuHyperlinkButton.NavigateUri = new Uri(NavigateUri, UriKind.Relative);
            MenuHyperlinkButton.TargetName = TargetName;
        }

    }
}
