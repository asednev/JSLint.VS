using System;
using System.Collections.Generic;
using JSLint.Framework.OptionClasses;
using Noesis.Javascript;

namespace JSLint.Framework.LinterBridge
{
	/// <summary>
	///  Constructs an object capable of linting javascript files and returning the result of JS Lint
	/// </summary>
    public sealed class JSLinter :
        IDisposable
    {
        private object _lock = new Object();

        public JSLinter()
        {
        }


        private Dictionary<Linters, JavascriptContext> _contexts = new Dictionary<Linters, JavascriptContext>();
        private JavascriptContext GetLinterContext(Linters linter)
        {
            if (_contexts.ContainsKey(linter))
            {
                return _contexts[linter];
            }
            JavascriptContext context = null;
            switch (linter)
            {
                case Linters.JSLint:
                    context = setupContext("fulljslint.js");
                    context.SetParameter("linterName", "JSLINT");
                    break;
                case Linters.JSLintOld:
                    context = setupContext("old_fulljslint.js");
                    context.SetParameter("linterName", "JSLINT");
                    break;
                case Linters.JSHint:
                    context = setupContext("jshint.js");
                    context.SetParameter("linterName", "JSHINT");
                    break;
                default:
                    throw new Exception("Invalid linter to create context for");
            }
            _contexts.Add(linter, context);
            return context;
        }

        private static JavascriptContext setupContext(string jslintFilename)
        {
            JavascriptContext context = new JavascriptContext();
            string JSLint = Utility.ReadResourceFile("JS." + jslintFilename);

            JSLint += Utility.ReadResourceFile("JS.LintRunner.js");
            context.Run(JSLint);

            return context;
        }

		public List<JSLintError> Lint(string javascript, bool isJavaScript)
		{
            return Lint(javascript, JSLintOptions.Default, isJavaScript);
		}

        public List<JSLintError> Lint(string javascript, JSLintOptions configuration, bool isJavaScript)
        {
			if (string.IsNullOrEmpty(javascript))
			{
				throw new ArgumentNullException("javascript");
			}

			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

            lock (_lock)
            {
                Linters linterToUse = isJavaScript ? configuration.SelectedLinter : Linters.JSLint;
                JavascriptContext context = GetLinterContext(linterToUse);

				LintDataCollector dataCollector = new LintDataCollector(configuration.ErrorOnUnused);

	            var options = configuration.ToJsOptionVar(linterToUse);

                // Setting the externals parameters of the context
				context.SetParameter("dataCollector", dataCollector);
                context.SetParameter("javascript", javascript);
				context.SetParameter("options", options);

                // Running the script
                context.Run("lintRunner(linterName, dataCollector, javascript, options);");

                return dataCollector.Errors;
            }
        }

		private class LintDataCollector
		{
			private List<JSLintError> _errors = new List<JSLintError>();
			private readonly bool _processUnuseds;

			public List<JSLintError> Errors
			{
				get { return _errors; }
			}

			public LintDataCollector(bool processUnuseds)
			{
				_processUnuseds = processUnuseds;
			}

			public void ProcessData(object data)
			{
				Dictionary<string, object> dataDict = data as Dictionary<string, object>;

				if (dataDict != null)
				{
					Action<Dictionary<string, object>> processor = (error) =>
						{
							JSLintError jsError = new JSLintError();
							if (error.ContainsKey("line") )
							{
                                if (error["line"] is int)
                                {
                                    jsError.Line = (int)error["line"];
                                }
							}

							if (error.ContainsKey("character"))
							{
                                if (error["character"] is int)
                                {
                                    jsError.Column = ((int)error["character"]) + 1;
                                }
							}

							if (jsError.Column == 0)
							{
								jsError.Column = 1;
							}

							if (error.ContainsKey("reason"))
							{
                                if (error["reason"] is string)
                                {
                                    jsError.Message = (string)error["reason"];
                                }
							}

							if (error.ContainsKey("evidence"))
							{
                                if (error["evidence"] is string)
                                {
                                    jsError.Evidence = (string)error["evidence"];
                                }
							}

							_errors.Add(jsError);
						};

					if (dataDict.ContainsKey("errors"))
					{
						ProcessListOfObject(dataDict["errors"], processor);
					}

					if (_processUnuseds && dataDict.ContainsKey("unused"))
					{
						ProcessListOfObject(dataDict["unused"], (unused) =>
						{
							JSLintError jsError = new JSLintError();
							if (unused.ContainsKey("line"))
							{
								jsError.Line = (int)unused["line"];
							}

                            jsError.Column = 1;

							if (unused.ContainsKey("name"))
							{
								jsError.Message = string.Format("Unused Variable '{0}'.", unused["name"]);
							}

							_errors.Add(jsError);
						});
					}

					if (dataDict.ContainsKey("vsdocerr"))
					{
						ProcessListOfObject(dataDict["vsdocerr"], processor);
					}
				}
			}

			private void ProcessListOfObject(object obj, Action<Dictionary<string, object>> processor)
			{
				object[] array = obj as object[];

				if (array != null)
				{
					foreach (object objItem in array)
					{
						Dictionary<string, object> objItemDictionary = objItem as Dictionary<string, object>;

						if (objItemDictionary != null)
						{
							processor(objItemDictionary);
						}
					}
				}
			}
		}

        #region IDisposable Members

        public void Dispose()
        {
            if (_contexts == null)
            {
                return;
            }

            foreach (JavascriptContext context in _contexts.Values)
            {
                context.Dispose();
            }
            _contexts = null;
        }

        #endregion
    }
}
