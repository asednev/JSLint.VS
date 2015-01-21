using System;

namespace JSLint.Framework.OptionClasses
{
	[Flags]
	public enum IncludeFileType
	{
		None = 0,
		JS = 1,
		HTML = 2,
		CSS = 4,
		Folder = 8
	}
}
