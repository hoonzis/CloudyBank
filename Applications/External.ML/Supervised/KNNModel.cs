using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.Attributes;
using ml.Math;
using System.IO;
using System.Xml.Serialization;

namespace ml.Supervised
{
    public class KNNModel<T> : IModel<T>
    {

        public TypeDescription Description { get; set; }

        public Matrix X { get; set; }

        public Vector Y { get; set; }
        public int K { get; set; }

        /// <summary>
        /// Initializes a new instance of the KNNModel class.
        /// </summary>
        public KNNModel(int k = 5)
        {
            K = k;
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

            return new KNNPredictor<T> { X = X, 
                                           Y = Y, 
                                           Description = Description, 
                                           K = K };
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
            KNNPredictor<T> predictor = new KNNPredictor<T>();
            XmlSerializer serializer = new XmlSerializer(predictor.GetType());
            return (KNNPredictor<T>)serializer.Deserialize(stream);
        }
    }
}
