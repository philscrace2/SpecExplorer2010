using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	public interface IExplorerUpdateUI
	{
		void ShowStatistics(ExplorationStatistics statistics);

		void ShowStatistics(TestingStatistics statistics);

		void SwitchState(ExplorationState state);

		void UpdateExplorationResult(ExplorationResult explorationResult);

		void ShowTestCaseFinishedProgress(TestCaseFinishedEventArgs args);
	}
}
