using System;
using System.Collections.Generic;
using JSLint.Framework.LinterBridge;
using JSLint.Framework.OptionClasses;

#pragma warning disable 1591

namespace JSLint.MSBuild.Abstractions
{
    public interface IJSLinter : IDisposable
    {
        List<JSLintError> Lint(string javascript, bool isJavaScript);

        List<JSLintError> Lint(string javascript, JSLintOptions configuration, bool isJavaScript);
    }
}
