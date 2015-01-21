#pragma warning disable 1591

namespace JSLint.MSBuild.Abstractions
{
    using System.Collections.Generic;

    public interface IIgnoreErrorSectionsHandler
    {
        List<IgnoreErrorSectionsHandler.IgnoreErrorSection> SectionsToIgnore { get; set; }

        bool IsErrorIgnored(int line, int col);
    }
}
