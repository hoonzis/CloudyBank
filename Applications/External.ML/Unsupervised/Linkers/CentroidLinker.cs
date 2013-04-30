using System;
using System.Collections.Generic;
using System.Linq;
using ml.Math;
using ml.Metrics;

namespace ml.Unsupervised.Linkers
{
    public class CentroidLinker : ILinker
    {
        private IDistance _distanceMetric;

        public CentroidLinker(IDistance distanceMetric)
        {
            _distanceMetric = distanceMetric;
        }

        public double Distance(IEnumerable<Vector> x, IEnumerable<Vector> y)
        {
            var length = x.ElementAt(0).Length;
            Vector xs = new Vector(length);
            Vector ys = new Vector(length);

            // Sum up all values in x

            foreach (var v in x)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    xs[i] += v[i];
                }
            }

            // Sum up all values in y

            foreach (var v in y)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    ys[i] += v[i];
                }
            }

            // Calculate the average values

            for (int i = 0; i < xs.Length; i++)
            {
                xs[i] = xs[i] / length;
            }

            // Calculate the average values

            for (int i = 0; i < ys.Length; i++)
            {
                ys[i] = ys[i] / length;
            }

            return _distanceMetric.Compute(xs, ys);
        }
    }
}
