namespace JSLint.MSBuild.Reporters
{
    using JSLint.MSBuild.Properties;

    /// <summary>
    /// Records JSLint violations and builds a report in text format.
    /// </summary>
    public class TextReporter : FormatReporterBase
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
                return Resources.TextReportFormat;
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
                return Resources.TextFileFormat;
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
                return Resources.TextViolationFormat;
            }
        }
    }
}
