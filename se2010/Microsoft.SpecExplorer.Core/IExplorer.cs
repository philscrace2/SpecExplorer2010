using System;
using System.Threading;
using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	public interface IExplorer : IDisposable
	{
		ISession Session { get; }

		bool Sandboxed { get; set; }

		ExplorationState State { get; }

		ExplorationResult ExplorationResult { get; }

		event EventHandler<ExplorationResultEventArgs> ExplorationResultUpdated;

		event EventHandler<ExplorationStatisticsEventArgs> ExplorationStatisticsProgress;

		event EventHandler<TestingStatisticsEventArgs> TestingStatisticsProgress;

		event EventHandler<TestCaseFinishedEventArgs> TestCaseFinishedProgress;

		event EventHandler<ExplorationStateChangedEventArgs> ExplorationStateChanged;

		WaitHandle StartBuilding();

		WaitHandle StartExploration();

		void Abort();
	}
}
