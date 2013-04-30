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

namespace CloudyBank.Web.Ria.Technical.XamlSerialization
{
    public class XamlSerializationVisibility : Attribute
    {
        public SerializationVisibility Visibility {get;set;}


        public XamlSerializationVisibility(SerializationVisibility visibility)
        {
            Visibility = visibility;
        }
    }
}
