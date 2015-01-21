namespace JSLint.MSBuild.Reporters
{
    /// <summary>
    /// Constructs <see cref="IReporter" /> instances.
    /// </summary>
    public interface IReporterFactory
    {
        /// <summary>
        /// Creates an <see cref="IReporter" /> instance based on the extension of the report file.
        /// </summary>
        /// <param name="reportFile">The report file.</param>
        /// <returns>
        /// A new <see cref="IReporter" /> instance.
        /// </returns>
        IReporter Create(string reportFile);

        /// <summary>
        /// Tries to create create an <see cref="IReporter" /> instance based on the extension of the report file.
        /// </summary>
        /// <param name="reportFile">The report file.</param>
        /// <param name="reporter">The reporter.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="IReporter" /> instance could be created; otherwise <c>false</c>.
        /// </returns>
        bool TryCreate(string reportFile, out IReporter reporter);

        /// <summary>
        /// Creates an <see cref="IReporter" /> instance for reporting in HTML format.
        /// </summary>
        /// <returns>
        /// A new <see cref="IReporter" /> instance.
        /// </returns>
        IReporter CreateHtml();

        /// <summary>
        /// Creates an <see cref="IReporter" /> instance for reporting in text format.
        /// </summary>
        /// <returns>
        /// A new <see cref="IReporter" /> instance.
        /// </returns>
        IReporter CreateText();
    }
}
