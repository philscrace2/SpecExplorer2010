using System;
using System.Globalization;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.SpecExplorer.VS
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	internal sealed class ProvideSolutionProps : RegistrationAttribute
	{
		public string PropertyName { get; private set; }

		public ProvideSolutionProps(string propName)
			: base()
		{
			PropertyName = propName;
		}

		public override void Register(RegistrationContext context)
		{
			context.Log.WriteLine(string.Format(CultureInfo.InvariantCulture, "ProvideSolutionProps: ({0} = {1})", new object[2]
			{
				context.ComponentType.GUID.ToString("B"),
				PropertyName
			}));
			Key val = null;
			try
			{
				val = context.CreateKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", new object[2] { "SolutionPersistence", PropertyName }));
				val.SetValue(string.Empty, (object)context.ComponentType.GUID.ToString("B"));
			}
			finally
			{
				if (val != null)
				{
					val.Close();
				}
			}
		}

		public override void Unregister(RegistrationContext context)
		{
			context.RemoveKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", new object[2] { "SolutionPersistence", PropertyName }));
		}
	}
}
