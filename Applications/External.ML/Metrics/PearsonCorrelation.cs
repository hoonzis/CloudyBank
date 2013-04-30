using System;
using ml.Math;

namespace ml.Metrics
{
    public sealed class PearsonCorrelation : ISimilarity
    {
        public double Compute(Vector x, Vector y)
        {
            if (x.Length != y.Length)
                throw new InvalidOperationException("Cannot compute similarity between two unequally sized Vectors!");

            var xSum = x.Sum();
            var ySum = y.Sum();
            return (x.Dot(y) - ((xSum * ySum) / x.Length)) / System.Math.Sqrt(((x ^ 2).Sum() - (System.Math.Pow(xSum, 2) / x.Length)) * ((y ^ 2).Sum() - (System.Math.Pow(ySum, 2) / y.Length)));
        }
    }
}
