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

namespace CloudyBank.Web.Ria.UserControls
{
    /// <summary>
    /// Button which allows parameters and URL pattern to be specified
    /// http://underground.infovark.com/2010/12/30/how-to-format-the-xaml-hyperlink-navigateuri/
    /// http://www.paulstovell.com/wpf-string-format-multibinding
    /// http://csharperimage.jeremylikness.com/2009/07/imultivalueconverter-with-silverlight.html
    /// Silverligh 4 does not support MultiBinding, that is why the parameters are hard-coded on the button
    /// </summary>
    public class ParameterHyperlinkButton : HyperlinkButton
    {
        public ParameterHyperlinkButton()
        {
            Click += new RoutedEventHandler(ParameterHyperlinkButton_Click);
        }

        void ParameterHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateUri = new Uri(String.Format(Url, Param1, Param2), UriKind.Relative);
        }

        public static readonly DependencyProperty UrlProperty = DependencyProperty.Register("Url", typeof (String), typeof (ParameterHyperlinkButton), null);

        public String Url
        {
            get {return (String)GetValue(UrlProperty);}
            set { SetValue(UrlProperty, value); }
        }

        public static readonly DependencyProperty Param1Property = DependencyProperty.Register("Param1", typeof(String), typeof(ParameterHyperlinkButton), null);

        public String Param1
        {
            get { return (String)GetValue(Param1Property); }
            set { SetValue(Param1Property, value); }
        }

        public static readonly DependencyProperty Param2Property = DependencyProperty.Register("Param2", typeof(String), typeof(ParameterHyperlinkButton), null);

        public String Param2
        {
            get { return (String)GetValue(Param2Property); }
            set { SetValue(Param2Property, value); }
        }

        
    }
}
