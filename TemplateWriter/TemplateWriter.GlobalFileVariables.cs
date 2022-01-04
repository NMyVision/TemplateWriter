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
        public enum GlobalFileVariables
        {
            /// <summary>
            /// File name without extension
            /// </summary>
            Name,
            /// <summary>
            /// File path
            /// </summary>
            Filename,
            /// <summary>
            /// File name extension
            /// </summary>
            Extension,
            /// <summary>
            /// File name with extension
            /// </summary>
            Fullname,
            /// <summary>
            ///  Directory file is in
            /// </summary>
            Directory,
            /// <summary>
            /// File creation date
            /// </summary>
            Created,
            /// <summary>
            /// File last write date
            /// </summary>
            Modified
        }
    }
}
