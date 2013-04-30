using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.Math;

namespace ml.Metrics
{
    public interface IDistance
    {
        double Compute(Vector x, Vector y);
    }
}
