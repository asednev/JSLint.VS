using JSLint.Framework.OptionClasses;

namespace JSLint.VS
{
    internal static class ErrorCategoryExtensions
    {
        internal static bool IsTaskError(this ErrorCategory cat)
        {
            return System.Enum.IsDefined(
                typeof(Microsoft.VisualStudio.Shell.TaskErrorCategory),
                (int)cat);
        }
    }
}

