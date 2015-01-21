namespace JSLint.MSBuild.Specs.Reporters
{
    using System.Text.RegularExpressions;
    using JSLint.MSBuild.Reporters;
    using JSLint.MSBuild.Specs.Helpers;
    using Xunit;

    public class HtmlReporterUnit : UnitBase<HtmlReporter>
    {
        [Fact]
        public void Should_add_total_counts_to_HTML_output()
        {
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(23, 44, "Something wrong", null));
            this.Instance.AddFile("uberfile2.js");
            this.Instance.AddFile("uberfile3.js");

            var actual = this.Instance.ToString();

            Expect.Matches(@"processed files[^\d]+3", RegexOptions.IgnoreCase, actual);
            Expect.Matches(@"violating files[^\d]+1", RegexOptions.IgnoreCase, actual);
            Expect.Matches(@"total violations[^\d]+1", RegexOptions.IgnoreCase, actual);
        }

        [Fact]
        public void Should_add_files_with_count_to_HTML_output()
        {
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(23, 44, "Something wrong", "evidence 1"));
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(33, 54, "Stupidity detected", "evidence 2"));
            this.Instance.AddViolation("uberfile2.js", JSLintHelper.CreateJSLintError(33, 54, "Stupidity detected", "evidence 3"));

            var actual = this.Instance.ToString();

            Assert.Contains("uberfile.js (2 violations)", actual);
            Assert.Contains("uberfile2.js (1 violations)", actual);
        }

        [Fact]
        public void Should_not_add_files_without_violations_to_HTML_output()
        {
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(23, 44, "Something wrong", "evidence 1"));
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(33, 54, "Stupidity detected", "evidence 2"));
            this.Instance.AddFile("uberfile2.js");

            var actual = this.Instance.ToString();

            Assert.DoesNotContain("uberfile2.js", actual);
        }

        [Fact]
        public void Should_add_violations_to_HTML_output()
        {
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(23, 44, "Something wrong", "evidence 1"));
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(33, 54, "Stupidity detected", "evidence 2"));

            var actual = this.Instance.ToString();

            Assert.Contains("Something wrong", actual);
            Assert.Contains("line 23 character 44", actual);
            Assert.Contains("Stupidity detected", actual);
            Assert.Contains("line 33 character 54", actual);
        }
    }
}
