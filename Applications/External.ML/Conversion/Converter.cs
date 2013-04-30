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
using System.ComponentModel;
using System.Reflection;
using ml.Attributes;
using System.Diagnostics;

namespace ml.Conversion
{
    internal static class Converter
    {
        internal static double ConvertBoolPlusMinus(bool o)
        {
            return o ? 1 : -1;
        }

        internal static double ConvertBoolOneZero(bool o)
        {
            return o ? 1 : 0;
        }

        internal static double ConvertEnum(Enum o)
        {
            return (int)System.Convert.ChangeType(o, typeof(int));
        }

        /// <summary>
        /// Make every effort to convert value into a type of T.
        /// </summary>
        /// <remarks>
        /// value is first converted to the enumerations UnderlyingType. This is because if value is a single or
        /// double then calling System.Enum.ToObject first will raise a conversion exception.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static object ConvertToEnum(object value, Type type)
        {
            var numericValue = System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(type));
            object enumValue = System.Enum.ToObject(type, numericValue);

            return enumValue;
        }

        internal static double ConvertNumber(object o)
        {

            if (o.GetType() == typeof(double))
                return (double)o;
            else if (o.GetType() == typeof(char))
                return (int)Encoding.ASCII.GetBytes(new char[] { (char)o })[0];
            else
            {
                var converter = TypeDescriptor.GetConverter(o.GetType());
                if (converter.CanConvertTo(typeof(double)))
                    return (double)converter.ConvertTo(o, typeof(double));
                else
                    throw new InvalidOperationException(string.Format("Cannot convert {0} to double!", o.GetType()));
            }
        }

        internal static double ConvertString(string o)
        {
            return o.Length;
        }

        internal static double Convert(object o)
        {
            Type t = o.GetType();
            TypeConverter converter = TypeDescriptor.GetConverter(t);
            if (converter.CanConvertTo(typeof(double)) || t == typeof(char))
                return ConvertNumber(o);
            else
            {
                if (t == typeof(string))
                    return ConvertString((string)o);
                else if (t == typeof(bool))
                    return ConvertBoolPlusMinus((bool)o);
                else if (t.BaseType == typeof(Enum))
                    return ConvertEnum((Enum)o);
                else
                {
                    var idProperty = t.GetProperty("Id");
                    if (idProperty != null)
                    {
                        //check to see if the object has ID property
                        return System.Convert.ToDouble(idProperty.GetValue(o, null));
                    }
                    else
                    {
                        throw new InvalidCastException(string.Format("Cannot convert {0} to double", t));
                    }
                }

            }
        }

        internal static object GetItem<T>(T example, Property property)
        {
            // getting label
            return property.isMethod ?
                        typeof(T).GetMethod(property.Name, new Type[] { }).Invoke(example, new object[] { }) :
                        typeof(T).GetProperty(property.Name, property.Type).GetValue(example, new object[] { });
        }
    }
}
