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

namespace ml.Math
{
    public static class Helper
    {
        public static double Entropy(this Vector x)
        {
            return ml.Math.Entropy.Of(x).Value;
        }

        public static double Gini(this Vector x)
        {
            return ml.Math.Gini.Of(x).Value;
        }

        public static double Gauss(double x, double mean, double std)
        {
            double tmp = 1 / ((System.Math.Sqrt(2 * System.Math.PI) * std));
            var val1 = tmp * System.Math.Exp(-.5 * System.Math.Pow((x - mean) / std, 2));

            var value = 1 / System.Math.Sqrt(2 * System.Math.PI * std);
            var value3 = -System.Math.Pow(x - mean, 2) / (2 * std);
            var val = value * System.Math.Exp(value3);

            //if (val != val1)
            //{
            //    throw new Exception("Math error");
            //}

            if (val > 1 || val < 0)
            {
                throw new Exception("Probability has to stay between 0 and 1");
            }

            return val;
        }

        public static double Variance(double[] data, double mean)
        {

            double[] deviation = new double[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {
                deviation[i] = System.Math.Pow((data[i] - mean), 2);
            }


            var variance = deviation.Average();
            return variance;
        } 
    }
}
