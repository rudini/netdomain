namespace netdomain
{
    using System.Collections.Generic;
    using System.IO;

    using netdomain.Abstract;

    /// <summary>
    /// Implements a query logger to log queries to an extension.
    /// </summary>
    public class QueryLogger : StringWriter
    {
        /// <summary>
        /// The registered extensions.
        /// </summary>
        private readonly List<IWorkspaceExtension> extensions;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryLogger"/> class.
        /// </summary>
        /// <param name="extensions">The extensions.</param>
        public QueryLogger(List<IWorkspaceExtension> extensions)
        {
            this.extensions = extensions;
        }

        /// <summary>
        /// Writes the specified region of a character array to this instance of the StringWriter.
        /// </summary>
        /// <param name="buffer">The character array to read data from.</param>
        /// <param name="index">The index at which to begin reading from <paramref name="buffer"/>.</param>
        /// <param name="count">The maximum number of characters to write.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer"/> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is negative. </exception>
        /// <exception cref="T:System.ArgumentException">(<paramref name="index"/> + <paramref name="count"/>)&gt; <paramref name="buffer"/>. Length. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The writer is closed. </exception>
        public override void Write(char[] buffer, int index, int count)
        {
            this.extensions.ForEach(ex => ex.OnPreQueryExecuted(new string(buffer)));
        }
    }
}