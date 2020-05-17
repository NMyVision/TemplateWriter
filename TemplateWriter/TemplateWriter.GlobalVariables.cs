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
            /// <summary>
            /// A unique GUID
            /// </summary>
            UUID,
            /// <summary>
            /// Current position when used with TemplateWriter instance, default base is 0.
            /// </summary>
            Index,
        }
    }
}
