namespace JSLint.MSBuild.Specs
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using JSLint.MSBuild.Specs.Helpers;
    using Xunit;

    public class JSLintTaskIntegration : IntegrationBase
    {
        private static readonly string MSBuildExecutable = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework\v4.0.30319\MSBuild.exe");

        private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\Resources"));

        private static readonly Regex ViolationCountPattern = new Regex(@"JSLINTVIOLATIONCOUNT=(?<Count>[\d]+)=JSLINTVIOLATIONCOUNT", RegexOptions.Compiled);

        private static readonly Regex ViolatingFileCountPattern = new Regex(@"JSLINTVIOLATINGFILECOUNT=(?<Count>[\d]+)=JSLINTVIOLATINGFILECOUNT", RegexOptions.Compiled);

        private static readonly Regex ProcessedFileCountPattern = new Regex(@"JSLINTPROCESSEDFILECOUNT=(?<Count>[\d]+)=JSLINTPROCESSEDFILECOUNT", RegexOptions.Compiled);

        [Fact]
        public void Should_fail_with_correct_counts_using_errors_project()
        {
            var actual = ExecuteMSBuildProject("ErrorsWithOptions");

            Assert.False(actual.Success);
            Assert.Equal(8, actual.ViolationCount);
            Assert.Equal(3, actual.ViolatingFileCount);
            Assert.Equal(4, actual.ProcessedFileCount);
        }

        [Fact]
        public void Should_succeed_with_correct_counts_using_warnings_project()
        {
            var actual = ExecuteMSBuildProject("WarningsWithOptions");

            Assert.True(actual.Success);
            Assert.Equal(8, actual.ViolationCount);
            Assert.Equal(3, actual.ViolatingFileCount);
            Assert.Equal(4, actual.ProcessedFileCount);
        }

        [Fact]
        public void Should_fail_when_source_files_property_omitted()
        {
            var actual = ExecuteMSBuildProject("NoSourceFiles");

            Assert.False(actual.Success);
            Assert.Contains("task was not given a value for the required parameter \"SourceFiles\"", actual.Output);
        }

        [Fact]
        public void Should_save_html_report_with_report_file_property()
        {
            var actual = ExecuteMSBuildProject("HtmlReport");

            var reportPath = Path.Combine(ProjectRoot, "JSLintReport.html");
            Assert.True(File.Exists(reportPath));
            File.Delete(reportPath);
        }

        private static int ParseCount(Regex pattern, string input)
        {
            var match = pattern.Match(input);

            if (match.Success && match.Groups["Count"].Success)
            {
                int count;

                if (int.TryParse(match.Groups["Count"].Value, out count))
                {
                    return count;
                }
            }

            return -1;
        }

        private static JSLintTaskResult ExecuteMSBuildProject(string projectName)
        {
            var result = ProcessHelper.Execute(MSBuildExecutable, projectName + ".proj", ProjectRoot);
            var exitCode = result.Item1;
            var output = result.Item2;

            return new JSLintTaskResult()
            {
                ExitCode = exitCode,
                Output = output,
                ViolationCount = ParseCount(ViolationCountPattern, output),
                ViolatingFileCount = ParseCount(ViolatingFileCountPattern, output),
                ProcessedFileCount = ParseCount(ProcessedFileCountPattern, output),
                Success = exitCode == 0 && output.Contains("Build succeeded.")
            };
        }

        private class JSLintTaskResult
        {
            public int ViolationCount { get; set; }

            public int ViolatingFileCount { get; set; }

            public int ProcessedFileCount { get; set; }

            public string Output { get; set; }

            public int ExitCode { get; set; }

            public bool Success { get; set; }
        }
    }
}
