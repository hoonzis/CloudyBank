/*
 Copyright (c) 2011 Seth Juarez

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
*/

using System;
using System.Reflection;
using ml.Conversion;
using System.Linq;
using System.Xml.Serialization;

namespace ml.Attributes
{
    [XmlRoot("Property", Namespace = "", IsNullable = false), XmlInclude(typeof(StringProperty))]
    public class Property
    {
        private Type _propertyType;

        public Property()
        {
            _propertyType = null;
        }

        /// <summary>
        /// Name of property or method
        /// </summary>
        [XmlElement]
        public string Name { get; set; }

        /// <summary>
        /// Return type name of property or method
        /// </summary>
        [XmlElement]
        public string TypeName { get; set; }

        /// <summary>
        /// Treat property as a method
        /// </summary>
        [XmlElement]
        public bool isMethod { get; set; }

        /// <summary>
        /// Return type of property or method
        /// </summary>
        [XmlIgnore]
        public Type Type
        {
            get
            {
                if (_propertyType == null)
                    _propertyType = System.Type.GetType(TypeName);
                // must be some other type?
                // load type from current app domain
                if (_propertyType == null)
                {
                    // look through ALL existing types
                    // (I know, slow right...)
                    var q = (from p in AppDomain.CurrentDomain.GetAssemblies()
                             from t in p.GetTypes()
                             where t.FullName == TypeName
                             select t).ToArray();

                    if (q.Length == 1)
                        _propertyType = q[0];
                    else
                        throw new InvalidCastException(string.Format("Cannot find type {0} in current app domain!", TypeName));
                }

                return _propertyType;
            }
            set
            {
                _propertyType = value;
                TypeName = _propertyType.FullName;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} as {1} (using {3})", Name, Type, GetType());
        }
    }
}
