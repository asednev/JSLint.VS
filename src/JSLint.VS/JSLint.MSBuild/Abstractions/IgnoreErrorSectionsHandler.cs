using JSLint.Framework.OptionClasses;

#pragma warning disable 1591

namespace JSLint.MSBuild.Abstractions
{
    public class IgnoreErrorSectionsHandler : Framework.LinterBridge.IgnoreErrorSectionsHandler, IIgnoreErrorSectionsHandler
    {
        public IgnoreErrorSectionsHandler(string contents, Options options = null)
            : base(contents, options)
        {
        }
    }
}
