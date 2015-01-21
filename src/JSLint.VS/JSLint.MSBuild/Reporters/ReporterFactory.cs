namespace JSLint.MSBuild.Reporters
{
    using System.IO;

    /// <summary>
    /// Constructs <see cref="IReporter" /> instances.
    /// </summary>
    public class ReporterFactory : IReporterFactory
    {
        /// <summary>
        /// Creates an <see cref="IReporter" /> instance based on the extension of the report file.
        /// </summary>
        /// <param name="reportFile">The report file.</param>
        /// <returns>
        /// A new <see cref="IReporter" /> instance.
        /// </returns>
        public IReporter Create(string reportFile)
        {
            if (!string.IsNullOrEmpty(reportFile))
            {
                switch (Path.GetExtension(reportFile).ToLower())
                {
                    case ".htm":
                    case ".html":
                        return this.CreateHtml();
                    default:
                        return this.CreateText();
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to create create an <see cref="IReporter" /> instance based on the extension of the report file.
        /// </summary>
        /// <param name="reportFile">The report file.</param>
        /// <param name="reporter">The reporter.</param>
        /// <returns>
        ///   <c>true</c> if the <see cref="IReporter" /> instance could be created; otherwise <c>false</c>.
        /// </returns>
        public bool TryCreate(string reportFile, out IReporter reporter)
        {
            reporter = this.Create(reportFile);

            return reporter != null;
        }

        /// <summary>
        /// Creates an <see cref="IReporter" /> instance for reporting in HTML format.
        /// </summary>
        /// <returns>
        /// A new <see cref="IReporter" /> instance.
        /// </returns>
        public IReporter CreateHtml()
        {
            return new HtmlReporter();
        }

        /// <summary>
        /// Creates an <see cref="IReporter" /> instance for reporting in text format.
        /// </summary>
        /// <returns>
        /// A new <see cref="IReporter" /> instance.
        /// </returns>
        public IReporter CreateText()
        {
            return new TextReporter();
        }
    }
}
