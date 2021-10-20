using System;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	internal interface IExploringOperator
	{
		ExplorationResult ExplorationResult { get; }

		event EventHandler<ExplorationResultEventArgs> ExplorationResultUpdated;

		void Explore();

		void SuspendExploration();
	}
}
