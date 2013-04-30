using System;
using ml.Math;

namespace ml.Metrics
{
    public sealed class TanimotoCoefficient : ISimilarity
    {
        public double Compute(Vector x, Vector y)
        {
            double dot = x.Dot(y);
            return dot / (System.Math.Pow(x.Norm(), 2) + System.Math.Pow(y.Norm(), 2) - dot);
        }
    }
}
