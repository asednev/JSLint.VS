using JSLint.Framework.LinterBridge;
using JSLint.Framework.OptionClasses;
using JSLint.MSBuild.Abstractions;
using JSLint.MSBuild.Reporters;
using JSLint.MSBuild.Specs.Helpers;

namespace JSLint.MSBuild.Specs
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Moq;
    using Xunit;

    public class JSLintTaskUnit
    {
        public abstract class JSLintTaskUnitBase : UnitBase<JSLintTask>
        {
            public Mock<IFileSystemWrapper> FileSystemWrapperMock { get; set; }

            public Mock<ITaskLoggingHelperFactory> LoggingHelperFactoryMock { get; set; }

            public Mock<ITaskLoggingHelper> LoggingHelperMock { get; set; }

            public Mock<IJSLintFactory> JSLintFactoryMock { get; set; }

            public Mock<IReporterFactory> ReporterFactoryMock { get; set; }

            public override JSLintTask Construct()
            {
                this.FileSystemWrapperMock = this.AutoMocker.Mock<IFileSystemWrapper>();
                this.LoggingHelperFactoryMock = this.AutoMocker.Mock<ITaskLoggingHelperFactory>();
                this.JSLintFactoryMock = this.AutoMocker.Mock<IJSLintFactory>();
                this.ReporterFactoryMock = this.AutoMocker.Mock<IReporterFactory>();
                this.LoggingHelperMock = new Mock<ITaskLoggingHelper>();

                this.LoggingHelperFactoryMock.Setup(x => x.Create(It.IsAny<ITask>()))
                    .Returns(this.LoggingHelperMock.Object);

                return base.Construct();
            }
        }

        public class Execute : JSLintTaskUnitBase
        {
            public Execute()
            {
                this.Instance.SourceFiles = new ITaskItem[0];
                this.JSLinterMock = new Mock<IJSLinter>();
                this.OptionsProviderMock = new Mock<IOptionsProvider>();
                this.IgnoreErrorsSectionHandlerMock = new Mock<IIgnoreErrorSectionsHandler>();
                this.ReporterMock = new Mock<IReporter>();

                this.JSLintFactoryMock
                    .Setup(x => x.CreateLinter())
                    .Returns(this.JSLinterMock.Object);

                this.JSLintFactoryMock
                    .Setup(x => x.CreateOptionsProvider(It.IsAny<string>()))
                    .Returns(this.OptionsProviderMock.Object);

                this.JSLintFactoryMock
                    .Setup(x => x.CreateIgnoreErrorSectionsHandler(It.IsAny<string>(), It.IsAny<Options>()))
                    .Returns(this.IgnoreErrorsSectionHandlerMock.Object);

                this.ReporterMock
                    .Setup(x => x.ToString())
                    .Returns("REPORTRESULT");
            }

            public Mock<IJSLinter> JSLinterMock { get; set; }

            public Mock<IOptionsProvider> OptionsProviderMock { get; set; }

            public Mock<IIgnoreErrorSectionsHandler> IgnoreErrorsSectionHandlerMock { get; set; }

            public Mock<IReporter> ReporterMock { get; set; }

            public void SetupJSLintFile(string fileName, int violationCount)
            {
                var contents = fileName + " contents";
                var violations = new JSLintError[violationCount];

                for (int i = 0; i < violationCount; i++)
                {
                    var number = i + 1;
                    violations[i] = JSLintHelper.CreateJSLintError(number, number, fileName + " message " + number, fileName + " evidence " + number);
                }

                var list = new List<ITaskItem>(this.Instance.SourceFiles);
                list.Add(new TaskItem(fileName));
                this.Instance.SourceFiles = list.ToArray();

                this.FileSystemWrapperMock
                    .Setup(x => x.ReadAllText(fileName, It.IsAny<Encoding>()))
                    .Returns(contents);

                this.JSLinterMock
                    .Setup(x => x.Lint(contents, It.IsAny<JSLintOptions>(), true))
                    .Returns(new List<JSLintError>(violations));
            }

            [Fact]
            public void Should_return_true_when_no_files_found()
            {
                var actual = this.Instance.Execute();

                Assert.True(actual);
            }

            [Fact]
            public void Should_return_true_when_no_files_contain_violations()
            {
                this.SetupJSLintFile("file.js", 0);

                var actual = this.Instance.Execute();

                Assert.True(actual);
            }

            [Fact]
            public void Should_return_false_when_one_violation_found()
            {
                this.SetupJSLintFile("file.js", 1);

                var actual = this.Instance.Execute();

                Assert.False(actual);
            }

            [Fact]
            public void Should_return_false_when_many_violations_found()
            {
                this.SetupJSLintFile("file1.js", 5);
                this.SetupJSLintFile("file2.js", 0);
                this.SetupJSLintFile("file3.js", 3);

                var actual = this.Instance.Execute();

                Assert.False(actual);
            }

            [Fact]
            public void Should_return_true_when_violations_found_but_treating_violations_as_warnings()
            {
                this.SetupJSLintFile("file.js", 2);
                this.Instance.TreatViolationsAsWarnings = true;

                var actual = this.Instance.Execute();

                Assert.True(actual);
            }

            [Fact]
            public void Should_lint_the_content_of_each_file_provided()
            {
                this.SetupJSLintFile("file1.js", 0);
                this.SetupJSLintFile("jsfile2.js", 0);

                this.Instance.Execute();

                this.JSLinterMock.Verify(x => x.Lint("file1.js contents", It.IsAny<JSLintOptions>(), true));
                this.JSLinterMock.Verify(x => x.Lint("jsfile2.js contents", It.IsAny<JSLintOptions>(), true));
                this.JSLinterMock.Verify(x => x.Lint(It.IsAny<string>(), It.IsAny<JSLintOptions>(), It.IsAny<bool>()), Times.Exactly(2));
            }

            [Fact]
            public void Should_log_each_violation_as_an_error()
            {
                this.SetupJSLintFile("file1.js", 2);
                this.SetupJSLintFile("jsfile2.js", 1);

                this.Instance.Execute();

                this.LoggingHelperMock.Verify(x => x.LogError(null, null, null, "file1.js", 1, 1, 0, 0, "file1.js message 1"));
                this.LoggingHelperMock.Verify(x => x.LogError(null, null, null, "file1.js", 2, 2, 0, 0, "file1.js message 2"));
                this.LoggingHelperMock.Verify(x => x.LogError(null, null, null, "jsfile2.js", 1, 1, 0, 0, "jsfile2.js message 1"));
                this.LoggingHelperMock.Verify(x => x.LogWarning(null, null, null, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), 0, 0, It.IsAny<string>()), Times.Never());
            }

            [Fact]
            public void Should_log_each_violation_as_a_warning_when_treating_violations_as_warnings()
            {
                this.SetupJSLintFile("file1.js", 2);
                this.SetupJSLintFile("jsfile2.js", 1);
                this.Instance.TreatViolationsAsWarnings = true;

                this.Instance.Execute();

                this.LoggingHelperMock.Verify(x => x.LogWarning(null, null, null, "file1.js", 1, 1, 0, 0, "file1.js message 1"));
                this.LoggingHelperMock.Verify(x => x.LogWarning(null, null, null, "file1.js", 2, 2, 0, 0, "file1.js message 2"));
                this.LoggingHelperMock.Verify(x => x.LogWarning(null, null, null, "jsfile2.js", 1, 1, 0, 0, "jsfile2.js message 1"));
                this.LoggingHelperMock.Verify(x => x.LogError(null, null, null, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), 0, 0, It.IsAny<string>()), Times.Never());
            }

            [Fact]
            public void Should_load_default_options_when_no_file_defined()
            {
                this.Instance.Execute();

                this.OptionsProviderMock.Verify(x => x.GetOptions(), Times.Never());
            }

            [Fact]
            public void Should_load_options_from_file_if_defined()
            {
                this.SetupJSLintFile("file.js", 0);

                this.FileSystemWrapperMock
                    .Setup(x => x.FileExists("settings.xml"))
                    .Returns(true);

                this.OptionsProviderMock
                    .Setup(x => x.GetOptions())
                    .Returns(new Options());

                this.Instance.OptionsFile = "settings.xml";
                this.Instance.Execute();

                this.OptionsProviderMock.Verify(x => x.GetOptions());
            }

            [Fact]
            public void Should_not_load_options_from_file_if_no_source_files_exist()
            {
                this.FileSystemWrapperMock
                    .Setup(x => x.FileExists("settings.xml"))
                    .Returns(true);

                this.OptionsProviderMock
                    .Setup(x => x.GetOptions())
                    .Returns(new Options());

                this.Instance.OptionsFile = "settings.xml";
                this.Instance.Execute();

                this.OptionsProviderMock.Verify(x => x.GetOptions(), Times.Never());
            }

            [Fact]
            public void Should_throw_if_options_file_cannot_be_found()
            {
                this.SetupJSLintFile("file.js", 0);
                this.Instance.OptionsFile = "settings.xml";

                Assert.Throws<FileNotFoundException>(() => this.Instance.Execute());
            }

            [Fact]
            public void Should_set_violation_count_to_total_number_of_violations()
            {
                this.SetupJSLintFile("file1.js", 2);
                this.SetupJSLintFile("file2.js", 1);
                this.SetupJSLintFile("file3.js", 5);
                this.SetupJSLintFile("file4.js", 2);

                this.Instance.Execute();

                Assert.Equal(this.Instance.ViolationCount, 10);
            }

            [Fact]
            public void Should_set_violating_file_count_to_total_number_of_files_with_violations()
            {
                this.SetupJSLintFile("file1.js", 2);
                this.SetupJSLintFile("file2.js", 0);
                this.SetupJSLintFile("file3.js", 3);

                this.Instance.Execute();

                Assert.Equal(2, this.Instance.ViolatingFileCount);
            }

            [Fact]
            public void Should_set_process_file_count_to_total_count_of_files_processed()
            {
                this.SetupJSLintFile("file1.js", 3);
                this.SetupJSLintFile("file2.js", 0);
                this.SetupJSLintFile("file3.js", 0);
                this.SetupJSLintFile("file4.js", 1);

                this.Instance.Execute();

                Assert.Equal(4, this.Instance.ProcessedFileCount);
            }

            [Fact]
            public void Should_not_log_ignored_violations()
            {
                this.SetupJSLintFile("file1.js", 3);

                this.IgnoreErrorsSectionHandlerMock
                    .Setup(x => x.IsErrorIgnored(3, 3))
                    .Returns(true);

                this.Instance.Execute();

                this.LoggingHelperMock.Verify(x => x.LogError(null, null, null, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), 0, 0, It.IsAny<string>()), Times.Exactly(2));
            }

            [Fact]
            public void Should_not_count_ignored_violations()
            {
                this.SetupJSLintFile("file1.js", 3);

                this.IgnoreErrorsSectionHandlerMock
                    .Setup(x => x.IsErrorIgnored(3, 3))
                    .Returns(true);

                this.Instance.Execute();

                Assert.Equal(2, this.Instance.ViolationCount);
            }

            [Fact]
            public void Should_not_count_files_with_all_ignored_violations()
            {
                this.SetupJSLintFile("file1.js", 3);
                this.SetupJSLintFile("file2.js", 5);

                this.IgnoreErrorsSectionHandlerMock
                    .Setup(x => x.IsErrorIgnored(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns((int x, int y) =>
                    {
                        if (x > 3)
                        {
                            return false;
                        }

                        return true;
                    });

                this.Instance.Execute();

                Assert.Equal(1, this.Instance.ViolatingFileCount);
            }

            [Fact]
            public void Should_not_try_to_create_a_reporter_if_no_source_files_exist()
            {
                this.Instance.Execute();

                var reporter = It.IsAny<IReporter>();
                this.ReporterFactoryMock.Verify(x => x.TryCreate(It.IsAny<string>(), out reporter), Times.Never());
            }

            [Fact]
            public void Should_try_to_create_a_reporter()
            {
                this.SetupJSLintFile("file.js", 0);

                this.Instance.Execute();

                var reporter = It.IsAny<IReporter>();
                this.ReporterFactoryMock.Verify(x => x.TryCreate(It.IsAny<string>(), out reporter));
            }

            [Fact]
            public void Should_add_all_violations_to_reporter_if_it_exists()
            {
                this.SetupJSLintFile("file1.js", 3);
                this.SetupJSLintFile("file2.js", 5);

                var reporter = this.ReporterMock.Object;
                this.ReporterFactoryMock
                    .Setup(x => x.TryCreate(It.IsAny<string>(), out reporter))
                    .Returns(true);

                this.Instance.Execute();

                this.ReporterMock.Verify(x => x.AddViolation("file1.js", It.IsAny<JSLintError>()), Times.Exactly(3));
                this.ReporterMock.Verify(x => x.AddViolation("file2.js", It.IsAny<JSLintError>()), Times.Exactly(5));
            }

            [Fact]
            public void Should_save_report_as_UTF8_if_it_exists()
            {
                this.SetupJSLintFile("file.js", 0);

                var reporter = this.ReporterMock.Object;
                this.ReporterFactoryMock
                    .Setup(x => x.TryCreate(It.IsAny<string>(), out reporter))
                    .Returns(true);

                this.Instance.ReportFile = "REPORTFILE";
                this.Instance.Execute();

                this.ReporterMock.Verify(x => x.ToString());
                this.FileSystemWrapperMock.Verify(x => x.WriteAllText("REPORTFILE", "REPORTRESULT", Encoding.UTF8));
            }
        }
    }
}
