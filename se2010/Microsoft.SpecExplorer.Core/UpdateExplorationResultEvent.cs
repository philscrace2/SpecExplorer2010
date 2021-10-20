using System;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	internal class UpdateExplorationResultEvent : ExplorerEvent
	{
		public ExplorationResult ExplorationResult { get; private set; }

		public UpdateExplorationResultEvent(ExplorationResult explorationResult)
			: base(ExplorerEventType.UpdateExplorationResult)
		{
			ExplorationResult = explorationResult;
		}
	}
}
