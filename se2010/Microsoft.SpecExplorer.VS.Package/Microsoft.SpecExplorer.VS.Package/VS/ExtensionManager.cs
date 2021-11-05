using System.Collections.Generic;
using Microsoft.SpecExplorer.VS.Common;

namespace Microsoft.SpecExplorer.VS
{
	internal class ExtensionManager : IExtensionManager
	{
		private Dictionary<string, IExtensionService> extensionServices = new Dictionary<string, IExtensionService>();

		internal ExtensionManager()
		{
		}

		public void RegisterExtension(string key, IExtensionService service)
		{
			extensionServices[key] = service;
		}

		public bool TyrGetExtensionData(string key, object inputValue, out object outputValue)
		{
			IExtensionService value;
			if (extensionServices.TryGetValue(key, out value))
			{
				return value.TryGetExtensionData(inputValue, out outputValue);
			}
			outputValue = null;
			return false;
		}
	}
}
