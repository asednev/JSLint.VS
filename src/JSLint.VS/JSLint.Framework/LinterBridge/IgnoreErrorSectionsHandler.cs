using System.Collections.Generic;
using JSLint.Framework.OptionClasses;

namespace JSLint.Framework.LinterBridge
{
	public class IgnoreErrorSectionsHandler
	{
		public IgnoreErrorSectionsHandler(string file, Options options = null)
		{
			if (options == null)
			{
				options = OptionsProviderRegistry.CurrentOptions;
			}
			SectionsToIgnore = new List<IgnoreErrorSection>();
			string start = options.IgnoreErrorStart,
					end = options.IgnoreErrorEnd,
					ignoreline = options.IgnoreErrorLine;

			int line = 1, startLineCharacter = 0, endLineCharacter = 0, character = 0, i;
			IgnoreErrorSection currentSection = null;
			bool lookAtStartEnd = true, lookAtLines = true;

			if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
			{
				lookAtStartEnd = false;
			}

			if (string.IsNullOrEmpty(ignoreline))
			{
				lookAtLines = false;
			}

			while (lookAtLines || lookAtStartEnd)
			{
				endLineCharacter = file.IndexOf('\n', startLineCharacter);
				character = startLineCharacter;

				if (endLineCharacter < 0)
				{
					break;
				}

				while (true)
				{
					if (currentSection != null)
					{
						 i = file.IndexOf(end, character, endLineCharacter - character);
						 if (i >= 0)
						 {
							 currentSection.EndLine = line;
							 currentSection.EndCol = i - startLineCharacter;
							 SectionsToIgnore.Add(currentSection);
							 currentSection = null;
							 character = i;
							 continue;
						 }
						 break;
					}

					i = file.IndexOf(ignoreline, character, endLineCharacter - character);

					if (i >= 0)
					{
						SectionsToIgnore.Add(new IgnoreErrorSection() { StartLine = line, EndLine = line, StartCol = -1 });
						break;
					}

					i = file.IndexOf(start, character, endLineCharacter - character);

					if (i >= 0)
					{
						currentSection = new IgnoreErrorSection() { StartLine = line, StartCol = i - startLineCharacter };
						character = i;
					}
					else
					{
						break;
					}
				}
				line++;
				startLineCharacter = endLineCharacter + 1;
			}
		}

		public class IgnoreErrorSection
		{
			public int StartLine { get; set; }
			public int EndLine { get; set; }
			public int StartCol { get; set; }
			public int EndCol { get; set; }
		}

		public List<IgnoreErrorSection> SectionsToIgnore { get; set; }

		public bool IsErrorIgnored(int line, int col)
		{
			foreach (IgnoreErrorSection section in SectionsToIgnore)
			{
				if (line < section.StartLine || line > section.EndLine)
				{
					continue;
				}

				if (section.StartLine == section.EndLine)
				{
					if (section.StartCol == -1)
					{
						return true;
					}
					else
					{
						if (col >= section.StartCol && col <= section.EndCol)
						{
							return true;
						}
					}
				}
				else
				{
					if (line > section.StartLine && line < section.EndLine)
					{
						return true;
					}

					if (line == section.StartLine && col >= section.StartCol)
					{
						return true;
					}

					if (line == section.EndLine && col <= section.EndCol)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
