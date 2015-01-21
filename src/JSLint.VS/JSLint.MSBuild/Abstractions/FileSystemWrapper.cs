#pragma warning disable 1591

namespace JSLint.MSBuild.Abstractions
{
    using System.IO;
    using System.Text;

    public class FileSystemWrapper : IFileSystemWrapper
    {
        public string ReadAllText(string path, Encoding encoding)
        {
            return File.ReadAllText(path, encoding);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            File.WriteAllText(path, contents, encoding);
        }
    }
}
