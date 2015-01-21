using System;
using System.Collections.Generic;
using JSLint.Framework.LinterBridge;
using JSLint.Framework.OptionClasses;
using JSLint.Framework.OptionClasses.OptionProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JSLint.VS.Test
{
	[TestClass]
	public class JsLintTestsBase : TestsBase
	{
		private static readonly String ResourceName = "JSLint.VS.Test.SettingFiles.JsLint.xml";

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
		public void MissingSemiColon1()
		{
			TestLint(
				"var a\nvar b;",
				new List<string>() { "Expected ';' and instead saw 'var'." });
		}

		[TestMethod]
		public void MissingSemiColon2()
		{
			TestLint(
				"var a;\nvar b",
				new List<string>() { "Expected ';' and instead saw '(end)'." });
		}

		[TestMethod]
		public void MoveVarOutOfFor()
		{
			TestLint(
				@"for (var i = 0; i < 4; i++) { i = 1; }",
				new List<string>() { "Move 'var' declarations to the top of the function.",
									 "Stopping. (100% scanned)."});
		}

		[TestMethod]
		public void Unparam()
		{
			TestLint(
				@"
function myfunc2(c) {
    if(c === 0) { return; }
    myfunc2(c);
}
function myfunc(b) {
    var c = 2;
    c += 1;
    myfunc2(c);
}
",
				new List<string>() { "Unused 'b'." },
				lintoptions: new JSLintOptions() { BoolOptions2 = GetOptions(truthy: "white.", falsy: "unparam"), ErrorOnUnused = true });
		}

		[TestMethod]
		public void StrangeEquals()
		{
			TestLint(
				@"var a;
if (a === a) {
    a = true;
}",
				new List<string>() { "Weird relation." });
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
		public void Whitespace2()
		{
			TestLint(
				@"
var i;
for(i = 0; i < 4; i++) { i = 1; }",
				new List<string>() { "Missing space between 'for' and '('." },
				new JSLintOptions() { BoolOptions2 = GetOptions(falsy: "white white.", truthy: "plusplus plusplus.") });
		}

		[TestMethod]
		public void InPrefix()
		{
			TestLint(
				@"
var a;
if ('e' in a) { a = true; }",
				new List<string>() { "Unexpected 'in'. Compare with undefined, or use the hasOwnProperty method instead." });
		}

	}
}
