#pragma warning disable 1591

namespace JSLint.MSBuild.Abstractions
{
    using Microsoft.Build.Framework;

    public class TaskLoggingHelper : Microsoft.Build.Utilities.TaskLoggingHelper, ITaskLoggingHelper
    {
        public TaskLoggingHelper(ITask task)
            : base(task)
        {
        }

        public TaskLoggingHelper(IBuildEngine buildEngine, string taskName)
            : base(buildEngine, taskName)
        {
        }
    }
}
