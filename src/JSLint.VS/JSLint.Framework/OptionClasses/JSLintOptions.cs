
using System;
using System.Collections.Generic;

namespace JSLint.Framework.OptionClasses
{
	[Flags]
	public enum Linters
	{
		JSLint = 1,
		JSLintOld = 2,
		JSHint = 4,
		All = int.MaxValue
	}

	public sealed class JSLintOptions
	{
		public JSLintOptions()
		{
			SelectedLinter = Linters.JSLint; //not just for preference for backwards compat too
		}

		public SerializableDictionary<string, bool> BoolOptions2 { get; set; }

		public bool ErrorOnUnused { get; set; }

		public int? IndentSize { get; set; }

		public int? MaxLength { get; set; }

        public int? MaxComplexity { get; set; }
        public int? MaxDepth { get; set; }
        public int? MaxStatements { get; set; }
        public int? MaxParams { get; set; }

		public string QuoteMark { get; set; }

		public Linters SelectedLinter { get; set; }

		public bool FindTodos { get; set; }

		/// <summary>
		///  List of strings predefined
		/// </summary>
		public List<string> PreDefined { get; set; }

		/// <summary>
		///  Creates an (javascript compatible) object that JsLint can use for options.
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, object> ToJsOptionVar(Linters linterToUse)
		{
			Dictionary<string, object> returner = new Dictionary<string, object>();
			
			foreach (KeyValuePair<string, bool> option in BoolOptions2)
			{
				if (LintBooleanSettingModel.AppliesTo(option.Key, linterToUse))
				{
					returner[option.Key.TrimEnd('.')] = option.Value;
				}
			}

			if (PreDefined != null && PreDefined.Count > 0)
			{
				returner["predef"] = PreDefined.ToArray();
			}

			if (MaxLength.HasValue)
			{
				returner["maxlen"] = MaxLength.Value;
			}

			if (IndentSize.HasValue)
			{
				returner["indent"] = IndentSize.Value;
			}

			if (QuoteMark != null)
			{
				switch (QuoteMark)
				{
					case "On":
						returner["quotmark"] = true;
						break;
					case "Off":
						returner["quotmark"] = false;
						break;
					case "Single":
						returner["quotmark"] = "single";
						break;
					case "Double":
						returner["quotmark"] = "double";
						break;
				}
			}

            if (MaxComplexity != null)
            {
                returner["maxcomplexity"] = MaxComplexity.Value;
            }

            if (MaxDepth != null)
            {
                returner["maxdepth"] = MaxDepth.Value;
            }

            if (MaxStatements != null)
            {
                returner["maxstatements"] = MaxStatements.Value;
            }

            if (MaxParams != null)
            {
                returner["maxparams"] = MaxParams.Value;
            }

			returner["maxerr"] = 499; //we stop at 499 errors in connect so we may as well let lint stop and generate a 499th error telling us where it got to.

			return returner;
		}

		public static JSLintOptions Default
		{
			get
			{
				return new JSLintOptions
				{
					BoolOptions2 = LintBooleanSettingModel.GetDefaultOptions(), 
					IndentSize = 4,
					MaxLength = 90,
					ErrorOnUnused = true,
				};
			}
		}
	}
}

