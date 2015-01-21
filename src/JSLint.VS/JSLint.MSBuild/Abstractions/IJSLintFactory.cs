using JSLint.Framework.OptionClasses;

#pragma warning disable 1591

namespace JSLint.MSBuild.Abstractions
{
    public interface IJSLintFactory
    {
        IIgnoreErrorSectionsHandler CreateIgnoreErrorSectionsHandler(string contents, Options options);

        IJSLinter CreateLinter();

        IOptionsProvider CreateOptionsProvider(string filePath);
    }
}
