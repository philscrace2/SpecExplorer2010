using System;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	public class ExplorationResultEventArgs : EventArgs
	{
		public ExplorationResult ExplorationResult { get; private set; }

		public ExplorationResultEventArgs(ExplorationResult explorationResult)
		{
			if (explorationResult == null)
			{
				throw new ArgumentNullException("explorationResult");
			}
			ExplorationResult = explorationResult;
		}
	}
}
