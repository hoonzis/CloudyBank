using System;
using ml.Math;
using System.Xml.Serialization;
using ml.Attributes;
using System.IO;
using System.Linq;

namespace ml.Supervised
{
    [XmlRoot("KNN")]
    public class KNNPredictor<T> : IPredict<T>
    {
        [XmlElement("TypeDescription")]
        public TypeDescription Description { get; set; }
        [XmlAttribute("K")]
        public int K { get; set; }
        [XmlElement("X")]
        public Matrix X { get; set; }
        [XmlElement("Y")]
        public Vector Y { get; set; }
        

        public T Predict(T item)
        {
            var x = Converter.Convert<T>(item, Description.Features);

            var queue = new PriorityQueue<int, double>(K);
            for (int i = 0; i < X.Rows; i++)
            {
                var y = X[i, VectorType.Row];
                queue.Enqueue(i, (x - y).Norm());

                if (queue.Count > K)
                    queue.Dequeue();
            }

            var slice = from q in queue select q.Value;
            var prediction = Y.Slice(slice).Mode();
            return Converter.SetItem<T>(item, Description.Label, prediction);
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
