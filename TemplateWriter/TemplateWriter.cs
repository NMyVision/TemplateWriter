using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace NMyVision
{

    /// <summary>
    /// A wrapper class for the Formattable Extension that makes it easy to define placeholder for transforming.
    /// </summary>
    /// <remarks>
    /// Predefined template names are visible via TemplateWriter.GlobalVariables.
    /// </remarks>
    /// <seealso cref="GlobalVariables"/>
    public partial class TemplateWriter : ICloneable, IDisposable
    {
        const string INDEX_KEY = nameof(GlobalVariables.Index);

        /// <summary>
        /// Index variable start value.
        /// </summary>
        public int Seed { get; private set; }

        /// <summary>
        /// Amount to increase the Index variable when Transform is called.
        /// </summary>
        public int Increment { get; private set; }


        /// <summary>
        /// Variable prefixed used when generated from Extract method.
        /// </summary>
        public string VariablePrefix { get; set; } = "@";

        //At the heart of it this is basically a class to work with an IDictionary object
        IDictionary<string, object> _d;

        /// <summary>
        /// Constructor for TemplateWriter object.
        /// </summary>
        public TemplateWriter(DateTime? currentDateTime = null, int seed = 0, int increment = 1)
        {
            _d = GetDefaultDictionary(currentDateTime ?? DateTime.Now, seed, increment);
        }

        /// <summary>
        /// Constructor for TemplateWriter object.
        /// </summary>
        public TemplateWriter(IDictionary<string, object> source)
        {
            _d = source ?? new Dictionary<string, object>(); ;
        }


        protected virtual IDictionary<string, object> GetDefaultDictionary(DateTime now, int seed = 0, int increment = 1)
        {

            var d = new Dictionary<string, object>();

            //Predefined variables at the root level...
            d.Add(nameof(GlobalVariables.Current), now);
            d.Add(nameof(GlobalVariables.Current_Date), now.ToString("yyyyMMdd"));
            d.Add(nameof(GlobalVariables.Current_DateTime), now.ToString("yyyyMMddHHmmss"));
            d.Add(nameof(GlobalVariables.Current_Time), now.ToString("HHmmss"));
            d.Add(nameof(GlobalVariables.Current_UniqueDate), now.ToString("yyyyMMddHHmmssfff"));


            d.Add(INDEX_KEY, seed);


            this.Increment = increment;

            Seed = seed;


            return d;

        }

        /// <summary>
        /// Add values to the template writer.
        /// </summary>
        /// <param name="key">The key is used in the format string.</param>
        /// <param name="value">The value for a given key.</param>
        public TemplateWriter Add(string key, object value)
        {
            _d.Add(key, value);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public TemplateWriter Add(object value)
        {
            if (value is IDictionary<string, object>)
            {
                var expando = new ExpandoObject();
                var expandoDic = (IDictionary<string, object>)expando;
                ((IDictionary<string, object>)value).ToList().ForEach((dk) => expandoDic.Add(dk.Key, dk.Value));
                //value = expando;
            }

            return this;
        }
        /// <summary>
        /// Set or retrieve value from the template writer.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TemplateWriter Set(string key, object value)
        {
            if (_d.ContainsKey(key))
            {
                _d[key] = value;
            }
            else
            {
                _d.Add(key, value);
            }
            return this;
        }

        /// <summary>
        /// Set or retrieve value from the template writer.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(string key)
            => _d.Remove(key);


        /// <summary>
        /// Set or retrieve value from the template writer.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return _d[key]; }
            set { _d[key] = value; }
        }



        /// <summary>
        /// Parses template from an object.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string Transform(string template, object o)
            => o.ToStringFromTemplate(template);

        /// <summary>
        /// Transforms a string replacing placeholders with values.
        /// </summary>
        /// <param name="format">A string with placeholders for the keys added and predefined keys.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns></returns>
        /// <example>
        /// var x = { Name = "John" , Age = 21 }; x.ToString("{Name} {Current}.txt");
        /// </example>
        public string Transform(string format, IFormatProvider provider = null)
        {
            var result = _d.ToStringFromTemplate(format, provider);

            IncrementCounter();

            return result;
        }

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns></returns>
        public object Clone() => this.MemberwiseClone();


        /// <summary>
        /// Gets an System.Collections.Generic.ICollection containing the keys of the TemplateWriter.
        /// </summary>
        public IReadOnlyList<string> Keys => new System.Collections.ObjectModel.ReadOnlyCollection<string>(_d.Keys.ToList());



        /// <summary>
        /// Create variables based on a pattern
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="input">The string to retrieve data from.</param>
        public void Extract(string pattern, string input)
        {
            var r = new Regex(pattern, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            var groups = r.GetGroupNames().Where(x => x != "0");

            var m = r.Match(input);

            if (m.Success)
            {
                foreach (var g in groups)
                {
                    this.Add($"{VariablePrefix}{g}", m.Groups[g].Value);
                }
            }
        }

        private void IncrementCounter()
        {
            if (_d.ContainsKey(INDEX_KEY))
            {
                if (int.TryParse(this[INDEX_KEY].ToString(), out int c))
                {
                    this.Set(INDEX_KEY, c + this.Increment);
                }
            }
        }


        /// <summary>
        /// LinqPad allow use to dump the underlining object
        /// </summary>
        /// <returns></returns>
        object ToDump() => new { Dictionary = _d, VariablePrefix, Seed, Increment };

        public void Dispose()
        {
            _d = null;
        }
    }
}
