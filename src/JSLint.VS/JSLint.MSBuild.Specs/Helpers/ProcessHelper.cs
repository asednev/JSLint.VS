namespace JSLint.MSBuild.Specs.Helpers
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public static class ProcessHelper
    {
        public static Tuple<int, string> Execute(string exePath, string arguments)
        {
            return Execute(exePath, arguments, null);
        }

        public static Tuple<int, string> Execute(string exePath, string arguments, string workingDirectory)
        {
            string standardOutput;
            int exitCode;

            using (var process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = exePath;
                process.StartInfo.WorkingDirectory = string.IsNullOrEmpty(workingDirectory) ? Path.GetDirectoryName(exePath) : workingDirectory;
                process.StartInfo.Arguments = arguments;
                process.Start();

                standardOutput = process.StandardOutput.ReadToEnd();

                process.WaitForExit();

                exitCode = process.ExitCode;
            }

            return new Tuple<int, string>(exitCode, standardOutput);
        }
    }
}
