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
using ml.Math;
using System.IO;
using ml.Attributes;
using System.Xml.Serialization;

namespace ml.Supervised
{
    [XmlRoot("LinearRegression")]
    public class LinearRegressionPredictor<T> : IPredict<T>
    {
        [XmlElement("TypeDescription")]
        public TypeDescription Description { get; set; }
        [XmlElement("WeightVector")]
        public Vector W { get; set; }
        [XmlElement("Bias")]
        public double B { get; set; } 

        public T Predict(T item)
        {
            // get representation
            var x = Converter.Convert<T>(item, Description.Features);
            // calculate estimate using normalized example
            var y = Vector.Dot(W, x / Vector.Norm(x)) + B;

            // return regression value
            return Converter.SetItem<T>(item, Description.Label, y);
        }

        public void Save(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            //ensure we delete the file first or we may have extra data
            if (File.Exists(filename))
            {
                // this could get an access violation but either way we 
                // don't want a pointer stuck in the app domain
                File.Delete(filename);
            }

            using (var stream = File.OpenWrite(filename))
            {
                serializer.Serialize(stream, this);
            }
        }
    }
}
