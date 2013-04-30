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
using System.Collections.Generic;
using ml.Math;
using System.IO;
using System.Xml.Serialization;
using ml.Attributes;

namespace ml.Supervised
{
    public class PerceptronModel<T> : IModel<T>
    {
        public bool Normalize { get; set; }
        public TypeDescription Description { get; set; }
        public Matrix X { get; set; }
        public Vector Y { get; set; }

        private void LoadExamples(IEnumerable<T> examples)
        {
            if (Description == null || X == null || Y == null)
            {
                var data = Converter.Convert<T>(examples, Description);
                X = data.Item1;
                Y = data.Item2;
                Description = data.Item3;
            }
        }

        public IPredict<T> Generate()
        {
            if (Description == null || X == null || Y == null)
                throw new InvalidOperationException("Model Parameters Not Set!");

            Vector w = Vector.Zeros(X[0].Length);
            Vector a = w.Copy();

            double wb = 0;
            double ab = 0;

            int n = 1;

            if (Normalize)
                for (int j = 0; j < X.Rows; j++)
                    X[j] = X[j] / X[j].Norm();

            // repeat 10 times for *convergence*
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < X.Rows; j++)
                {
                    Vector x = X[j];
                    double y = Y[j];

                    // perceptron update
                    if (y * (Vector.Dot(w, x) + wb) <= 0)
                    {
                        w += (y * x);
                        wb += y;
                        a += (y * x) * n;
                        ab += y * n;
                    }

                    n += 1;
                }
            }

            return new PerceptronPredictor<T>
            {
                Description = Description,
                W = w - (a / n),
                B = wb - (ab / n),
                Normalized = Normalize
            };
        }

        public IPredict<T> Generate(IEnumerable<T> examples)
        {
            if (examples == null)
                throw new InvalidOperationException("Cannot generate a model will no data!");

            LoadExamples(examples);

            return Generate();
        }

        public IPredict<T> Load(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                return Load(stream);
            }
        }

        public IPredict<T> Load(Stream stream)
        {
            PerceptronPredictor<T> predictor = new PerceptronPredictor<T>();
            XmlSerializer serializer = new XmlSerializer(predictor.GetType());
            return (PerceptronPredictor<T>)serializer.Deserialize(stream);
        }
    }
}
