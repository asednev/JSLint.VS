using System.Collections.Generic;
using System.IO;

namespace JSLint.Framework.OptionClasses
{
	public static class OptionsProviderRegistry
	{
		static OptionsProviderRegistry()
		{
			_optionsProviders = new Stack<IOptionsProvider>();
		}

		private static Stack<IOptionsProvider> _optionsProviders;

		public static Options CurrentOptions
		{
			get
			{
				return _optionsProviders.Peek().GetOptions();
			}
		}

		public static IOptionsProvider CurrentOptionsProvider
		{
			get
			{
				return GetCurrentOptionsProvider();
			}
		}

		public static void PushOptionsProvider(IOptionsProvider provider)
		{
			_optionsProviders.Push(provider);
		}

		public static void PopOptionsProvider()
		{
			_optionsProviders.Pop();
		}

		public static Options ReloadCurrent()
		{
			var provider = GetCurrentOptionsProvider();
			provider.Refresh();
			return provider.GetOptions();
		}

		public static void SaveChanges(Options options)
		{
			lock(options)
			{
				GetCurrentOptionsProvider().Save(options);
			}
		}

		private static IOptionsProvider GetCurrentOptionsProvider()
		{
			return _optionsProviders.Peek();
		}

		public static Options Import(string filename)
		{
			Options options = null;
			try
			{
				if (File.Exists(filename))
				{
					using (FileStream fin = File.OpenRead(filename))
					{
						var serializer = new OptionsSerializer();
						options = serializer.Deserialize(fin);
						var currentProvider = GetCurrentOptionsProvider();
						if (!currentProvider.IsReadOnly)
						{
							currentProvider.Save(options);
						}
					}
				}
			}
			catch
			{
				options = new Options();
			}
			return options;
		}

		public static void Export(Options options, string filename)
		{
			using (FileStream fout = File.Create(filename))
			{
				OptionsSerializer serializer = new OptionsSerializer();
				serializer.Serialize(fout, options);
			}
		}
	}
}
