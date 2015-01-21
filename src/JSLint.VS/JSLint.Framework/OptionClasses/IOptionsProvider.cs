using System;

namespace JSLint.Framework.OptionClasses
{
    public interface IOptionsProvider
    {
		String Name { get; }
        Options GetOptions();
        void Save(Options options);
        bool IsReadOnly { get; }
        void Refresh();
    }
}
