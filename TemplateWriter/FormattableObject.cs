using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NMyVision
{
    /// <summary>
    /// Extension for templating objects to strings.
    /// </summary>


    public class ResolverOptions
    {
        public string Variable { get; }
        public string Format { get; }
        public string Source { get; }
        public IFormatProvider formatProvider { get; }
        public Type retrievedType { get; }
        public object retrievedObject { get; }

        public ResolverOptions(string Variable, string Format, string Source, IFormatProvider formatProvider, Type retrievedType = null, object retrievedObject = null)
        {
            this.Variable = Variable;
            this.Format = Format;
            this.Source = Source;
            this.formatProvider = formatProvider;
            this.retrievedType = retrievedType;
            this.retrievedObject = retrievedObject;
        }

        internal void Deconstruct(out string Variable, out string Format, out string Source, out IFormatProvider formatProvider, out Type retrievedType, out object retrievedObject)
        {
            Variable = this.Variable;
            Format = this.Format;
            Source = this.Source;
            formatProvider = this.formatProvider;
            retrievedType = this.retrievedType;
            retrievedObject = this.retrievedObject;
        }

        internal void Deconstruct(out string Format, out IFormatProvider formatProvider, out Type retrievedType, out object retrievedObject)
        {
            Format = this.Format;
            formatProvider = this.formatProvider;
            retrievedType = this.retrievedType;
            retrievedObject = this.retrievedObject;
        }
    }

    /// <summary>
    /// Extension for templating objects to strings.
    /// </summary>
    partial class TemplateWriter
    {

        public delegate string Resolver(ResolverOptions options);

        internal BindingFlags defaultBindings => BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase;

        //originated:
        //http://www.hanselman.com/blog/ASmarterOrPureEvilToStringWithExtensionMethods.aspx
        //modified to parse object properties
        /// <summary>
        /// Takes an object (including anonymous type) and allows a string to format the output.
        /// </summary>
        /// <param name="anObject">Object being extended.</param>
        /// <param name="aFormat">Template format</param>
        /// <param name="formatProvider"></param>
        /// <param name="resolver">function used to resolve variables to values</param>
        /// <example>new { Name = new { FirstName = "John", LastName = "Doe" }, Age = 20 }.ToStringFromTemplate("{Name.FirstName} {Name.LastName} {Age}")</example>
        /// <returns></returns>
        public virtual string ToStringFromTemplate(object anObject, string aFormat, IFormatProvider formatProvider = null)
        {
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

                string toGet = string.Empty;
                string toFormat = string.Empty;

                toGet = a[0].Trim();
                if (a.Length > 1)
                {
                    toFormat = a[1];
                }

                Type retrievedType = null;
                var retrievedObject = GetPropValue(anObject, toGet, out retrievedType);


                if (retrievedType != null) //Cool, we found something
                {
                    // strings with format might actually be dates lets check
                    if (retrievedType == typeof(string) && toFormat.Length > 0)
                    {
                        if (DateTime.TryParse(retrievedObject.ToString(), out var d))
                        {
                            retrievedType = typeof(DateTime);
                            retrievedObject = d;
                        }
                    }

                    var ro = new ResolverOptions(toGet, toFormat, g.Value, formatProvider, retrievedType, retrievedObject);

                    sb.Append(DefaultResolver(ro));
                }
                else //didn't find a property with that name, so be gracious and put it back
                {
                    var ro = new ResolverOptions(a[0], toFormat, g.Value, formatProvider);
                    var e = new ResolverEventArgs($"{{{g.Value}}}", false, ro);

                    @OnResolve(this, e);

                    sb.Append(e.Result);
                }

                startIndex = g.Index + g.Length + 1;
            }
            if (startIndex < aFormat.Length) //include the rest (end) of the string
            {
                sb.Append(aFormat.Substring(startIndex));
            }
            return sb.ToString();
        }


        public virtual string DefaultResolver(ResolverOptions options)
        {
            (string toFormat, IFormatProvider formatProvider, Type retrievedType, object retrievedObject) = options;
            string result = string.Empty;

            // Add support to do substring on String properties
            if (retrievedType == typeof(string) && toFormat.Length > 0)
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
            else if (toFormat == string.Empty) //no format info
            {
                result = retrievedType.InvokeMember("ToString", defaultBindings, null, retrievedObject, null) as string;
            }
            else //format info
            {
                result = retrievedType.InvokeMember("ToString", defaultBindings, null, retrievedObject, new object[] { toFormat, formatProvider }) as string;
            }

            var e = new ResolverEventArgs(result, true, options);

            @OnResolve(this, e);

            result = e.Result;

            return result;
        }

        // still testing
        private EventHandler<ResolverEventArgs> @OnResolve = (sender, e) => { };

        //Get the property value
        protected virtual object GetPropValue(object obj, string propName, out Type type)
        {
            PropertyInfo pi;
            type = null;
            var nameParts = propName.Split('.');

            foreach (string part in nameParts)
            {
                if (obj == null) { type = null; return null; }

                if (obj is System.Dynamic.ExpandoObject || obj is IDictionary<string, object>)
                {
                    if (((IDictionary<string, object>)obj).ContainsKey(part))
                    {
                        obj = ((IDictionary<string, object>)obj)[part];
                        type = obj.GetType();
                    }
                    else
                    {
                        type = null;
                        return null;
                    }
                }
                else
                {
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

    public class ResolverEventArgs : EventArgs
    {
        public bool Resolved { get; internal set; }
        public string Result { get; set; }
        public ResolverOptions Options { get; internal set; }

        public ResolverEventArgs(string result, bool resolved, ResolverOptions options)
        {
            this.Resolved = resolved;
            this.Result = result;
            this.Options = options;
        }

    }
}