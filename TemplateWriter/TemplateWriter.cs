using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace NMyVision
{

    /// <summary>
    /// A wrapper class for the Formattable Extension that makes it easy to define placeholder for transforming.
    /// </summary>
    /// <remarks>
    /// Predefined template names are visible via TemplateWriter.GlobalVariables.
    /// </remarks>
    /// <seealso cref="GlobalVariables"/>
    public class TemplateWriter : ICloneable
    {

        /// <summary>
        /// Public enumeration of predefined variables that are available these are not intented to be used directly.
        /// </summary>
        public enum GlobalVariables
        {
            /// <summary>
            /// Current date/time use this when you want to provide a custom format
            /// </summary>
            Current,
            /// <summary>
            /// Current date in yyyyMMdd format
            /// </summary>
            Current_Date,
            /// <summary>
            /// Current date/time in yyyyMMddHHmmss format
            /// </summary>
            Current_DateTime,
            /// <summary>
            /// Current date/time in HHmmss format
            /// </summary>
            Current_Time,
            /// <summary>
            /// Current date/time in yyyyMMddHHmmssfff format
            /// </summary>
            Current_UniqueDate,
        }

        //At the heart of it this is basically a class to work with an IDictionary object
        IDictionary<string, object> _d;

        /// <summary>
        /// Constructor for TemplateWriter object.
        /// </summary>
        public TemplateWriter(DateTime? currentDateTime = null)
        {
            _d = new Dictionary<string, object>();

            var now = currentDateTime ?? DateTime.Now;
            //Predefined variables at the root level...
            _d.Add(GlobalVariables.Current.ToString(), now);
            _d.Add(GlobalVariables.Current_Date.ToString(), now.ToString("yyyyMMdd"));
            _d.Add(GlobalVariables.Current_DateTime.ToString(), now.ToString("yyyyMMddHHmmss"));
            _d.Add(GlobalVariables.Current_Time.ToString(), now.ToString("HHmmss"));
            _d.Add(GlobalVariables.Current_UniqueDate.ToString(), now.ToString("yyyyMMddHHmmssfff"));
        }

        /// <summary>
        /// Add values to the template writer.
        /// </summary>
        /// <param name="key">The key is used in the format string.</param>
        /// <param name="value">The value for a given key.</param>
        public void Add(string key, object value) => _d.Add(key, value);

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
        /// Transforms a string replacing placeholders with values.
        /// </summary>
        /// <param name="format">A string with placeholders for the keys added and predefined keys.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns></returns>
        /// <example>
        /// var x = { Name = "John" , Age = 21 }; x.ToString("{Name} {Current}.txt");
        /// </example>
        public string Transform(string format, IFormatProvider provider = null) => _d.ToStringFromTemplate(format, provider);

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns></returns>
        public object Clone() => this.MemberwiseClone();


        /// <summary>
        /// Gets an System.Collections.Generic.ICollection containing the keys of the TemplateWriter.
        /// </summary>
        public IEnumerable<string> Keys => _d.Keys;

        /// <summary>
        /// Create a TemplateWriter instance with default File keys from a FileInfo object.
        /// </summary>
        /// <returns></returns>
        public static TemplateWriter Create(System.IO.FileInfo fi)
        {
            var writer = new TemplateWriter();

            writer.Add("Name", Path.GetFileNameWithoutExtension(fi.Name));
            writer.Add("Filename", fi.Name);
            writer.Add("Extension", fi.Extension);
            writer.Add("Fullname", fi.FullName);
            writer.Add("Directory", fi.Directory.FullName);
            writer.Add("Created", fi.CreationTime);
            writer.Add("Modified", fi.LastWriteTime);

            return writer;
        }

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
                foreach(var g in groups)
                {
                    this.Add($"{VariablePrefix}{g}", m.Groups[g].Value);
                }
            }
        }

        public string VariablePrefix = "@";
    }
}
