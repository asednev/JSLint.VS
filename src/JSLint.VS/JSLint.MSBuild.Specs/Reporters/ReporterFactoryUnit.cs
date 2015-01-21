namespace JSLint.MSBuild.Specs.Reporters
{
    using JSLint.MSBuild.Reporters;
    using Xunit;

    public class ReporterFactoryUnit
    {
        public abstract class ReporterFactoryUnitBase : UnitBase<ReporterFactory>
        {
        }

        public class Create : ReporterFactoryUnitBase
        {
            [Fact]
            public void Should_return_a_text_reporter_given_an_unknown_file()
            {
                var actual = this.Instance.Create("something.unsupported");

                Assert.NotNull(actual);
                Assert.IsType<TextReporter>(actual);
            }

            [Fact]
            public void Should_return_an_html_reporter_given_an_htm_file()
            {
                var actual = this.Instance.Create("path.htm");

                Assert.NotNull(actual);
                Assert.IsType<HtmlReporter>(actual);
            }

            [Fact]
            public void Should_return_an_html_reporter_given_an_html_file()
            {
                var actual = this.Instance.Create("path.html");

                Assert.NotNull(actual);
                Assert.IsType<HtmlReporter>(actual);
            }

            [Fact]
            public void Should_return_a_text_reporter_given_a_txt_file()
            {
                var actual = this.Instance.Create("path.txt");

                Assert.NotNull(actual);
                Assert.IsType<TextReporter>(actual);
            }
        }

        public class TryCreate : ReporterFactoryUnitBase
        {
            [Fact]
            public void Should_return_true_given_an_empty_file_path()
            {
                IReporter reporter;
                var actual = this.Instance.TryCreate(string.Empty, out reporter);

                Assert.False(actual);
            }

            [Fact]
            public void Should_return_true_given_an_unknown_file_type()
            {
                IReporter reporter;
                var actual = this.Instance.TryCreate("path.unsupported", out reporter);

                Assert.True(actual);
            }

            [Fact]
            public void Should_return_true_given_a_text_file_path()
            {
                IReporter reporter;
                var actual = this.Instance.TryCreate("path.txt", out reporter);

                Assert.True(actual);
            }

            [Fact]
            public void Should_return_true_given_a_html_file_path()
            {
                IReporter reporter;
                var actual = this.Instance.TryCreate("path.html", out reporter);

                Assert.True(actual);
            }
        }

        public class CreateHtml : ReporterFactoryUnitBase
        {
            [Fact]
            public void Should_return_an_HTML_reporter()
            {
                var actual = this.Instance.CreateHtml();

                Assert.NotNull(actual);
                Assert.IsType<HtmlReporter>(actual);
            }
        }

        public class CreateText : ReporterFactoryUnitBase
        {
            [Fact]
            public void Should_return_a_text_reporter()
            {
                var actual = this.Instance.CreateText();

                Assert.NotNull(actual);
                Assert.IsType<TextReporter>(actual);
            }
        }
    }
}
