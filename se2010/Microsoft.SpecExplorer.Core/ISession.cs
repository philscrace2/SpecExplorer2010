using System.Collections.Generic;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	public interface ISession
	{
		IHost Host { get; }

		ApplicationBase Application { get; }

		string InstallDir { get; }

		string ConfigurationDir { get; }

		IExplorer CreateExplorer(ICollection<string> assemblies, ICollection<string> scripts, ExplorationMode explorationMode, string machine, string outputDir, string replayPath, int? onTheFlyMaximumExperimentCount, IDictionary<string, string> machineSwitches, bool allowEndingAtEventStates);

		ITestCodeGenerator CreateTestCodeGenerator(TransitionSystem transitionSystem);
	}
}
