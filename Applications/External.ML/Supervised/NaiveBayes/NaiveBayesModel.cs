/*
 Copyright (c) 2011 Jan Fajfr

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using ml.Attributes;
using ml.Math;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace ml.Supervised.NaiveBayes
{
    /// <summary>
    /// Class which performs Naive Bayesian Categorization. Free types of features are supported:
    /// Continous - Double and Decimal in default. Can be extended to other continous types.
    /// Boolean - true/false values.
    /// String - String values which are converted into vector of boolean values for each word.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NaiveBayesModel<T> : IModel<T>
    {

        public TypeDescription Description { get; set; }
        public Matrix X {get;set;}
        public Vector Y {get;set;}

        private double _itemsCount {get;set;}
        private int _featuresCount {get;set;}
        private int _categoryCount {get;set;}

        //This implementation uses Jagged arrays inside instead of multidimensional arrays.
        //There are two reasons: multidimensional arrays do not serialize (out of box)
        //and the fact that jagged arrays are faster (maybe :http://stackoverflow.com/questions/597720/what-is-differences-between-multidimensional-array-and-array-of-arrays-in-c)

        /// <summary>
        /// Stores the avarage value of certain feature of classes of a concrete category [Category][Feature]
        /// </summary>
        private double[][] CategoryFeatureAvg; 

        /// <summary>
        /// Stores the variance of concrete feature of classes of a concrete category [Category][Feature]
        /// </summary>
        private double[][] CategoryFeatureVariance;
        
        /// <summary>
        /// Stores the count of items of certain category having a certain feature [Category][Feature]
        /// </summary>
        private double[][] CategoryFeatureItemsCount;

        /// <summary>
        /// List of values of concrete feature of concrete category. This list is used to compute the variance for each Category/Feature combination.
        /// </summary>
        private List<double>[][] CategoryFeatureValues;
        
        /// <summary>
        /// Number of items of concrete category. Used to compute the Apriory probability of an item being of category.
        /// </summary>
        private double[] CategoryItemsCount;
        
        /// <summary>
        /// Apriory probability of a category being of certain categoery.
        /// </summary>
        public double[] Apriory;
        
        /// <summary>
        /// Probability that item is of a given category when having the specified feature.[cat,feature]
        /// </summary>
        public double[][] Posteriory;

        
        public IPredict<T> Generate()
        {
            if (Description == null || X == null || Y == null)
                throw new InvalidOperationException("Model Parameters Not Set!");

            BuildTheModel();

            return new NaiveBayesPredict<T>()
            {
                Posteriory = Posteriory,
                Categories = Y.Distinct().ToList(),
                Description = Description,
                Apriory = Apriory,
                CategoryFeatureAvg = CategoryFeatureAvg,
                CategoryFeatureVariance = CategoryFeatureVariance
            };
        }

        /// <summary>
        /// This implementation makes uses of arrays which store the values of variance, avg, and apriory/posteriory probabilities.
        /// Double representation of each value is used as index (during conversion) for each category.
        /// When bool categories are used, they are converted to -1, 1. This method converts them to 0,1 so that they can be used as
        /// indexes in the arrays of this implementation.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int ConvertToCategoryIdentifier(Type labelType, double val)
        {
            if (labelType == typeof(bool))
            {
                if (val == -1)
                    return 0;
                else
                    return 1;
            }
            else
            {
                return (int)val;
            }
        }

        /// <summary>
        /// because I am using Jagged Arrays I am performing the intialization of two-dimensional arrays here in the cycle
        /// </summary>
        private void InitializeArrays()
        {
            Apriory = new double[_categoryCount];
            CategoryItemsCount = new double[_categoryCount];
            CategoryFeatureVariance = new double[_categoryCount][];
            CategoryFeatureAvg = new double[_categoryCount][];
            CategoryFeatureValues = new List<double>[_categoryCount][];
            CategoryFeatureItemsCount = new double[_categoryCount][];
            Posteriory = new double[_categoryCount][];

            for (int category = 0; category < _categoryCount; category++)
            {
                CategoryFeatureAvg[category] = new double[_featuresCount];
                CategoryFeatureValues[category] = new List<double>[_featuresCount];
                CategoryFeatureItemsCount[category] = new double[_featuresCount];
                Posteriory[category] = new double[_featuresCount];
                CategoryFeatureVariance[category] = new double[_featuresCount];
            }
        }

        /// <summary>
        /// The estimation of classifier parameters has to run in two cycles. This is the Preprocess Phrase. In order to later compute
        /// the variance of each category/feature combination, the avarage of each category/feature combination has to be computed before.
        /// 
        /// The count of items for each category is computed to be used later to obtain the apriory probability.
        /// The count of items for each category/feature combination is computed to later obtain the posteriory probability.
        /// </summary>
        /// <param name="examples"></param>
        private void PreprocessData()
        {
            for (int i = 0; i < _itemsCount; i++)
            {
                int category = ConvertToCategoryIdentifier(Description.Label.Type,Y[i]);

                CategoryItemsCount[category]++;
                
                var values = X[i];
                int j = 0;
                for (int helper = 0; helper < Description.Features.Count(); helper++)
                {
                    
                    var feature = Description.Features[helper];
                    if (ContinuesTypes.Contains(feature.Type))
                    {
                        var value = values[j];
                        
                        // add the value to the avarage (later this will be divided by the count to obtain the avarage).
                        CategoryFeatureAvg[category][j] += value;

                        //have a small list of values for each feature/category 
                        if (CategoryFeatureValues[category][j] == null)
                        {
                            CategoryFeatureValues[category][j] = new List<double>();
                        }
                        CategoryFeatureItemsCount[category][j]++;
                        CategoryFeatureValues[category][j].Add(value);
                    }

                    if (feature is StringProperty)
                    {
                        var sp = feature as StringProperty;
                        var wordCount = sp.Dictionary.Count();
                        //each string is converted into binary vector.  I will loop over the vector representing the string.
                        for (int k = 0; k < wordCount; k++)
                        {
                            if (values[j + k] == 1)
                            {
                                CategoryFeatureItemsCount[category][j+k]++;
                            }
                        }
                        
                        j += wordCount-1;

                    }//string property


                    if (feature.Type == typeof(bool))
                    {
                        if (values[j] == 1)
                        {
                            CategoryFeatureItemsCount[category][j]++;
                        }
                    }
                    j++;
                }//features
            }//items
        }

        /// <summary>
        /// This method builds the model - it fills the Apriory,Posteriory, CategoryFeatureAvg and CategoryFeatureVariance with approriate values.
        /// </summary>
        private void BuildTheModel()
        {
            _itemsCount = Y.Count();
            _featuresCount = X.Cols;
            _categoryCount = Y.Distinct().Count();

            //initialize two dimensional arrays
            InitializeArrays();

            //prepare the data - avg, counts of items foreach category/feature.
            PreprocessData();

            for (int i = 0; i < _categoryCount; i++)
            {
                Apriory[i] = CategoryItemsCount[i] / _itemsCount;
                int j = 0;
                for (int featureIndex = 0; featureIndex < Description.Features.Count(); featureIndex++)
                {
                    var feature = Description.Features[featureIndex];

                    if (ContinuesTypes.Contains(feature.Type))
                    {
                        //values were added so far to this fied
                        //no divide the result by the count
                        CategoryFeatureAvg[i][j] /= CategoryFeatureItemsCount[i][j];
                        var variance = Helper.Variance(CategoryFeatureValues[i][j].ToArray(), CategoryFeatureAvg[i][j]);
                        CategoryFeatureVariance[i][j] = variance;
                    }

                    if (feature is StringProperty)
                    {

                        var sp = feature as StringProperty;
                        var wordCount = sp.Dictionary.Count();
                        for (int k = 0; k < wordCount; k++)
                        {
                            Posteriory[i][j + k] = GetProbability(i, j + k);
                        }
                        j += wordCount - 1;
                    }

                    if (feature.Type == typeof(bool))
                    {
                        Posteriory[i][j] = GetProbability(i, j);
                    }
                    j++;
                }
            }
        }

        /// <summary>
        /// Generates the NaiveBayesPredict on a base of a item set.
        /// </summary>
        /// <param name="examples"></param>
        /// <returns></returns>
        public IPredict<T> Generate(IEnumerable<T> examples)
        {
            if (Description == null || X == null || Y == null)
            {
                var data = Converter.Convert<T>(examples, Description);
                X = data.Item1;
                Y = data.Item2;
                Description = data.Item3;
            }

            return Generate();
        }

        public IPredict<T> Load(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                return Load(stream);
            }
        }

        public IPredict<T> Load(System.IO.Stream stream)
        {
            NaiveBayesPredict<T> predictor = new NaiveBayesPredict<T>();
            XmlSerializer serializer = new XmlSerializer(predictor.GetType());
            return (NaiveBayesPredict<T>)serializer.Deserialize(stream);
        }


        /// <summary>
        /// When there are no items for category/feature pair, the probability is etimated using 1/_itemsCount.
        /// This is to evict the 0 probabilities.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private double GetProbability(int i, int j)
        {
            if (CategoryFeatureItemsCount[i][j] == 0.0)
            {
                return (double)1 / _itemsCount;
            }
            else
            {
                return CategoryFeatureItemsCount[i][j] / CategoryItemsCount[i];
            }
        }

        private static Type[] _continuesTypes;
        
        /// <summary>
        /// The set of types which are considered as continous. This means that the probability of item being of a category
        /// when having a feature (which is of a type in this set), will be computed using Gaussian probability distribution.
        /// </summary>
        public static Type[] ContinuesTypes
        {
            get
            {
                if (_continuesTypes == null)
                {
                    _continuesTypes =  new Type[]{ typeof(Double), typeof(Decimal)};
                }
                return _continuesTypes;
            }
            set
            {
                _continuesTypes = value;
            }
        }
    }
}
