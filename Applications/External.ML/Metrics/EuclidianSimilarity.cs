using System;
using ml.Math;

namespace ml.Metrics
{
    public sealed class EuclidianSimilarity : ISimilarity
    {
        public double Compute(Vector x, Vector y)
        {
            return 1 / (1 + (x - y).Norm());
        }
    }
}
