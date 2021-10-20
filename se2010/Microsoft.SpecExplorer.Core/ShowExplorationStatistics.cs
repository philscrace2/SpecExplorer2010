using System;
using Microsoft.ActionMachines;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	internal class ShowExplorationStatistics : ExplorerEvent
	{
		public ExplorationStatistics Statistics { get; private set; }

		public ShowExplorationStatistics(ExplorationStatistics statistics)
			: base(ExplorerEventType.ShowExplorationStatistics)
		{
			if (statistics == null)
			{
				throw new ArgumentNullException("statistics");
			}
			Statistics = statistics;
		}
	}
}
