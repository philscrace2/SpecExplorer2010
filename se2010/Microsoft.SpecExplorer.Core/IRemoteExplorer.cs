using System;
using System.Collections.Generic;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	public interface IRemoteExplorer : IDisposable
	{
		ExplorationResult ExplorationResult { get; }

		IEnumerable<string> TempAssemblyFiles { get; }

		ExplorationState State { get; set; }

		object AbortLock { get; }

		void Configure(ExplorerConfiguration explorerConfig, EventManager eventManager, ExplorerMediator explorerMediator, bool isRemoteAppDomain);

		void StartBuild();

		void StartExploration();

		void SuspendExploration();

		void Abort();
	}
}
