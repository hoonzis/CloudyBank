/*
 Copyright (c) 2011 Seth Juarez

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
using ml.Math;
using System.Linq;
using ml.Supervised;
using ml.Attributes;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

namespace ml
{
    public static class Converter
    {
        public static IEnumerable<string> GetChars(string s, string[] exclusions = null)
        {
            s = s.Trim().ToUpperInvariant();

            foreach (char a in s.ToCharArray())
            {
                string key = a.ToString();

                // ignore whitespace (should maybe set as option? I think it's noise)
                if (string.IsNullOrWhiteSpace(key)) continue;

                // ignore excluded items
                if (exclusions != null && exclusions.Length > 0 && exclusions.Contains(key)) 
                    continue;

                // make numbers and symbols a single feature
                // I think it is noise....
                key = char.IsSymbol(a) || char.IsPunctuation(a) || char.IsSeparator(a) ? "#SYM#" : key;
                key = char.IsNumber(a) ? "#NUM#" : key;

                yield return key;
            }
        }

        public static IEnumerable<string> GetWords(string s, string separator, string[] exclusions = null)
        {

            s = s.Trim().ToUpperInvariant();

            foreach (string w in s.Split(separator.ToCharArray()))
            {
                string key = w.Trim();

                // kill inlined stuff that creates noise
                // (like punctuation etc.)
                key = key.Aggregate("",
                    (x, a) =>
                    {
                        if (char.IsSymbol(a) || char.IsPunctuation(a) || char.IsSeparator(a))
                            return x;
                        else
                            return x + a.ToString();
                    }
                );

                // null or whitespace
                if (string.IsNullOrWhiteSpace(key)) continue;

                // if stemming or anything of that nature is going to
                // happen, it should happen here. The exclusion dictionary
                // should also be modified to take into account the 
                // excluded terms

                // in excluded list
                if (exclusions != null && exclusions.Length > 0 && exclusions.Contains(key)) 
                    continue;

                // found a number! decimal pointed numbers should work since we
                // killed all of the punctuation!
                key = key.Where(c => char.IsNumber(c)).Count() == key.Length ? "#NUM#" : key;

                yield return key;
            }
        }

        public static Dictionary<string, double> BuildDictionary<T>(IEnumerable<T> examples, StringProperty property)
        {
            Type t = typeof(T);
            Dictionary<string, double> d = new Dictionary<string, double>();
            MethodInfo m = null;
            PropertyInfo p = null;
            if (property.isMethod)
                m = t.GetMethod(property.Name, new Type[] { });
            else
                p = t.GetProperty(property.Name, property.Type);

            // for holding string
            string s = string.Empty;
            // for params/indices
            var nothing = new object[] { };

            foreach (T o in examples)
            {
                // get proper string
                s = property.isMethod ? (string)m.Invoke(o, nothing) : (string)p.GetValue(o, nothing);

                //if no value was presented
                if (s == null)
                {
                    break;
                }

                if (property.SplitType == StringType.Character)
                {
                    foreach (string key in GetChars(s, property.Exclude))
                    {
                        if (d.ContainsKey(key))
                            d[key] += 1;
                        else
                            d.Add(key, 1);
                    }
                }
                else if (property.SplitType == StringType.Word)
                {
                    
                    foreach (string key in GetWords(s, property.Separator, property.Exclude))
                    {
                        if (d.ContainsKey(key))
                            d[key] += 1;
                        else
                            d.Add(key, 1);
                    }
                }
            }

            // remove words occurring only once !! NOT A GOOD
            // IDEA WHEN TRIMMING OUT THINGS...
            //var remove = d.Where(kv => kv.Value == 1).Select(kv => kv.Key).ToArray();
            //for (int i = 0; i < remove.Length; i++)
            //    d.Remove(remove[i]);
            

            // calculate relative term weight
            var sum = d.Select(kv => kv.Value).ToArray().Sum();
            foreach (var key in d.Select(kv => kv.Key).ToArray())
                d[key] /= sum;

            return d;
        }

        public static TypeDescription GetDescription(Type t)
        {
            List<Property> feat = new List<Property>();
            List<LabelProperty> targ = new List<LabelProperty>();

            // go through properties
            foreach (PropertyInfo property in t.GetProperties())
            {
                var feature = property.GetCustomAttributes(typeof(MainAttribute), false);

                // nothing to see here
                if (feature.Length == 0) continue;

                if (feature.Length == 1 && feature[0] is FeatureAttribute)
                {
                    feat.Add(new Property
                    {
                        isMethod = false,
                        Name = property.Name,
                        Type = property.PropertyType
                    });
                }

                if (feature.Length == 1 && feature[0] is StringFeatureAttribute)
                {
                    if (property.PropertyType != typeof(string))
                        throw new InvalidOperationException(string.Format("Cannot mark {0} as a StringFeature since it is not of type string!", property.Name));

                    StringFeatureAttribute s = feature[0] as StringFeatureAttribute;

                    // calculate dictionary exclusions
                    s.CalculateExclusions();

                    feat.Add(new StringProperty
                    {
                        isMethod = false,
                        Name = property.Name,
                        Type = property.PropertyType,
                        Separator = s.Separator ?? " ",
                        SplitType = s.SplitType,
                        Exclude = s.Exclusions
                    });
                }

                // this makes it so that only one learn attribute survives
                if (feature.Length == 1 && feature[0] is LabelAttribute)
                {
                    targ.Add(new LabelProperty
                    {
                        isMethod = false,
                        Name = property.Name,
                        Type = property.PropertyType
                    });
                }
            }

            // go through methods
            foreach (MethodInfo method in t.GetMethods())
            {
                var feature = method.GetCustomAttributes(typeof(MainAttribute), false);

                // nothing to see here
                if (feature.Length == 0) continue;

                if (method.GetGenericArguments().Length > 0)
                    throw new InvalidOperationException("Feature and Label methods cannot take paramters!");

                if (feature.Length == 1 && feature[0] is FeatureAttribute)
                {
                    feat.Add(new Property
                    {
                        isMethod = true,
                        Name = method.Name,
                        Type = method.ReturnType
                    });
                }

                if (feature.Length == 1 && feature[0] is StringFeatureAttribute)
                {
                    if (method.ReturnType != typeof(string))
                        throw new InvalidOperationException(string.Format("Cannot mark {0} as a StringFeature since it is not of type string!", method.Name));

                    StringFeatureAttribute s = feature[0] as StringFeatureAttribute;

                    // calculate dictionary exclusions
                    s.CalculateExclusions();

                    feat.Add(new StringProperty
                    {
                        isMethod = true,
                        Name = method.Name,
                        Type = method.ReturnType,
                        Separator = s.Separator ?? " ",
                        SplitType = s.SplitType,
                        Exclude = s.Exclusions
                    });
                }

                // this makes it so that only one learn attribute survives
                if (feature.Length == 1 && feature[0] is LabelAttribute)
                {
                    targ.Add(new LabelProperty
                    {
                        isMethod = true,
                        Name = method.Name,
                        Type = method.ReturnType
                    });
                }
            }

            return new TypeDescription
            {
                Features = feat.ToArray(),
                Label = (LabelProperty)(targ.Count == 1 ? targ[0] : null)
            };

        }

        public static TypeDescription BuildDictionaries<T>(this TypeDescription desc, IEnumerable<T> examples)
        {
            // build dictionaries for string properties
            foreach (var p in desc.Features.Where(p => p is StringProperty))
            {
                var sprop = p as StringProperty;

                // get dictionary of terms/chars to use in word vector
                var d = BuildDictionary<T>(examples, p as StringProperty);

                List<string> bagOfWords = new List<string>();

                // just add them all together
                // need to look into eliminating
                // *bad* tokens...
                foreach (var kv in d)
                    bagOfWords.Add(kv.Key);

                sprop.Dictionary = bagOfWords.ToArray();
            }

            return desc;
        }

        public static double[] GetWordCount(string item, StringProperty desc)
        {
            double[] counts = new double[desc.Dictionary.Length];

            //if there property was somehow null - have to deal with it as well
            if (item == null)
            {
                return counts;
            }

            var d = new Dictionary<string, int>();

            for (int i = 0; i < counts.Length; i++)
            {
                counts[i] = 0;
                // for quick index lookup
                d.Add(desc.Dictionary[i], i);
            }

            IEnumerable<string> words = desc.SplitType == StringType.Character ? GetChars(item) : GetWords(item, desc.Separator);

            foreach (var s in words)
            {
                if (d.ContainsKey(s))
                    counts[d[s]]++;
            }

            return counts;
        }

        public static Vector Convert<T>(T o, Property[] features)
        {
            var t = typeof(T);

            // get true feature count
            var d = features
                        .Where(p => !(p is StringProperty)).Count() +
                    features
                        .Where(p => p is StringProperty)
                        .Aggregate(0, (no, p) => no += (p as StringProperty).Dictionary.Length);

            var X = new double[d];

            int i = 0;
            foreach (var f in features)
            {
                var item = Conversion.Converter.GetItem<T>(o, f);

                if (f is StringProperty)
                {
                    var wc = GetWordCount((string)item, f as StringProperty);
                    for (int k = 0; k < wc.Length; k++)
                    {
                        X[i] = wc[k];
                        i++;
                    }
                }
                else
                {
                    X[i] = Conversion.Converter.Convert(item);
                    i++;
                }
            }

            return new Vector(X);
        }

        public static Tuple<Matrix, Vector, TypeDescription> Convert<T>(IEnumerable<T> examples, TypeDescription description = null)
        {
            var t = typeof(T);
            Dictionary<Object, int> labelsDictionary = new Dictionary<Object, int>();
            int labelsCounter = 0;
            Dictionary<int, Object> labelsDictionaryReversed = new Dictionary<int, Object>();

            // build up type descriptions along with dictionary
            if (description == null)
                description = GetDescription(t)
                                .BuildDictionaries<T>(examples);


            // number of examples
            var n = examples.Count();

            // this in essence counts all non-string properties singly
            // and adds the correspondin dictionary lengths for all
            // string features
            var d = description.Features
                        .Where(p => !(p is StringProperty)).Count() +
                    description.Features
                        .Where(p => p is StringProperty)
                        .Aggregate(0, (no, p) => no += (p as StringProperty).Dictionary.Length);

            var X = new double[n, d];
            var Y = new double[n];
            int i = 0;

            foreach (var example in examples)
            {
                int j = 0;
                foreach (var f in description.Features)
                {
                    var item = Conversion.Converter.GetItem<T>(example, f);

                    if (f is StringProperty)
                    {
                        var s = f as StringProperty;

                        var wc = GetWordCount((string)item, s);
                        for (int k = 0; k < wc.Length; k++)
                        {
                            X[i, j] = wc[k];
                            j++;
                        }

                    }
                    else
                    {
                        X[i, j] = Conversion.Converter.Convert(item);
                        j++;
                    }
                }

                // getting label
                var y = Conversion.Converter.GetItem<T>(example, description.Label);

                //building the labesl dictionary - because later we have to work with doubles/integers
                //which represent the labels
                if (!labelsDictionary.ContainsKey(y))
                {
                    labelsDictionary.Add(y, labelsCounter);
                    labelsDictionaryReversed.Add(labelsCounter, y);
                    labelsCounter++;

                }

                
                Y[i] = labelsDictionary[y];
                
                i++;
            }

            description.Label.Labels = labelsDictionaryReversed.Select(x => x.Value).ToArray();



            // create vectors for each example
            return new Tuple<Matrix, Vector, TypeDescription>(
                        new Matrix(X),
                        new Vector(Y),
                        description);
        }

        public static Matrix Convert<T>(IEnumerable<T> examples, Property[] features)
        {
            // type of item
            var t = typeof(T);

            // number of examples
            var n = examples.Count();

            // number of features
            var d = features
                        .Where(p => !(p is StringProperty)).Count() +
                    features
                        .Where(p => p is StringProperty)
                        .Aggregate(0, (no, p) => no += (p as StringProperty).Dictionary.Length);

            // setting up matrix array
            var X = new double[n, d];
            int i = 0;

            foreach (var example in examples)
            {
                int j = 0;
                foreach (var f in features)
                {
                    var item = Conversion.Converter.GetItem<T>(example, f);

                    if (f is StringProperty)
                    {
                        var s = f as StringProperty;

                        var wc = GetWordCount((string)item, s);
                        for (int k = 0; k < wc.Length; k++)
                        {
                            X[i, j] = wc[k];
                            j++;
                        }

                    }
                    else
                    {
                        X[i, j] = Conversion.Converter.Convert(item);
                        j++;
                    }
                }

                i++;
            }

            return new Matrix(X);
        }

        internal static T SetItem<T>(T example, Property property, object value)
        {
            if (property.isMethod)
                throw new InvalidOperationException("Cannot set the value of a method!");

            var p = typeof(T).GetProperty(property.Name, property.Type);

            //if property was a label of string and dictionary was build
            //then use the dictionary to set the value to the correct string
            if (property is LabelProperty)// && property.Type == typeof(String))
            {
                int intValue = System.Convert.ToInt32(value);
                var labelValue = (property as LabelProperty).Labels[intValue];
                p.SetValue(example, labelValue, null);
                return example;
            }

            if (!p.PropertyType.IsEnum)
            {
                if(p.PropertyType == typeof(bool) && value.GetType() == typeof(double))
                    p.SetValue(example, (double)value > 0, null);
                else
                    p.SetValue(example, System.Convert.ChangeType(value, property.Type), null);
            }
            else
            {
                p.SetValue(example, Conversion.Converter.ConvertToEnum(value, property.Type), null);
            }
            return example;
        }
    }
}
