﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DataDictionary = System.Collections.Generic.Dictionary<string, object>;
using IDataDictionary = System.Collections.Generic.IDictionary<string, object>;

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

        //At the heart of it this is basically a class to work with an IDictionary object
        IDataDictionary _d;

        /// <summary>
        /// Index variable start value.
        /// </summary>
        public int Seed { get; private set; }

        /// <summary>
        /// Amount to increase the Index variable when Transform is called.
        /// </summary>
        public int IncrementValue { get; private set; }

        /// <summary>
        /// Deterimines if after every Transform will the Index variable increment.
        /// </summary>
        public bool AutoIncrement { get; private set; }

        /// <summary>
        /// Variable prefixed used when generated from Extract method.
        /// </summary>
        public string VariablePrefix { get; set; } = "@";

        /// <summary>
        /// Constructor for TemplateWriter object.
        /// </summary>
        public TemplateWriter(DateTime? currentDateTime = null, int seed = 0, int increment = 1, bool autoIncrement = true)
        {
            _d = GetDefaultDictionary(null, currentDateTime ?? DateTime.Now, seed, increment, autoIncrement);
        }

        /// <summary>
        /// Constructor for TemplateWriter object.
        /// </summary>
        public TemplateWriter(IDataDictionary source)
        {
            if (source == null)
                throw new NullReferenceException("Source can not be null, if you want an empty TemplateWriter use TemplateWriter.Empty ");

            _d = source;
        }


        protected virtual IDataDictionary GetDefaultDictionary(IDataDictionary d, DateTime now, int seed = 0, int increment = 1, bool autoIncrement = true)
        {

            if (d == null) d = new DataDictionary();

            //Predefined variables at the root level...
            d.Add(nameof(GlobalVariables.Current), now);
            d.Add(nameof(GlobalVariables.Current_Date), now.ToString("yyyyMMdd"));
            d.Add(nameof(GlobalVariables.Current_DateTime), now.ToString("yyyyMMddHHmmss"));
            d.Add(nameof(GlobalVariables.Current_Time), now.ToString("HHmmss"));
            d.Add(nameof(GlobalVariables.Current_UniqueDate), now.ToString("yyyyMMddHHmmssfff"));

            d.Add(INDEX_KEY, seed);

            AutoIncrement = autoIncrement;
            IncrementValue = increment;
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
        /// Load a dictionary object
        /// </summary>
        /// <param name="value"></param>
        public void Load(object value)
        {
            if (value == null)
                throw new NullReferenceException($"Source '{nameof(value)}' can not be null, if you want an empty TemplateWriter use TemplateWriter.Empty ");

            var idex = new InvalidDataException("value must be an anonymous object or class or IDictionay<string, object>");

            var type = value.GetType();
            if (value is IEnumerable)
            {
                throw idex;
            }
            else if (value is IDictionary<string, object>)
            {
                var expando = new ExpandoObject();
                var expandoDic = (IDataDictionary)expando;
                ((IDataDictionary)value).ToList().ForEach((dk) => Set(dk.Key, dk.Value));
                value = expando;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var lkkey = type.GetProperty("Key").GetValue(value, null);
                var lkvalue = type.GetProperty("Value").GetValue(value, null);
                Set(lkkey.ToString(), lkvalue);
            }
            else if (!(value is String) && type.IsClass)
            {
                type
                    .GetProperties()
                    .ToList()
                    .ForEach(pi => Set(pi.Name, pi.GetValue(value)));
            }
            else
                throw idex;
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
        /// The Index variable value
        /// </summary>
        public int? CurrentIndex
        {
            get
            {
                return this._d.Keys.Contains(INDEX_KEY) ? (int?)this[INDEX_KEY] : null;
            }
            set
            {
                this.Set(INDEX_KEY, value);
            }
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
        /// Removes all variables.
        /// </summary>
        /// <param name="all">If true clear the global variables</param>
        /// <param name="resetIndex">Set the Index back to the original seed value</param>
        public void Clear(bool all = false, bool resetIndex = true)
        {

            if (all)
            {
                _d.Clear();
            }
            else
            {
                // Reset the index
                if (resetIndex)
                    Set(INDEX_KEY, Seed);

                var keys = _d.Keys.ToList();
                for (int i = keys.Count - 1; i >= 0; i--)
                {
                    var key = keys[i];
                    if (Enum.IsDefined(typeof(GlobalFileVariables), key) || Enum.IsDefined(typeof(GlobalVariables), key))
                        continue;

                    _d.Remove(key);
                }
            }
        }

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
        /// <param name="template">Template that will be used to generate the text output.</param>
        /// <param name="source">Class or Anonymous object, where the properties will be used for the template placeholders.</param>
        /// <returns></returns>
        /// <example>
        /// var x = { Name = "John" , Age = 21 };
        /// Transform("{Name} {Current}.txt", x);
        /// </example>
        public static string Transform(string template, object source, IFormatProvider provider = null)
        => TemplateWriter.Empty.ToStringFromTemplate(source, template, provider);


        /// <summary>
        /// Transforms a string replacing placeholders with values.
        /// </summary>
        /// <param name="format">A string with placeholders for the keys added and predefined keys.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns></returns>
        public string Transform(string format, IFormatProvider provider = null)
        {
            Set(nameof(GlobalVariables.UUID), Guid.NewGuid());
            var result = ToStringFromTemplate(_d, format, provider);

            if (AutoIncrement)
                Increment();

            return result;
        }

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns></returns>
        public object Clone() => MemberwiseClone();


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
                    Add($"{VariablePrefix}{g}", m.Groups[g].Value);
                }
            }
        }

        /// <summary>
        /// Increment the index variable
        /// </summary>
        public void Increment(int? value = null)
        {
            if (_d.ContainsKey(INDEX_KEY))
            {
                CurrentIndex = CurrentIndex + (value ?? IncrementValue);
            }
        }


        /// <summary>
        /// LinqPad allow use to dump the underlining object
        /// </summary>
        /// <returns></returns>
        object ToDump() => new { Dictionary = _d, VariablePrefix, Seed, IncrementValue };

        public void Dispose()
        {
            _d = null;
        }
    }
}
