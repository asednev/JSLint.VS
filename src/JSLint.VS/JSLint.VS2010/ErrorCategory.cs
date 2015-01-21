using System.Xml.Serialization;

namespace JSLint.VS2010.VS2010
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

