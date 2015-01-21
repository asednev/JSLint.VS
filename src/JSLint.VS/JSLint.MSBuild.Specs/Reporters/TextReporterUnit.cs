namespace JSLint.MSBuild.Specs.Reporters
{
    using JSLint.MSBuild.Reporters;
    using JSLint.MSBuild.Specs.Helpers;
    using Xunit;

    public class TextReporterUnit : UnitBase<TextReporter>
    {
        [Fact]
        public void Should_add_total_counts_to_text_output()
        {
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(23, 44, "Something wrong", null));
            this.Instance.AddFile("uberfile2.js");
            this.Instance.AddFile("uberfile3.js");

            var actual = this.Instance.ToString();

            Assert.Contains("3 processed files", actual);
            Assert.Contains("1 violating files", actual);
            Assert.Contains("1 total violations", actual);
        }

        [Fact]
        public void Should_add_files_with_count_to_text_output()
        {
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(23, 44, "Something wrong", "evidence 1"));
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(33, 54, "Stupidity detected", "evidence 2"));
            this.Instance.AddViolation("uberfile2.js", JSLintHelper.CreateJSLintError(33, 54, "Stupidity detected", "evidence 3"));

            var actual = this.Instance.ToString();

            Assert.Contains("uberfile.js (2 violations)", actual);
            Assert.Contains("uberfile2.js (1 violations)", actual);
        }

        [Fact]
        public void Should_not_add_files_without_violations_to_text_output()
        {
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(23, 44, "Something wrong", "evidence 1"));
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(33, 54, "Stupidity detected", "evidence 2"));
            this.Instance.AddFile("uberfile2.js");

            var actual = this.Instance.ToString();

            Assert.DoesNotContain("uberfile2.js", actual);
        }

        [Fact]
        public void Should_add_violations_to_text_output()
        {
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(23, 44, "Something wrong", "evidence 1"));
            this.Instance.AddViolation("uberfile.js", JSLintHelper.CreateJSLintError(33, 54, "Stupidity detected", "evidence 2"));

            var actual = this.Instance.ToString();

            Assert.Contains("Something wrong (line 23 character 44)", actual);
            Assert.Contains("Stupidity detected (line 33 character 54)", actual);
        }
    }
}
