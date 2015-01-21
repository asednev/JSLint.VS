using System;
using System.Globalization;
using Microsoft.VisualStudio.Shell;

namespace JSLint.VS
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class ProvideSolutionProps : RegistrationAttribute
	{
		public string PropertyName { get; private set; }

		public ProvideSolutionProps(String propertyName)
		{
			PropertyName = propertyName;
		}

		public override void Register(RegistrationContext ctx)
		{
			ctx.Log.WriteLine(Format("{0}: ({1} = {2})", typeof(ProvideSolutionProps).Name, GetComponentTypeGuidAsString(ctx), PropertyName));

			using(var key = ctx.CreateKey(GetKeyName()))
			{
				key.SetValue(String.Empty, GetComponentTypeGuidAsString(ctx));
			}
		}

		public override void Unregister(RegistrationContext context)
		{
			context.RemoveKey(GetKeyName());
		}

		private String GetComponentTypeGuidAsString(RegistrationContext ctx)
		{
			return ctx.ComponentType.GUID.ToString("B");
		}

		private String GetKeyName()
		{
			return Format("{0}\\{1}", "SolutionPersistence", PropertyName);
		}
		
		private String Format(String format, params object[] args)
		{
			return String.Format(CultureInfo.InvariantCulture, format, args);
		}
	}
}
