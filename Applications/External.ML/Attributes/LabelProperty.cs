using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ml.Attributes
{
    public class LabelProperty : Property
    {
        [XmlArray("Labels"), XmlArrayItem("Labels")]
        public Object[] Labels { get; set; }
    }
}
