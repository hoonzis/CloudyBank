using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.Attributes;
using ml.Math;
using ml.Metrics;
using ml.Unsupervised.Linkers;

namespace ml.Unsupervised
{
    public class HClusterModel<T>
    {
        public TypeDescription Description { get; set; }
        public ILinker Linker { get; set; }

        public HCluster Generate(IEnumerable<T> examples, ILinker linker)
        {
            // Initialize

            Linker = linker;

            var clusters = new List<HCluster>();
            var distances = new Dictionary<Tuple<int, int>, double>();

            // Load data 

            if (Description == null)
                Description = Converter.GetDescription(typeof(T)).BuildDictionaries<T>(examples);

            Matrix X = Converter.Convert<T>(examples, Description.Features);

            // Create a new cluster for each data point

            for (int i = 0; i < X.Rows; i++)
                clusters.Add(new HCluster { Id = i, Points = new Vector[] { X[i, VectorType.Row] } });

            // Set the current closest distance/pair to the first pair of clusters
            var key = new Tuple<int, int>(0, 0);
            var distance = 0.0;

            var clusterId = -1;

            while (clusters.Count > 1)
            {
                var closestClusters = new Tuple<int, int>(0, 1);
                var smallestDistance = Linker.Distance(clusters[0].Points, clusters[1].Points);
               
                // Loop through each of the clusters looking for the two closest

                for (int i = 0; i < clusters.Count; i++)
                {
                    for (int j = i+1; j < clusters.Count; j++)
                    {
                        key = new Tuple<int, int>(clusters[i].Id, clusters[j].Id);
                        
                        // Cache the distance if it hasn't been calculated yet

                        if (!distances.ContainsKey(key))
                            distances.Add(key, Linker.Distance(clusters[i].Points, clusters[j].Points));

                        // Update closest clusters and distance if necessary

                        distance = distances[key];

                        if (distance < smallestDistance)
                        {
                            smallestDistance = distance;
                            closestClusters = new Tuple<int, int>(i, j);
                        }
                    }
                }

                var min = System.Math.Min(closestClusters.Item1, closestClusters.Item2);
                var max = System.Math.Max(closestClusters.Item1, closestClusters.Item2);

                var newCluster = new HCluster(clusterId, clusters[min],
                                              clusters[max]);

                // Remove the merged clusters

                clusters.RemoveAt(min);
                clusters.RemoveAt(max - 1);

                // Add new cluster

                clusters.Add(newCluster);

                clusterId += 1;

            }

            return clusters.Single();
        }
    }
}
