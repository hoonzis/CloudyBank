using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.Math;

namespace ml.Unsupervised.Linkers
{
    public interface ILinker
    {
        double Distance(IEnumerable<Vector> x, IEnumerable<Vector> y);
    }
}
