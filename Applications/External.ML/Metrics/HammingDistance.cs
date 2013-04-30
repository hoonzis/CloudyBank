using System;
using ml.Math;

namespace ml.Metrics
{
    public sealed class HammingDistance : IDistance
    {
        public double Compute(Vector x, Vector y)
        {
            if (x.Length != y.Length)
                throw new InvalidOperationException("Cannot compute distance between two unequally sized Vectors!");
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
                if (x[i] != y[i])
                    sum++;
            return sum;
        }
    }
}
