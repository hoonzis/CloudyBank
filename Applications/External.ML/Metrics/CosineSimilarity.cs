using System;
using ml.Math;

namespace ml.Metrics
{
    public sealed class CosineSimilarity : ISimilarity
    {
        public double Compute(Vector x, Vector y)
        {
            return x.Dot(y) / (x.Norm() * y.Norm());
        }
    }
}
