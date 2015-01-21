#pragma warning disable 1591

namespace JSLint.MSBuild.Abstractions
{
    using System.Text;

    public interface IFileSystemWrapper
    {
        string ReadAllText(string path, Encoding encoding);

        bool FileExists(string path);

        void WriteAllText(string path, string contents, Encoding encoding);
    }
}
