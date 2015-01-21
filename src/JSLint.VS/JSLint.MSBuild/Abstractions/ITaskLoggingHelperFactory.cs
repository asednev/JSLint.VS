#pragma warning disable 1591

namespace JSLint.MSBuild.Abstractions
{
    using Microsoft.Build.Framework;

    public interface ITaskLoggingHelperFactory
    {
        ITaskLoggingHelper Create(ITask task);
    }
}
