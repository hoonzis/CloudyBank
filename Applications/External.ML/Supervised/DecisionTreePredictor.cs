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
    [XmlRoot("DecisionTree")]
    public class DecisionTreePredictor<T> : IPredict<T>
    {
        [XmlElement("TypeDescription")]
        public TypeDescription Description { get; set; }
        [XmlElement("Tree")]
        public Node Tree { get; set; }
        [XmlAttribute]
        public double Hint { get; set; }

        public T Predict(T item)
        {
            if (Description != null)
                Description = Converter.GetDescription(typeof(T));

            var values = Converter.Convert<T>(item, Description.Features);
            var prediction = WalkNode(values, Tree);

            Converter.SetItem<T>(item, Description.Label, prediction);

            return item;
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

        private double WalkNode(Vector v, Node node)
        {
            if (node.IsLeaf)
                return node.Label;

            // Get the index of the feature for this node.
            var feature = node.Feature;
            if (feature == -1)
                throw new InvalidOperationException("Invalid Feature encountered during node walk!");

            // segmented split
            // with width set
            if (node.Segmented)
            {
                for (int i = 1; i < node.Values.Length; i++)
                    if (v[feature] >= node.Values[i - 1] && v[feature] < node.Values[i])
                        return WalkNode(v, node.Children[i - 1]);

                if (Hint != null)
                    return Hint;
                else
                    throw new InvalidOperationException(String.Format("Unable to match split value {0} for feature {1}", v[feature], Description.Features[feature].Name));
            }
            // non-continous split
            else
            {
                for (int i = 0; i < node.Values.Length; i++)
                    if (node.Values[i] == v[feature])
                        return WalkNode(v, node.Children[i]);

                if (Hint != null)
                    return Hint;
                else
                    throw new InvalidOperationException(String.Format("Unable to match split value {0} for feature {1}", v[feature], Description.Features[feature].Name));
            }
        }
    }
}
