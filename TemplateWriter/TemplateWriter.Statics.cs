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
    public partial class TemplateWriter 
    {
        /// <summary>
        /// Create a new instance with no variables.
        /// </summary>
        public static TemplateWriter Empty { get; } = new TemplateWriter(null);


        /// <summary>
        /// Create a TemplateWriter instance with default File keys from a FileInfo object.
        /// </summary>
        /// <returns></returns>
        public static TemplateWriter Create(System.IO.FileInfo fi)
        {
            var writer = new TemplateWriter();

            writer.Add(nameof(GlobalFileVariables.Name), Path.GetFileNameWithoutExtension(fi.Name));
            writer.Add(nameof(GlobalFileVariables.Filename), fi.Name);
            writer.Add(nameof(GlobalFileVariables.Extension), fi.Extension);
            writer.Add(nameof(GlobalFileVariables.Fullname), fi.FullName);
            writer.Add(nameof(GlobalFileVariables.Directory), fi.Directory.FullName);
            writer.Add(nameof(GlobalFileVariables.Created), fi.CreationTime);
            writer.Add(nameof(GlobalFileVariables.Modified), fi.LastWriteTime);

            return writer;
        }

        /// <summary>
        /// Create a TemplateWriter instance with default File keys from a FileInfo object.
        /// </summary>
        /// <returns></returns>
        public static TemplateWriter CreateFromObject(object source)
        {
            var type = source.GetType();
            var dict = type.GetProperties().ToDictionary(pi => pi.Name, pi => pi.GetValue(source));
            return new TemplateWriter(dict);
        }
    }
}
