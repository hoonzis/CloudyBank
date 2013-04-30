using System;
using System.Collections.Generic;
using System.Linq;
using ml.Math;
using ml.Metrics;

namespace ml.Unsupervised.Linkers
{
    public class CompleteLinker : ILinker
    {
         private IDistance _distanceMetric;

        public CompleteLinker(IDistance distanceMetric)
        {
            _distanceMetric = distanceMetric;
        }
        public double Distance(IEnumerable<Vector> x, IEnumerable<Vector> y)
        {
            double distance = -1;
            double maxDistance = double.MinValue;

            for (int i = 0; i < x.Count(); i++)
            {
                for (int j = i+1; j < y.Count(); j++)
                {
                    distance = _distanceMetric.Compute(x.ElementAt(i), y.ElementAt(j));

                    if (distance > maxDistance)
                        maxDistance = distance;
                }
            }

            return maxDistance;
        }
    }
}
