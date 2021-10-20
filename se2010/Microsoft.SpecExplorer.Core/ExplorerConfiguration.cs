using System;
using System.Collections.Generic;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	public class ExplorerConfiguration
	{
		public string Machine { get; private set; }

		public ICollection<string> Assemblies { get; private set; }

		public ICollection<string> Scripts { get; private set; }

		public IDictionary<string, string> MachineSwitches { get; private set; }

		public ExplorationMode ExplorationMode { get; private set; }

		public string OutputDirectory { get; private set; }

		public int? OnTheFlyMaximumExperimentCount { get; private set; }

		public string ReplayResultPath { get; private set; }

		public bool AllowEndingAtEventStates { get; private set; }

		public ExplorerConfiguration(ICollection<string> assemblies, ICollection<string> scripts, ExplorationMode explorationMode, string machine, IDictionary<string, string> machineSwitches, string outputDir, string replayResultPath, int? onTheFlyMaximumExperimentCount, bool allowEndingAtEventStates)
		{
			Assemblies = assemblies;
			Scripts = scripts;
			ExplorationMode = explorationMode;
			Machine = machine;
			MachineSwitches = machineSwitches;
			OutputDirectory = outputDir;
			ReplayResultPath = replayResultPath;
			OnTheFlyMaximumExperimentCount = onTheFlyMaximumExperimentCount;
			AllowEndingAtEventStates = allowEndingAtEventStates;
		}
	}
}
