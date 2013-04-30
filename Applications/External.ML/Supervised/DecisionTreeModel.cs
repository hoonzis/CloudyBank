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
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using ml.Attributes;

namespace ml.Supervised
{
    public class DecisionTreeModel<T> : IModel<T>
    {
        public int Depth { get; set; }
        public int Width { get; set; }
        public ImpurityType Type { get; set; }
        public TypeDescription Description { get; set; }
        public Matrix X { get; set; }
        public Vector Y { get; set; }

        public DecisionTreeModel(int depth = 5, int width = 2, ImpurityType type = ImpurityType.Entropy)
        {
            if (width < 2)
                throw new InvalidOperationException("Cannot set dt tree width to less than 2!");
            Depth = depth;
            Width = width;
            Type = type;
        }

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

            var n = BuildTree(X, Y, Depth, new List<int>(X.Cols));

            return new DecisionTreePredictor<T>
            {
                Tree = n,
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

        private Node BuildTree(Matrix x, Vector y, int depth, List<int> used)
        {
            // reached depth limit or all labels are the same
            if (depth < 0 || y.Distinct().Count() == 1)
                return new Node { IsLeaf = true, Label = y.Mode() };

            double bestGain = -1;
            int bestFeature = -1;
            double[] splitValues = new double[] { };
            Impurity measure = null;

            for (int i = 0; i < x.Cols; i++)
            {
                var feature = x[i, VectorType.Column];
                var fd = Description.Features[i];

                // is feature discrete? ie enum or bool?
                var discrete = fd.Type.IsEnum || fd.Type == typeof(bool);

                switch (Type)
                {
                    case ImpurityType.Error:
                        if (!discrete)
                            measure = Error.Of(y)
                                        .Given(feature)
                                        .WithWidth(Width);
                        else
                            measure = Error.Of(y)
                                        .Given(feature);
                        break;
                    case ImpurityType.Entropy:
                        if (!discrete)
                            measure = Entropy.Of(y)
                                        .Given(feature)
                                        .WithWidth(Width);
                        else
                            measure = Entropy.Of(y)
                                        .Given(feature);
                        break;
                    case ImpurityType.Gini:
                        if (!discrete)
                            measure = Gini.Of(y)
                                         .Given(feature)
                                         .WithWidth(Width);
                        else
                            measure = Gini.Of(y)
                                         .Given(feature);
                        break;
                }

                double gain = measure.RelativeGain();

                if (gain > bestGain && !used.Contains(i))
                {
                    bestGain = gain;
                    bestFeature = i;
                    splitValues = measure.SplitValues;
                }
            }

            // uh oh, need to return something?
            // a weird node of some sort...
            // but just in case...
            if (bestFeature == -1)
                return new Node { IsLeaf = true, Label = y.Mode() };

            used.Add(bestFeature);
            Node n = new Node();
            n.Gain = bestGain;
            // measure has a width property set
            // meaning its a continuous var
            // (second conditional indicates
            //  a width that has range values)

            var bestFD = Description.Features[bestFeature];

            // multiway split - constant fan-out width (non-continuous)
            if (bestFD.Type.IsEnum || bestFD.Type == typeof(bool))
            {
                n.Children = new Node[splitValues.Length];
                for (int i = 0; i < n.Children.Length; i++)
                {
                    var slice = x.Indices(v => v[bestFeature] == splitValues[i], VectorType.Row);
                    n.Children[i] = BuildTree(x.Slice(slice, VectorType.Row), y.Slice(slice), depth - 1, used);
                }
                n.Segmented = false;
            }
            // continuous split with built in ranges
            else
            {
                // since this is in ranges, need each slot
                // represents two boundaries
                n.Children = new Node[measure.Width];
                for (int i = 0; i < n.Children.Length; i++)
                {
                    var slice = x.Indices(
                        v => v[bestFeature] >= splitValues[i] && v[bestFeature] < splitValues[i + 1],
                        VectorType.Row);

                    n.Children[i] = BuildTree(x.Slice(slice, VectorType.Row), y.Slice(slice), depth - 1, used);
                }
                n.Segmented = true;
            }

            n.IsLeaf = false;
            n.Feature = bestFeature;
            n.Values = splitValues;
            return n;
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
            DecisionTreePredictor<T> predictor = new DecisionTreePredictor<T>();
            XmlSerializer serializer = new XmlSerializer(predictor.GetType());
            return (DecisionTreePredictor<T>)serializer.Deserialize(stream);
        }
    }
}
