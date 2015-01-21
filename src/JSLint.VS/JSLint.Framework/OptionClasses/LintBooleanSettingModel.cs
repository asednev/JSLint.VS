using System.Collections.Generic;

namespace JSLint.Framework.OptionClasses
{
    public class LintBooleanSettingModel
    {
        public LintBooleanSettingModel(string jsname, string label, string tooltip, Linters appliesTo, bool onByDefault = false)
        {
            JSName = jsname;
            Tooltip = tooltip;
            LintersAppliesTo = appliesTo;
            Label = label;
            DefaultOn = onByDefault;
        }

        public string JSName { get; set; }
        public string Label { get; set; }
        public string Tooltip { get; set; }

        public bool DefaultOn { get; set; }
        public Linters LintersAppliesTo { get; set; }

        public static SerializableDictionary<string, bool> GetDefaultOptions()
        {
            var returner = new SerializableDictionary<string, bool>();

            foreach(var lintBoolOption in AllOptions) 
            {
                returner[lintBoolOption.JSName] = lintBoolOption.DefaultOn;
            }

            return returner;
        }

        public static bool AppliesTo(string jsName, Linters linter)
        {
            foreach (LintBooleanSettingModel option in AllOptions)
            {
                if (option.JSName == jsName)
                {
                    return (option.LintersAppliesTo & linter) > 0;
                }
            }
            return false;
        }

        private static IEnumerable<LintBooleanSettingModel> _allOptions = null;
        public static IEnumerable<LintBooleanSettingModel> AllOptions
        {
            get
            {
                if (_allOptions == null)
                {
                    _allOptions = GetAllOptions();
                }
                return _allOptions;
            }
        }

        public static IEnumerable<LintBooleanSettingModel> GetAllOptions()
        {
            yield return new LintBooleanSettingModel("adsafe", "ADsafe", "if ADsafe should be enforced", Linters.JSLintOld);
            yield return new LintBooleanSettingModel("anon", "Tolerate Anonymous whitespace", "if the space may be omitted in anonymous function declarations", Linters.JSLint);
            yield return new LintBooleanSettingModel("asi", "Tolerate automatic semicolon insertion", "if automatic semicolon insertion should be tolerated", Linters.JSHint);
            yield return new LintBooleanSettingModel("bitwise", "Disallow bitwise operators", "if bitwise operators should not be allowed", Linters.JSHint | Linters.JSLintOld, true);
            yield return new LintBooleanSettingModel("bitwise.", "Tolerate bitwise operators", "if bitwise operators should be allowed", Linters.JSLint);
            yield return new LintBooleanSettingModel("boss", "Tolerate != null etc.", "if advanced usage of assignments and == should be allowed", Linters.JSHint);
            yield return new LintBooleanSettingModel("browser", "Assume a browser", "if the standard browser globals should be predefined", Linters.All);
            yield return new LintBooleanSettingModel("camelcase", "Identifiers camel case", "if identifiers should be required to be in camel case", Linters.JSHint);
            yield return new LintBooleanSettingModel("cap", "Tolerate upper case HTML", "if upper case HTML should be allowed", Linters.JSLintOld | Linters.JSLint);
            yield return new LintBooleanSettingModel("continue", "Tolerate continue", "if continue statements should be allowed", Linters.JSLint);
            yield return new LintBooleanSettingModel("couch", "Assume CouchDB", "if CouchDB globals should be predefined", Linters.JSHint);
            yield return new LintBooleanSettingModel("css", "Tolerate CSS workarounds", "if CSS workarounds should be tolerated", Linters.JSLintOld | Linters.JSLint);
            yield return new LintBooleanSettingModel("curly", "Always require curly braces", "if curly braces around blocks should be required (even in if/for/while)", Linters.JSHint, true);
            yield return new LintBooleanSettingModel("debug", "Tolerate debugger", "if debugger statements should be allowed", Linters.All);
            yield return new LintBooleanSettingModel("devel", "Assume development globals", "if logging globals should be defined (console, alert, etc.)", Linters.All);
            yield return new LintBooleanSettingModel("dojo", "Assume dojo", "if dojo globals should be defined", Linters.JSHint);
            yield return new LintBooleanSettingModel("eqeqeq", "Disallow == and !=", "if === or !== should be required", Linters.JSLintOld | Linters.JSHint);
            yield return new LintBooleanSettingModel("eqeq", "Tolerate == and !=", "if === or !== should be allowed", Linters.JSLint);
            yield return new LintBooleanSettingModel("eqnull", "Tolerate == null", "if == null comparisons should be tolerated", Linters.JSHint);
			yield return new LintBooleanSettingModel("es3", "Require adherence to ES3", "if ECMAScript 3 compliance should be required", Linters.JSHint);
			yield return new LintBooleanSettingModel("es5", "Tolerate ES5 syntax", "if ES5 syntax should be allowed", Linters.All);
            yield return new LintBooleanSettingModel("esnext", "Tolerate ES.next syntax", "if es.next specific syntax should be allowed", Linters.JSHint);
            yield return new LintBooleanSettingModel("evil", "Tolerate eval", "if eval should be allowed", Linters.All);
            yield return new LintBooleanSettingModel("expr", "Allow ExpressionStatement", "if ExpressionStatement should be allowed as Programs", Linters.JSHint);
            yield return new LintBooleanSettingModel("forin", "Require filtered for in", "if for in statements must filter", Linters.JSLintOld | Linters.JSHint);
            yield return new LintBooleanSettingModel("forin.", "Tolerate unfiltered for in", "Allow unfiltered for in statements", Linters.JSLint);
            yield return new LintBooleanSettingModel("fragment", "Tolerate HTML fragments", "if HTML fragments should be allowed", Linters.JSLintOld | Linters.JSLint);
            yield return new LintBooleanSettingModel("funcscope", "Only function scope", "if only function scope should be used for scope tests", Linters.JSHint);
            yield return new LintBooleanSettingModel("globalstrict", "Allow global ES5 strict", "if global \"use strict\"; should be allowed (also enables 'strict')", Linters.JSHint);
            yield return new LintBooleanSettingModel("immed", "Require parens for immed invocations", "if immediate invocations must be wrapped in parens", Linters.JSLintOld | Linters.JSHint);
            yield return new LintBooleanSettingModel("iterator", "Tolerate __iterator__", "if the `__iterator__` property should be allowed", Linters.JSHint);
            yield return new LintBooleanSettingModel("jquery", "Assume jQuery", "if jQuery globals should be predefined", Linters.JSHint);
            yield return new LintBooleanSettingModel("lastsemic", "Omit Last semi-colon", "if semicolons may be ommitted for the trailing statements inside of a one-line blocks.", Linters.JSHint);
            yield return new LintBooleanSettingModel("latedef", "Disallow use before definition", "Disallow the use of functions before their definition", Linters.JSHint, true);
            yield return new LintBooleanSettingModel("laxbreak", "Tolerate sloppy line breaking", "if line breaks should not be checked", Linters.All);
            yield return new LintBooleanSettingModel("laxcomma", "Tolerate comma line breaking", "if line breaks should not be checked around commas", Linters.JSHint);
            yield return new LintBooleanSettingModel("loopfunc", "Tolerate functions in loops", "if functions should be allowed to be defined within loops", Linters.JSHint);
            yield return new LintBooleanSettingModel("mootools", "Define MooTools globals", "Define MooTools globals", Linters.JSHint);
            yield return new LintBooleanSettingModel("multistr", "Allow Multi-line strings", "Allow multiline strings", Linters.JSHint);
            yield return new LintBooleanSettingModel("newcap", "Require Initial Caps for constructors", "if constructor names must be capitalized", Linters.JSLintOld | Linters.JSHint, true);
            yield return new LintBooleanSettingModel("newcap.", "Tolerate uncapitalized constructors", "if constructor names capitalization is ignored", Linters.JSLint);
            yield return new LintBooleanSettingModel("noarg", "Disallow arguments.caller and callee", "if arguments.caller and arguments.callee should be disallowed", Linters.JSHint, true);
            yield return new LintBooleanSettingModel("node", "Assume Node.js", "if the Node.js environment globals should be predefined", Linters.JSHint | Linters.JSLint);
            yield return new LintBooleanSettingModel("noempty", "Disallow empty blocks", "if empty blocks should be disallowed", Linters.JSHint, true);
            yield return new LintBooleanSettingModel("nonew", "Disallow new for side-effects", "if using `new` for side-effects should be disallowed", Linters.JSHint, true);
            yield return new LintBooleanSettingModel("nomen", "Disallow dangling __ in identifiers", "if names should be checked and rejected if there is a dangling _ (usually meant to suggest the variable is private)", Linters.JSHint | Linters.JSLintOld, true);
            yield return new LintBooleanSettingModel("nomen.", "Tolerate dangling __ in identifiers", "if names may have _ at begining or end(usually meant to suggest the variable is private)", Linters.JSLint);
            yield return new LintBooleanSettingModel("nonstandard", "Predefine browser non-standard", "if non-standard (but widely adopted) globals should be predefined", Linters.JSHint);
            yield return new LintBooleanSettingModel("on", "Tolerate HTML event handlers", "if HTML event handlers should be allowed", Linters.JSLintOld | Linters.JSLint);
            yield return new LintBooleanSettingModel("onecase", "Tolerate one case switch", "if one case switch statements should be allowed", Linters.JSHint);
            yield return new LintBooleanSettingModel("onevar", "Require only one var per function", "if only one var statement per function should be allowed", Linters.JSHint | Linters.JSLintOld, true);
            yield return new LintBooleanSettingModel("passfail", "Stop on first error", "if the scan should stop on first error", Linters.All);
            yield return new LintBooleanSettingModel("plusplus", "Disallow ++ and --", "if increment++/decrement-- should not be allowed", Linters.JSHint | Linters.JSLintOld, true);
            yield return new LintBooleanSettingModel("plusplus.", "Tolerate ++ and --", "if increment++/decrement-- should be allowed", Linters.JSLint);
            yield return new LintBooleanSettingModel("proto", "Tolerate __proto__", "if the `__proto__` property should be allowed", Linters.JSHint);
            yield return new LintBooleanSettingModel("prototypejs", "Assume prototype", "if Prototype and Scriptaculous globals shoudl be predefined", Linters.JSHint);
            yield return new LintBooleanSettingModel("regexp", "Disallow insecure literal regexp", "if the insecure . and [^...] should not be allowed in regexp literals (/RegExp/)", Linters.JSHint | Linters.JSLintOld, true);
            yield return new LintBooleanSettingModel("regexp.", "Tolerate insecure literal regexp", "if the insecure . and [^...] should be allowed in regexp literals (/RegExp/)", Linters.JSLint);
            yield return new LintBooleanSettingModel("regexdash", "Tolerate regexp unescaped dash", "if unescaped last dash (-) inside brackets should be tolerated", Linters.JSHint);
            yield return new LintBooleanSettingModel("rhino", "Assume Rhino", "if the Rhino environment globals should be predefined", Linters.All);
            yield return new LintBooleanSettingModel("safe", "Safe subset", "if use of some browser features should be restricted", Linters.JSLintOld);
            yield return new LintBooleanSettingModel("scripturl", "Allow script URL", "if script-targeted URLs should be tolerated", Linters.JSHint);
            yield return new LintBooleanSettingModel("shadow", "Tolerate variable shadowing", "Allow a var definition to occur multiple times", Linters.JSHint);
            yield return new LintBooleanSettingModel("sloppy", "Tolerate missing 'use strict' pragma", "Allow \"use strict\"; pragma to be missing from the begining of functions", Linters.JSLint, true);
            yield return new LintBooleanSettingModel("smarttabs", "Tolerate Smart Tabs", "if smarttabs should be tolerated (http://www.emacswiki.org/emacs/SmartTabs)", Linters.JSHint);
            yield return new LintBooleanSettingModel("stupid", "Tolerate Stupidity", "Node.js related option for syncronous methods", Linters.JSLint, true);
            yield return new LintBooleanSettingModel("sub", "Tolerate inefficient subscripting", "if all forms of subscript notation are tolerated", Linters.All);
            yield return new LintBooleanSettingModel("supernew", "tolerate certain new calls", "if `new function () { ... };` and `new Object;` should be tolerated", Linters.JSHint);
            yield return new LintBooleanSettingModel("strict", "Require \"use strict\"", "require the \"use strict\"; pragma at the begining of functions", Linters.JSLintOld | Linters.JSHint);
            yield return new LintBooleanSettingModel("trailing", "strict trailing whitespace", "if trailing whitespace rules apply", Linters.JSHint);
            yield return new LintBooleanSettingModel("todo", "Tolerate TODO's", "If //TODO comments are tolerated", Linters.JSLint);
            yield return new LintBooleanSettingModel("undef", "Disallow undefined variables", "if variables should be declared before used", Linters.JSHint | Linters.JSLintOld, true);
            yield return new LintBooleanSettingModel("undef.", "Tolerate misordered definitions", "if variables can be declared out of order", Linters.JSLint);
            yield return new LintBooleanSettingModel("unused", "Disallow unused variables", "if unused variables should be disallowed", Linters.JSHint);
            yield return new LintBooleanSettingModel("unparam", "Tolerate unused params", "Suppress warning on unused parameter variables", Linters.JSLint);
            yield return new LintBooleanSettingModel("validthis", "Tolerate this everywhere", "if 'this' inside a non-constructor function is valid. This is a function scoped option only.", Linters.JSHint);
            yield return new LintBooleanSettingModel("vars", "Tolerate many vars per function", "Allow more than one var statement per function", Linters.JSLint);
            yield return new LintBooleanSettingModel("white", "Strict whitespace", "if strict whitespace rules apply", Linters.JSLintOld | Linters.JSHint, true);
            yield return new LintBooleanSettingModel("white.", "Tolerate sloppy whitespace", "if sloppy whitespace is tolerated", Linters.JSLint);
            yield return new LintBooleanSettingModel("widget", "Assume a Yahoo Widget", "if the Yahoo Widgets globals should be predefined", Linters.JSLintOld);
            yield return new LintBooleanSettingModel("windows", "Assume windows", "if MS Windows-specific globals should be predefined", Linters.JSLint | Linters.JSLintOld);
            yield return new LintBooleanSettingModel("withstmt", "Tolerate with statement", "Allow with statement", Linters.JSHint);
            yield return new LintBooleanSettingModel("worker", "Assume workers", "if worker specific variables should be defined", Linters.JSHint);
            yield return new LintBooleanSettingModel("wsh", "Assume windows script host", "if windows script host specific globals should be predefined", Linters.JSHint);
            yield return new LintBooleanSettingModel("yui", "Assume YUI", "if yu specific variables should be defined", Linters.JSHint);
        }
    }
}
