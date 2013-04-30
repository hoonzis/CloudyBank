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
using System.Linq;
using System.Text;
using ml.Attributes;
using ml.Math;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;

namespace ml.Supervised.NaiveBayes
{
    public class NaiveBayesPredict<T>  : IPredict<T>
    {
        public double[][] Posteriory { get; set; }
        public double[] Apriory { get; set; }
        public double[][] CategoryFeatureAvg { get; set; }
        public double[][] CategoryFeatureVariance { get; set; }

        public List<double> Categories { get; set; }

        public TypeDescription Description { get; set; }

        
        
        public T Predict(T item)
        {
            double maxProbability = 0;
            double maxCategory = -1;

            foreach (var catHelp in Categories)
            {
                var category = NaiveBayesModel<T>.ConvertToCategoryIdentifier(Description.Label.Type,catHelp);// (int)catHelp;
                var values = Converter.Convert<T>(item, Description.Features);
                var probability = Apriory[(int)category];

                int j = 0;
                for (int i = 0; i < Description.Features.Count(); i++)
                {
                    var feature = Description.Features[i];
                    
                    if (feature is StringProperty)
                    {
                        var sp = feature as StringProperty;
                        var wordCount = sp.Dictionary.Count();
                        for (int k = 0; k < wordCount; k++)
                        {
                            if (values[j + k] == 1)
                            {
                                if (Posteriory[category][j + k] == 0)
                                {
                                    throw new Exception("Something went wrong - the posteriory probability of binary feature is 0, that should have never happened.");
                                }
                                probability = probability * Posteriory[category][j + k];
                            }
                        }
                        j += wordCount - 1;
                    }

                    if (NaiveBayesModel<T>.ContinuesTypes.Contains(feature.Type))
                    {
                        var value = values[j];
                        var normalProbability = Helper.Gauss(value, CategoryFeatureAvg[category][j], CategoryFeatureVariance[category][j]);

                        if (normalProbability == 0 || normalProbability == Double.NaN)
                        {
                            throw new Exception("The probability 0 or not a valid number");
                        }

                        probability = probability * normalProbability;
                    }

                    if (feature.Type == typeof(bool))
                    {
                        var probabilityValue = Posteriory[category][j];
                        if (probability == 0 || probability == Double.NaN)
                        {
                            throw new Exception("The probability 0 or not a valid number");
                        }
                    }
                    j++;
                }

                if (probability > maxProbability)
                {
                    maxProbability = probability;
                    maxCategory = category;
                }
            }
            
            item = Converter.SetItem<T>(item, Description.Label, maxCategory);
            return item;
        }

        public void Save(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            
            //ensure we delete the file first or we may have extra data
            if (File.Exists(filename))
            {
                // this could get an access violation but either way we 
                // don't want a pointer stuck in the app domain
                File.Delete(filename);
            }

            using (var stream = File.OpenWrite(filename))
            {
                serializer.Serialize(stream, this);
            }
        }
    }
}
