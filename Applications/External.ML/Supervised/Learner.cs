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
using System.Linq;
using ml.Attributes;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ml.Supervised
{
    public class Learner<T>
    {
        private static readonly Random r = new Random(DateTime.Now.Millisecond);
        public IModel<T>[] Models { get; private set; }
        public IPredict<T>[] Predictors { get; private set; }
        public Vector Accuracy { get; private set; }

        public IEnumerable<T> Examples { get; private set; }

        public Learner(params IModel<T>[] model)
        {
            Models = model;
        }

        public void Learn(IEnumerable<T> examples)
        {
            Examples = examples;
            var total = Examples.Count();
            // 80% for training
            var trainingCount = (int)System.Math.Ceiling(total * .8);
            // 20% for testing
            var testingSlice = GetTestPoints(total - trainingCount, total)
                                    .ToArray();

            var trainingSlice = GetTrainingPoints(testingSlice, total);

            // getting data
            var data = Converter.Convert<T>(examples);
            Matrix x = data.Item1;
            Vector y = data.Item2;
            TypeDescription description = data.Item3;

            // training
            var x_t = x.Slice(trainingSlice, VectorType.Row);
            var y_t = y.Slice(trainingSlice);

            Predictors = new IPredict<T>[Models.Length];

            // run in parallel since they all have 
            // read-only references to the data model
            // and update independently to different
            // spots
            //Parallel.For(0, Models.Length, i =>
            //{
            for (int i = 0; i < Models.Length; i++)
            {
                Models[i].X = x_t;
                Models[i].Y = y_t;
                Models[i].Description = description;
                Predictors[i] = Models[i].Generate();
            }
            //});

            // testing            
            T[] test = GetTestExamples(testingSlice, examples);
            Accuracy = Vector.Zeros(Predictors.Length);
            for (int i = 0; i < Predictors.Length; i++)
            {
                Accuracy[i] = 0;
                for (int j = 0; j < test.Length; j++)
                {
                    var truth = Conversion.Converter.GetItem<T>(test[j], description.Label);

                    Predictors[i].Predict(test[j]);

                    var pred = Conversion.Converter.GetItem<T>(test[j], description.Label);

                    // need to set it back...
                    Converter.SetItem<T>(test[j], description.Label, truth);

                    if (Object.Equals(truth, pred))
                        Accuracy[i] += 1;
                }

                Accuracy[i] /= test.Length;
            };
        }


        private T[] GetTestExamples(IEnumerable<int> slice, IEnumerable<T> examples)
        {
            return examples
                        .Where((t, i) => slice.Contains(i))
                        .ToArray();
        }

        private IEnumerable<int> GetTestPoints(int testCount, int total)
        {
            List<int> taken = new List<int>(testCount);
            while (taken.Count < testCount)
            {
                int i = r.Next(total);
                if (!taken.Contains(i))
                {
                    taken.Add(i);
                    yield return i;
                }
            }
        }

        private IEnumerable<int> GetTrainingPoints(IEnumerable<int> testPoints, int total)
        {
            for (int i = 0; i < total; i++)
                if (!testPoints.Contains(i))
                    yield return i;
        }
    }
}
