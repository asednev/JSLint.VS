namespace JSLint.MSBuild.Reporters
{
    using System.Security;
    using JSLint.MSBuild.Properties;

    /// <summary>
    /// Records JSLint violations and builds a report in HTML format.
    /// </summary>
    public class HtmlReporter : FormatReporterBase
    {
        /// <summary>
        /// Gets the report format.
        /// </summary>
        /// <value>
        /// The report format.
        /// </value>
        protected override string ReportFormat
        {
            get
            {
                return Resources.HtmlReportFormat;
            }
        }

        /// <summary>
        /// Gets the file format.
        /// </summary>
        /// <value>
        /// The file format.
        /// </value>
        protected override string FileFormat
        {
            get
            {
                return Resources.HtmlFileFormat;
            }
        }

        /// <summary>
        /// Gets the violation format.
        /// </summary>
        /// <value>
        /// The violation format.
        /// </value>
        protected override string ViolationFormat
        {
            get
            {
                return Resources.HtmlViolationFormat;
            }
        }

        /// <summary>
        /// Escapes the text.
        /// </summary>
        /// <param name="text">The text to escape.</param>
        /// <returns>
        /// Escaped text.
        /// </returns>
        protected override string EscapeText(string text)
        {
            return SecurityElement.Escape(text);
        }
    }
}
