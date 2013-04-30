using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.Math;
using ml.Metrics;

namespace ml.Unsupervised.Linkers
{
    public class SingleLinker : ILinker
    {
        private IDistance _distanceMetric;

        public SingleLinker(IDistance distanceMetric)
        {
            _distanceMetric = distanceMetric;
        }
        public double Distance(IEnumerable<Vector> x, IEnumerable<Vector> y)
        {
            double distance = -1;
            double leastDistance = Int32.MaxValue;

            for (int i = 0; i < x.Count(); i++)
            {
                for (int j = i+1; j < y.Count(); j++)
                {
                    distance = _distanceMetric.Compute(x.ElementAt(i), y.ElementAt(j));
                    
                    if (distance < leastDistance)
                        leastDistance = distance;
                }
            }

            return leastDistance;
        }
    }
}
