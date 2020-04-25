using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NMyVision
{
    /// <summary>
    /// Extension for templating objects to strings.
    /// </summary>
    internal static class FormattableObjectExtension
    {
        //originated:
        //http://www.hanselman.com/blog/ASmarterOrPureEvilToStringWithExtensionMethods.aspx
        //modified to parse object properties
        /// <summary>
        /// Takes an object (including anonymous type) and allows a string to format the output.
        /// </summary>
        /// <param name="anObject">Object being extended.</param>
        /// <param name="aFormat">Template format</param>
        /// <param name="formatProvider"></param>
        /// <example>new { Name = new { FirstName = "John", LastName = "Doe" }, Age = 20 }.ToStringFromTemplate("{Name.FirstName} {Name.LastName} {Age}")</example>
        /// <returns></returns>
        public static string ToStringFromTemplate(this object anObject, string aFormat, IFormatProvider formatProvider = null)
        {
            BindingFlags defaultBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase;
            var sb = new StringBuilder();
            var type = anObject.GetType();
            var reg = new Regex(@"({)([^}]+)(})", RegexOptions.IgnoreCase);
            var mc = reg.Matches(aFormat);
            int startIndex = 0;
            foreach (Match m in mc)
            {
                Group g = m.Groups[2]; //it's second in the match between { and }
                int length = g.Index - startIndex - 1;
                sb.Append(aFormat.Substring(startIndex, length));


                var a = g.Value.Split(':');

                string toGet = String.Empty;
                string toFormat = String.Empty;
                string toCase = String.Empty;

                toGet = a[0];
                if (a.Length > 1)
                {
                    toFormat = a[1];
                    if (a.Length > 2)
                        toCase = a[2];
                }

                Type retrievedType = null;
                var retrievedObject = GetPropValue(anObject, toGet, out retrievedType);


                if (retrievedType != null) //Cool, we found something
                {
                    string result = String.Empty;

                    // strings with format might actually be dates lets check
                    if (retrievedType == typeof(string) && toFormat.Length > 0)
                    {
                        if (DateTime.TryParse(retrievedObject.ToString(), out var d))
                        {
                            retrievedType = typeof(DateTime);
                            retrievedObject = d;
                        }
                    }

                    // Add support to do substring on String properties
                    if (retrievedType == typeof(String) && toFormat.Length > 0)
                    {
                        result = retrievedType.InvokeMember("ToString", defaultBindings, null, retrievedObject, null) as string;
                        if (toFormat.Contains(",")) //only substring if comma is present
                        {
                            var int_array = toFormat.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s) as object).ToArray();

                            result = retrievedType.InvokeMember("Substring", defaultBindings, null, result, int_array) as string;
                        }
                        // allow formatting of numbers ie: if i = 12 and template is {i:000} output 012.
                        else if (toFormat.Contains("#") || toFormat.Contains("0"))
                        {
                            if (int.TryParse(retrievedObject.ToString(), out var i))
                            {
                                retrievedType = typeof(int);
                                retrievedObject = i;
                                result = retrievedType.InvokeMember("ToString", defaultBindings, null, retrievedObject, new object[] { toFormat, formatProvider }) as string;
                            }
                        }
                    }
                    else if (toFormat == String.Empty) //no format info
                    {
                        result = retrievedType.InvokeMember("ToString", defaultBindings, null, retrievedObject, null) as string;
                    }
                    else //format info
                    {
                        result = retrievedType.InvokeMember("ToString", defaultBindings, null, retrievedObject, new object[] { toFormat, formatProvider }) as string;
                    }
                    // not sure how helpful this is... maybe pass in an options object instead
                    //if (!string.IsNullOrEmpty(toCase))
                    //{
                    //    if (toCase.ToUpper().Contains("L")) result = result.ToLower();
                    //    else if (toCase.ToUpper().Contains("U")) result = result.ToUpper();

                    //    if (toCase.ToUpper().Contains("S")) result = result.Replace(' ', '_');
                    //}

                    sb.Append(result);
                }
                else //didn't find a property with that name, so be gracious and put it back
                {
                    sb.Append("{");
                    sb.Append(g.Value);
                    sb.Append("}");
                }
                startIndex = g.Index + g.Length + 1;
            }
            if (startIndex < aFormat.Length) //include the rest (end) of the string
            {
                sb.Append(aFormat.Substring(startIndex));
            }
            return sb.ToString();
        }

        //Get the property value
        internal static Object GetPropValue(Object obj, String propName, out Type type)
        {
            PropertyInfo pi;
            type = null;
            var nameParts = propName.Split('.');

            foreach (String part in nameParts)
            {
                if (obj == null) { type = null; return null; }

                if (obj is System.Dynamic.ExpandoObject || obj is IDictionary<string, object>)
                {
                    if (((IDictionary<string, object>)obj).ContainsKey(part))
                    {
                        obj = ((IDictionary<string, object>)obj)[part];
                        type = obj.GetType();
                    }
                    else {
                        type = null;
                        return null;
                    }
                }
                else {
                    type = obj.GetType();
                    pi = type.GetProperty(part);
                    if (pi == null) { type = null; return null; }

                    obj = pi.GetValue(obj, null);
                    type = pi.PropertyType;
                }
            }

            return obj;
        }
    }
}