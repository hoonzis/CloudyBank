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
using System.Xml.Serialization;
using System.Collections.Generic;
using ml.Attributes;

namespace ml.Supervised
{
    public class KernelPerceptronModel<T> : IModel<T>
    {
        public KernelType Type { get; set; }
        public double P { get; set; }
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

        public KernelPerceptronModel()
        {
            // default kernel values
            Type = KernelType.Polynomial;
            P = 2;
        }

        public IPredict<T> Generate()
        {
            if (Description == null || X == null || Y == null)
                throw new InvalidOperationException("Model Parameters Not Set!");

            int N = Y.Length;
            Vector a = Vector.Zeros(N);
            Matrix K = GetKernel(X);

            int n = 1;

            // hopefully enough to 
            // converge right? ;)
            // need to be smarter about
            // storing SPD kernels...
            bool found_error = true;
            while (n < 500 && found_error)
            {
                found_error = false;
                for (int i = 0; i < N; i++)
                {
                    if (Y[i] * a.Dot(K[i, VectorType.Row]) <= 0)
                    {
                        a[i] += Y[i];
                        found_error = true;
                    }
                }

                n++;
            }

            // anything that *matters*
            // i.e. support vectors
            var indices = a.Indices(d => d != 0);

            // slice up examples to contain
            // only support vectors
            return new KernelPerceptronPredictor<T>
            {
                A = a.Slice(indices),
                Y = Y.Slice(indices),
                X = X.Slice(indices, VectorType.Row),
                Type = Type,
                Param = P,
                Description = Description
            };
        }

        public IPredict<T> Generate(IEnumerable<T> examples)
        {
            if (examples == null)
                throw new InvalidOperationException("Cannot generate a model will no data!");

            LoadExamples(examples);

            return Generate();
        }

        public Matrix GetKernel(Matrix X)
        {
            Matrix K = Matrix.Zeros(1);
            switch (Type)
            {
                case KernelType.Polynomial:
                    K = X.PolyKernel(P);
                    break;
                case KernelType.RBF:
                    K = X.RBFKernel(P);
                    break;
            }

            return K;
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
            KernelPerceptronPredictor<T> predictor = new KernelPerceptronPredictor<T>();
            XmlSerializer serializer = new XmlSerializer(predictor.GetType());
            return (KernelPerceptronPredictor<T>)serializer.Deserialize(stream);
        }
    }
}
