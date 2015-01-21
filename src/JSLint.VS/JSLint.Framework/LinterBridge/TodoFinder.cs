using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JSLint.Framework.LinterBridge
{
    public static class TodoFinder
    {
        private const string FindTodoRegexString = @"(/\*[\r\w\n\W]*?\*/)|(//.*?([\r\n]|$))";
        private static Regex FindTodoRegex = new Regex(FindTodoRegexString, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static List<JSLintError> FindTodos(string file)
        {
            int lastindex = -1;
            int lastline = 1;
            List<JSLintError> todos = new List<JSLintError>();
            MatchCollection matches = FindTodoRegex.Matches(file);
            foreach(Match match in matches)
            {
                int indexOfTODO = -1;

                while (true)
                {
                    indexOfTODO = match.Captures[0].Value.IndexOf("TODO", indexOfTODO+1, StringComparison.InvariantCultureIgnoreCase);
                    if (indexOfTODO < 0)
                    {
                        break;
                    }

                    char characterBefore = indexOfTODO > 2 ? match.Captures[0].Value[indexOfTODO - 1] : ' ';
                    char characterAfter = indexOfTODO + 4 >= match.Captures[0].Value.Length ? ' ' : match.Captures[0].Value[indexOfTODO + 4];

                    if (char.IsWhiteSpace(characterBefore) && !char.IsLetter(characterAfter))
                    {
                        break;
                    }
                }

                if (indexOfTODO < 0)
                {
                    continue;
                }

                while (true)
                {
                    int i = file.IndexOf('\n', lastindex+1);
                    if (i > match.Index || i <= 0)
                    {
                        break;
                    }
                    lastline++;
                    lastindex = i;
                }

                JSLintError jse = new JSLintError() { Column = match.Index - lastindex, Line = lastline, Message = match.Captures[0].Value.Substring(indexOfTODO).Trim('/', '*').Replace('\n', ' ').Replace('\r', ' ').Trim() };
                todos.Add(jse);
            }
            return todos;
        }
    }
}
