using JSLint.Framework.OptionClasses;
using JSLint.Framework.OptionClasses.OptionProviders;

#pragma warning disable 1591

namespace JSLint.MSBuild.Abstractions
{
    public class JSLintFactory : IJSLintFactory
    {
        public IIgnoreErrorSectionsHandler CreateIgnoreErrorSectionsHandler(string contents, Options options)
        {
            return new IgnoreErrorSectionsHandler(contents, options);
        }

        public IOptionsProvider CreateOptionsProvider(string filePath)
        {
            return new FileOptionsProvider("FileOptionsProvider", filePath);
        }

        public IJSLinter CreateLinter()
        {
            return new JSLinter();
        }
    }
}
