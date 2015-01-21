using System.Collections.Generic;
using System.Linq;
using JSLint.Framework.LinterBridge;
using JSLint.Framework.OptionClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JSLint.VS.Test
{
	[TestClass]
	public abstract class TestsBase
	{
		protected JSLinter _linter;

		public SerializableDictionary<string, bool> GetOptions(string truthy = null, string falsy = null)
		{
			var returner = LintBooleanSettingModel.GetDefaultOptions();
			if (truthy != null)
			{
				foreach (string truthyoption in truthy.Split(' '))
				{
					returner[truthyoption] = true;
				}
			}
			if (falsy != null)
			{
				foreach (string falsyoption in falsy.Split(' '))
				{
					returner[falsyoption] = false;
				}
			}

			return returner;
		}

		protected void TestLint(string javascript, List<string> errorsExpected, JSLintOptions lintoptions = null, List<string> todos = null, Options options = null)
		{
			if (options == null)
			{
				options = OptionsProviderRegistry.CurrentOptions;

				if (lintoptions != null)
				{
					foreach (var boolOptionKey in lintoptions.BoolOptions2.Keys)
					{
						var boolOptionValue = lintoptions.BoolOptions2[boolOptionKey];

						if (!options.JSLintOptions.BoolOptions2.ContainsKey(boolOptionKey))
						{
							options.JSLintOptions.BoolOptions2.Add(boolOptionKey, boolOptionValue);
						}
						else
						{
							options.JSLintOptions.BoolOptions2[boolOptionKey] = boolOptionValue;
						}
					}
				}
			}

			lintoptions = options.JSLintOptions;

			IgnoreErrorSectionsHandler ignoreErrorHandler = new IgnoreErrorSectionsHandler(javascript);

			IEnumerable<JSLintError> errors = _linter.Lint(javascript, lintoptions, true);

			errors = errors.Where(a => !ignoreErrorHandler.IsErrorIgnored(a.Line, a.Column));

			if (lintoptions.FindTodos)
			{
				errors = errors.Concat(TodoFinder.FindTodos(javascript));
			}

			string errorMessage = "got ";

			for (var i = 0; i < errors.Count(); i++)
			{
				errorMessage += "<" + errors.ElementAt(i).Message + "> ";
			}

			for (var i = 0; i < errorsExpected.Count; i++)
			{
				if (errors.Count() <= i)
				{
					break;
				}
				Assert.AreEqual(errorsExpected[i], errors.ElementAt(i).Message, errorMessage);
			}

			Assert.AreEqual(errorsExpected.Count, errors.Count(), errorMessage);
		}

	}
}
