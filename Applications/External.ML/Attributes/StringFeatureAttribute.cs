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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ml.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class StringFeatureAttribute
        : MainAttribute
    {
        public StringType SplitType { get; set; }
        public string Separator { get; set; }
        public string[] Exclusions { get; set; }
        public string ExclusionFile { get; set; }

        public StringFeatureAttribute()
        {
            SplitType = StringType.Character;
        }

        public StringFeatureAttribute(StringType splitType, string separator = " ", string exclusions = null)
        {
            SplitType = splitType;
            Separator = separator;
            ExclusionFile = exclusions;

            CalculateExclusions();
        }

        public void CalculateExclusions()
        {
            // add exclusions
            if (ExclusionFile != null && File.Exists(ExclusionFile))
            {
                Regex regex;
                if (SplitType == StringType.Word)
                    regex = new Regex(@"\w+", RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                else
                    regex = new Regex(@"\w", RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);

                List<string> exclusionList = new List<string>();
                using (StreamReader sr = new StreamReader(ExclusionFile))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var match = regex.Match(line);
                        // found something not already in list...
                        if (match.Success && !exclusionList.Contains(match.Value.Trim().ToUpperInvariant()))
                            exclusionList.Add(match.Value.Trim().ToUpperInvariant());
                    }
                }

                Exclusions = exclusionList.ToArray();
            }
            else
                Exclusions = new string[] { }; 
        }
    }
}
