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
    [XmlRoot("KernelPerceptron")]
    public class KernelPerceptronPredictor<T> : IPredict<T>
    {
        [XmlElement("TypeDescription")]
        public TypeDescription Description { get; set; }
        [XmlAttribute("Type")]
        public KernelType Type { get; set; }
        [XmlAttribute("Param")]
        public double Param { get; set; }
        [XmlElement("X")]
        public Matrix X { get; set; }
        [XmlElement("Y")]
        public Vector Y { get; set; }
        [XmlElement("A")]
        public Vector A { get; set; }

        public T Predict(T item)
        {
            var x = Converter.Convert<T>(item, Description.Features);
            var K = GetKernel(x);
            double v = 0;
            for (int i = 0; i < A.Length; i++)
                v += A[i] * Y[i] * K[i];

            return Converter.SetItem<T>(item, Description.Label, v >= 0);
        }

        private Vector GetKernel(Vector x)
        {
            Vector K = Vector.Zeros(1);
            switch (Type)
            {
                case KernelType.Polynomial:
                    K = X.PolyKernel(x, Param);
                    break;
                case KernelType.RBF:
                    K = X.RBFKernel(x, Param);
                    break;
            }

            return K;
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
