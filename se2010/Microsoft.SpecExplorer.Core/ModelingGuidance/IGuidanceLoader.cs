using System.Collections.Generic;
using System.IO;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	public interface IGuidanceLoader
	{
		IEnumerable<IGuidance> LoadedGuidanceList { get; }

		IGuidance LoadGuidance(Stream GuidanceDataStream);

		void LoadGuidanceUsage(string combinedUsageString);

		void UnloadGuidanceList();
	}
}
