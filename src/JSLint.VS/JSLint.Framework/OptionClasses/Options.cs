using System.Xml.Serialization;

namespace JSLint.Framework.OptionClasses
{
	[XmlRoot]
	public sealed class Options
	{
		public Options()
		{
			Enabled = true;
			JSLintOptions = JSLintOptions.Default;
			ErrorCategory = ErrorCategory.Warning;
			TODOEnabled = false;
			TODOCategory = ErrorCategory.Task;
			RunOnBuild = true;
			CancelBuildOnError = true;
			FakeCSSCharset = false;
			BuildFileTypes = IncludeFileType.JS;
			IgnoreErrorStart = "/*ignore jslint start*/";
			IgnoreErrorEnd = "/*ignore jslint end*/";
			IgnoreErrorLine = "//ignore jslint";
			RunOnSave = false;
			SaveFileTypes = IncludeFileType.JS;
		}

		public bool Enabled { get; set; }

		public ErrorCategory ErrorCategory { get; set; }

		public bool TODOEnabled {
			get { return JSLintOptions.FindTodos; }
			set { JSLintOptions.FindTodos = value; }
		}

		public ErrorCategory TODOCategory { get; set; }

		public bool RunOnBuild { get; set; }

		public bool CancelBuildOnError { get; set; }

		public JSLintOptions JSLintOptions { get; set; }

		public IncludeFileType BuildFileTypes { get; set; }

		public IncludeFileType SaveFileTypes { get; set; }

		public bool FakeCSSCharset { get; set; }

		public string IgnoreErrorStart { get; set; }

		public string IgnoreErrorEnd { get; set; }

		public string IgnoreErrorLine { get; set; }

		public bool RunOnSave { get; set; }
	}
}

