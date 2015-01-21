using System.Collections.Generic;
using System.Linq;
using JSLint.Framework.LinterBridge;

namespace JSLint.MSBuild.Reporters
{
    /// <summary>
    /// Serves as the base class for reporting JSLint violations.
    /// </summary>
    public abstract class ReporterBase : IReporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReporterBase"/> class.
        /// </summary>
        public ReporterBase()
        {
            this.Files = new Dictionary<string, IList<JSLintError>>();
        }

        /// <summary>
        /// Gets the processed file count.
        /// </summary>
        /// <value>
        /// The processed file count.
        /// </value>
        public int ProcessedFileCount
        {
            get
            {
                return this.Files.Count;
            }
        }

        /// <summary>
        /// Gets the violating file count.
        /// </summary>
        /// <value>
        /// The violating file count.
        /// </value>
        public int ViolatingFileCount
        {
            get
            {
                return this.Files.Count(x => x.Value.Count > 0);
            }
        }

        /// <summary>
        /// Gets the violation count.
        /// </summary>
        /// <value>
        /// The violation count.
        /// </value>
        public int ViolationCount
        {
            get
            {
                return this.Files.Sum(x => x.Value.Count);
            }
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        protected IDictionary<string, IList<JSLintError>> Files { get; private set; }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void AddFile(string file)
        {
            if (!this.Files.ContainsKey(file))
            {
                this.Files.Add(file, new List<JSLintError>());
            }
        }

        /// <summary>
        /// Adds the violation.
        /// </summary>
        /// <param name="violatingFile">The violating file.</param>
        /// <param name="violation">The violation.</param>
        public void AddViolation(string violatingFile, JSLintError violation)
        {
            this.AddFile(violatingFile);
            this.Files[violatingFile].Add(violation);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public abstract override string ToString();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
	}
}
