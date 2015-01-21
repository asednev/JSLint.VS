using System.IO;
using System.Text;
using JSLint.Framework.OptionClasses;
using JSLint.MSBuild.Abstractions;
using JSLint.MSBuild.Reporters;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace JSLint.MSBuild
{
    /// <summary>
    /// Task that will run JSLint over JavaScript files.
    /// </summary>
    public class JSLintTask : Task
    {
        private IJSLintFactory jsLintFactory;

        private IFileSystemWrapper fileSystemWrapper;

        private IReporterFactory reporterFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintTask"/> class with default services.
        /// </summary>
        public JSLintTask()
            : this(new JSLintFactory(), new FileSystemWrapper(), new TaskLoggingHelperFactory(), new ReporterFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintTask" /> class with custom services.
        /// </summary>
        /// <param name="jsLintFactory">The JSLint factory.</param>
        /// <param name="fileSystemWrapper">The file system wrapper.</param>
        /// <param name="taskLoggingHelperFactory">The task logging helper factory.</param>
        /// <param name="reporterFactory">The reporter factory.</param>
        public JSLintTask(IJSLintFactory jsLintFactory, IFileSystemWrapper fileSystemWrapper, ITaskLoggingHelperFactory taskLoggingHelperFactory, IReporterFactory reporterFactory)
        {
            this.jsLintFactory = jsLintFactory;
            this.fileSystemWrapper = fileSystemWrapper;
            this.reporterFactory = reporterFactory;

            this.LoggingHelper = taskLoggingHelperFactory.Create(this);
        }

        private delegate void ViolationLogger(string subcategory, string errorCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs);

        /// <summary>
        /// Gets or sets the source files to be processed by JSLint.
        /// </summary>
        /// <value>
        /// The source files.
        /// </value>
        [Required]
        public ITaskItem[] SourceFiles { get; set; }

        /// <summary>
        /// Gets or sets the path to JSLint options file.
        /// </summary>
        /// <value>
        /// The options file.
        /// </value>
        public string OptionsFile { get; set; }

        /// <summary>
        /// Gets or sets the report file.
        /// </summary>
        /// <value>
        /// The report file.
        /// </value>
        public string ReportFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether JSLint violations should be treated as warnings rather than errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if violations should be treated as warnings; otherwise, <c>false</c>.
        /// </value>
        public bool TreatViolationsAsWarnings { get; set; }

        /// <summary>
        /// Gets the violation count.
        /// </summary>
        /// <value>
        /// The violation count.
        /// </value>
        [Output]
        public int ViolationCount { get; private set; }

        /// <summary>
        /// Gets the violating file count.
        /// </summary>
        /// <value>
        /// The violating file count.
        /// </value>
        [Output]
        public int ViolatingFileCount { get; private set; }

        /// <summary>
        /// Gets the processed file count.
        /// </summary>
        /// <value>
        /// The processed file count.
        /// </value>
        [Output]
        public int ProcessedFileCount { get; private set; }

        /// <summary>
        /// Gets the logging helper.
        /// </summary>
        /// <value>
        /// The logging helper.
        /// </value>
        protected ITaskLoggingHelper LoggingHelper { get; private set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the task successfully executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool Execute()
        {
            var violatingFileCount = 0;
            var violationCount = 0;
            var processedFileCount = 0;

            if (this.SourceFiles.Length > 0)
            {
                var logger = this.GetLogger();
                var options = this.GetOptions();
                IReporter reporter;
                var report = this.reporterFactory.TryCreate(this.ReportFile, out reporter);

                using (var linter = this.jsLintFactory.CreateLinter())
                {
                    foreach (var item in this.SourceFiles)
                    {
                        var file = item.ItemSpec;
                        var text = this.fileSystemWrapper.ReadAllText(file, Encoding.UTF8);
                        var violations = linter.Lint(text, options.JSLintOptions, true);

                        if (report)
                        {
                            reporter.AddFile(file);
                        }

                        if (violations.Count > 0)
                        {
                            var ignoreParser = this.jsLintFactory.CreateIgnoreErrorSectionsHandler(text, options);
                            var fileViolationCount = 0;

                            foreach (var violation in violations)
                            {
                                if (!ignoreParser.IsErrorIgnored(violation.Line, violation.Column))
                                {
                                    logger(
                                        subcategory: null,
                                        errorCode: null,
                                        helpKeyword: null,
                                        file: file,
                                        lineNumber: violation.Line,
                                        columnNumber: violation.Column,
                                        endLineNumber: 0,
                                        endColumnNumber: 0,
                                        message: violation.Message);

                                    if (report)
                                    {
                                        reporter.AddViolation(file, violation);
                                    }

                                    fileViolationCount += 1;
                                }
                            }

                            if (fileViolationCount > 0)
                            {
                                violatingFileCount += 1;
                                violationCount += fileViolationCount;
                            }
                        }

                        processedFileCount += 1;
                    }
                }

                if (report)
                {
                    var result = reporter.ToString();
                    this.fileSystemWrapper.WriteAllText(this.ReportFile, result, Encoding.UTF8);
                    reporter.Dispose();
                }
            }

            this.ViolationCount = violationCount;
            this.ViolatingFileCount = violatingFileCount;
            this.ProcessedFileCount = processedFileCount;

            return this.TreatViolationsAsWarnings || violationCount == 0;
        }

        private ViolationLogger GetLogger()
        {
            if (this.TreatViolationsAsWarnings)
            {
                return this.LoggingHelper.LogWarning;
            }

            return this.LoggingHelper.LogError;
        }

        private Options GetOptions()
        {
            if (string.IsNullOrEmpty(this.OptionsFile))
            {
                return new Options();
            }

            if (!this.fileSystemWrapper.FileExists(this.OptionsFile))
            {
                throw new FileNotFoundException("The options file could not be found.", this.OptionsFile);
            }

            var provider = this.jsLintFactory.CreateOptionsProvider(this.OptionsFile);

            return provider.GetOptions();
        }
    }
}
