using System;
using System.Collections.Generic;
using JSLint.Framework.LinterBridge;
using JSLint.Framework.OptionClasses;
using JSLint.Framework.OptionClasses.OptionProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JSLint.VS.Test
{
	[TestClass]
	public class JsHintTestBase : TestsBase
	{
		private static readonly String ResourceName = "JSLint.VS.Test.SettingFiles.JsHint.xml";

		[TestInitialize]
		public void Setup()
		{
			OptionsProviderRegistry.PushOptionsProvider(new EmbeddedResourceOptionsProvider("Global", typeof(JsHintTestBase).Assembly, ResourceName));

			OptionsProviderRegistry.ReloadCurrent();

			_linter = new JSLinter();
		}

		[TestCleanup]
		public void TearDown()
		{
			_linter.Dispose();
		}

		[TestMethod]
		public void Whitespace1()
		{
			TestLint(
				@"
var i;
for(i = 0; i < 4; i += 1) { i = 1; }",
				new List<string>() { },
				new JSLintOptions() { BoolOptions2 = GetOptions(truthy: "white white.") });
		}

		[TestMethod]
		public void Whitespace_Defaults()
		{
			TestLint(
				@"
var i;
for(i = 0; i < 4; i += 1) { i = 1; }",
				new List<string>() { });
		}

		[TestMethod]
		public void MissingSemiColon1()
		{
			TestLint(
				"var a\nvar b; a=b;",
				new List<string>() { "Missing semicolon." });
		}

		[TestMethod]
		public void MissingSemiColon2()
		{
			TestLint(
				"var a;\nvar b " +
				"a = b;",
				new List<string>() { "Missing semicolon." });
		}

	}
}
