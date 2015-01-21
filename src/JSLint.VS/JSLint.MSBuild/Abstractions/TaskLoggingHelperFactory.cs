#pragma warning disable 1591

namespace JSLint.MSBuild.Abstractions
{
    using Microsoft.Build.Framework;

    public class TaskLoggingHelperFactory : ITaskLoggingHelperFactory
    {
        public ITaskLoggingHelper Create(ITask task)
        {
            return new TaskLoggingHelper(task);
        }

        public ITaskLoggingHelper Create(IBuildEngine buildEngine, string taskName)
        {
            return new TaskLoggingHelper(buildEngine, taskName);
        }
    }
}
