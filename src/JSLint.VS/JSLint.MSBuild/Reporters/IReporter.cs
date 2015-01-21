using JSLint.Framework.LinterBridge;

namespace JSLint.MSBuild.Reporters
{
    using System;
    
    /// <summary>
    /// Records JSLint violations and builds a report.
    /// </summary>
    public interface IReporter : IDisposable
    {
        /// <summary>
        /// Gets the processed file count.
        /// </summary>
        /// <value>
        /// The processed file count.
        /// </value>
        int ProcessedFileCount { get; }

        /// <summary>
        /// Gets the violating file count.
        /// </summary>
        /// <value>
        /// The violating file count.
        /// </value>
        int ViolatingFileCount { get; }

        /// <summary>
        /// Gets the violation count.
        /// </summary>
        /// <value>
        /// The violation count.
        /// </value>
        int ViolationCount { get; }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="file">The file.</param>
        void AddFile(string file);

        /// <summary>
        /// Adds the violation.
        /// </summary>
        /// <param name="violatingFile">The violating file.</param>
        /// <param name="violation">The violation.</param>
        void AddViolation(string violatingFile, JSLintError violation);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        string ToString();
    }
}
