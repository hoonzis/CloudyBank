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
    public class LinearRegressionModel<T> : IModel<T>
    {
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

            // normalize each each row
            for (int i = 0; i < X.Rows; i++)
                X[i, VectorType.Row] = X[i, VectorType.Row] / Vector.Norm(X[i, VectorType.Row]);

            // calculate W 
            // Could throw the following exceptions:
            // 1. SingularMatrixException, inverse becomes unstable, 
            //    This could happen if X.T * X is not full rank, although
            //    this should ALWAYS be symmetric positive definite
            //    Equivalent to Moore-Penrose pseudoinverse
            // 2. InvalidOperationException because of invalid Matrix to Vector conversion

            // this is dumb, I need to do a Cholesky factorization + backsolve
            // to ensure stability of operation
            var W = ((((X.T * X) ^ -1) * X.T) * Y).ToVector();

            // bias term
            double B = Y.Mean() - Vector.Dot(W, X.GetRows().Mean());

            // create predictor
            return new LinearRegressionPredictor<T>
            {
                Description = Description,
                W = W,
                B = B
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
            LinearRegressionPredictor<T> predictor = new LinearRegressionPredictor<T>();
            XmlSerializer serializer = new XmlSerializer(predictor.GetType());
            return (LinearRegressionPredictor<T>)serializer.Deserialize(stream);
        }
    }
}
