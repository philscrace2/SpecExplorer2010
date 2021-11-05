using System.Collections.Generic;

namespace Microsoft.SpecExplorer.VS
{
	internal class AddActionWizardData
	{
		public string ConfigName { get; set; }

		public string ScriptName { get; set; }

		public string ProjectName { get; set; }

		public bool IsConfigToBeCreated { get; set; }

		public bool IsScriptToBeCreated { get; set; }

		public IEnumerable<CodeElementAndContainerPair> CodeElementsToBeImported { get; set; }
	}
}
