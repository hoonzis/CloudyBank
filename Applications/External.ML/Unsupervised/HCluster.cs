using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.Math;

namespace ml.Unsupervised
{
    public class HCluster
    {
        public int Id { get; set; }
        public IEnumerable<Vector> Points { get; set; }
        public HCluster Left { get; set; }
        public HCluster Right { get; set; }

        public HCluster()
        {
        }

        public HCluster(int id, HCluster left, HCluster right )
        {
            Id = id;
            Left = left;
            Right = right;
            Points = left.Points.Concat(right.Points);
        }
    }
}
