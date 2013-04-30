using System;
using ml.Math;

namespace ml.Metrics
{
    public sealed class CosineDistance : IDistance
    {
        public double Compute(Vector x, Vector y)
        {
            return 1 - (x.Dot(y) / (x.Norm() * y.Norm()));
        }
    }
}
