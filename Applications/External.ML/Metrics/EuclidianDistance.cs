using System;
using ml.Math;

namespace ml.Metrics
{
    public sealed class EuclidianDistance : IDistance
    {
        public double Compute(Vector x, Vector y)
        {
            return (x - y).Norm();
        }
    }
}
