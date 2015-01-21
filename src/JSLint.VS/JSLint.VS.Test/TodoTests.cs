using System;
using System.Collections.Generic;
using JSLint.Framework.LinterBridge;
using JSLint.Framework.OptionClasses;
using JSLint.Framework.OptionClasses.OptionProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JSLint.VS.Test
{
	[TestClass]
	public class TodoTestsBase  : TestsBase
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
		[Ignore]
		public void TodoDetection1()
		{
			TestLint(
				@"//TODO: first
var i;//TODO second
for (/*TODO third*//*TODO fourth*/i = 0; i < 4; i++) { i = 1; }//           todo five
/* a
b
c
d
 TODO sixth*/
/*
Todo seventh*/
/*TODO*/
//todo",
				new List<string>() { "TODO: first", "TODO second", "TODO third", "TODO fourth", "todo five", "TODO sixth", "Todo seventh", "TODO", "todo" },
				lintoptions: new JSLintOptions() { BoolOptions2 = GetOptions(truthy: "white. plusplus."), FindTodos = true });
		}

		[TestMethod]
		[Ignore]
		public void TodoDetection2()
		{
			TestLint(
				@"/*TODO 1*/function/*TODO 2*/ anon/*TODO 3*/(/*TODO 4*/)/*TODO 5*/ {/*TODO 6*/ var/*TODO 7*/ a/*TODO 8*/ = /*TODO 9*/1,/*TODO 10*/ b/*TODO 11*/ = /*TODO 12*/function/*TODO 13*/(/*TODO 14*/)/*TODO 15*/ {/*TODO 16*/ }/*TODO 17*/;/*TODO 18*/ }/*TODO 19*/",
				getTodos(19),
				lintoptions: new JSLintOptions() { BoolOptions2 = GetOptions(truthy: "white."), FindTodos = true });
		}

		private List<string> getTodos(int top)
		{
			List<string> todos = new List<string>();
			int i = 1;
			while (i <= top)
			{
				todos.Add(string.Format("TODO {0}", i++));
			}
			return todos;
		}

		[TestMethod]
		[Ignore]
		public void TodoDetection3()
		{
			TestLint(
				@"/*TODO 1*/function/*TODO 2*/ c/*TODO 3*/(/*TODO 4*/a/*TODO 5*/,/*TODO 6*/b/*TODO 7*/)/*TODO 8*/{/*TODO 9*/if/*TODO 10*/ (/*TODO 11*/a/*TODO 12*/ ===/*TODO 13*/ b/*TODO 14*/)/*TODO 15*/ {/*TODO 16*/ b =/*TODO 17*/ true; /*TODO 18*/} /*TODO 19*/else/*TODO 20*/ {/*TODO 21*/ return/*TODO 22*/ false;/*TODO 23*/ }/*TODO 24*/}/*TODO 25*/",
				getTodos(25),
				lintoptions: new JSLintOptions() { BoolOptions2 = GetOptions(truthy: "white."), FindTodos = true });
		}

		[TestMethod]
		public void TodoDetection4()
		{
			TestLint(
				@"var todo = 'n'; //not a to-do
// and stodo is not a to-do either
// and todos is not a to-do either",
				new List<string>() { },
				lintoptions: new JSLintOptions() { BoolOptions2 = GetOptions(truthy: "white. plusplus."), FindTodos = true });
		}
	}
}
