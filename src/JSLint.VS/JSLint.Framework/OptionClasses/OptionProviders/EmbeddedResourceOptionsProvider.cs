using System;
using System.IO;
using System.Reflection;

namespace JSLint.Framework.OptionClasses.OptionProviders
{
	public class EmbeddedResourceOptionsProvider : OptionsProviderBase
	{
		private Assembly _callingAssembly;
		private String _resourceName;
		private Options _options;

		public EmbeddedResourceOptionsProvider(String providerName, Assembly callingAssembly, String resourceName)
			: base(providerName)
		{
			_callingAssembly = callingAssembly;
			_resourceName = resourceName;
		}

		public override Options GetOptions()
		{
			if (_options == null)
				_options = LoadOptions();
			return _options;
		}

		private Options LoadOptions()
		{
			Options options;
			using (Stream s = _callingAssembly.GetManifestResourceStream(_resourceName))
			{
				OptionsSerializer serializer = new OptionsSerializer();
				options = serializer.Deserialize(s);
			}
			return options;
		}

		public override void Save(Options options)
		{
			throw new NotImplementedException();
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override void Refresh()
		{
			_options = null;
		}
	}
}
