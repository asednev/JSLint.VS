namespace JSLint.MSBuild.Reporters
{
    using System;
    using System.Text;

    /// <summary>
    /// Serves as the base class for reporting JSLint violations using string formatting.
    /// </summary>
    public abstract class FormatReporterBase : ReporterBase
    {
        /// <summary>
        /// Gets the report format.
        /// </summary>
        /// <value>
        /// The report format.
        /// </value>
        protected abstract string ReportFormat { get; }

        /// <summary>
        /// Gets the file format.
        /// </summary>
        /// <value>
        /// The file format.
        /// </value>
        protected abstract string FileFormat { get; }

        /// <summary>
        /// Gets the violation format.
        /// </summary>
        /// <value>
        /// The violation format.
        /// </value>
        protected abstract string ViolationFormat { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var fileBuilder = new StringBuilder();

            foreach (var file in this.Files.Keys)
            {
                var violations = this.Files[file];

                if (violations.Count > 0)
                {
                    var violationBuilder = new StringBuilder();

                    foreach (var violation in violations)
                    {
                        violationBuilder.AppendFormat(
                            this.ViolationFormat,
                            violation.Line,
                            violation.Column,
                            this.EscapeText(violation.Message),
                            this.EscapeText(violation.Evidence));
                    }

                    fileBuilder.AppendFormat(
                        this.FileFormat,
                        this.EscapeText(file),
                        violations.Count,
                        violationBuilder.ToString());
                }
            }

            return string.Format(
                this.ReportFormat,
                "JSLint Report",
                DateTime.Now.ToString("s"),
                this.ProcessedFileCount,
                this.ViolatingFileCount,
                this.ViolationCount,
                fileBuilder.ToString());
        }

        /// <summary>
        /// Escapes the text.
        /// </summary>
        /// <param name="text">The text to escape.</param>
        /// <returns>
        /// Escaped text.
        /// </returns>
        protected virtual string EscapeText(string text)
        {
            return text;
        }
    }
}
