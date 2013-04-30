using System;
using ml.Math;

namespace ml.Metrics
{
    public sealed class ManhattanDistance : IDistance
    {
        public double Compute(Vector x, Vector y)
        {
            return (x - y).LpNorm(1);
        }
    }
}
