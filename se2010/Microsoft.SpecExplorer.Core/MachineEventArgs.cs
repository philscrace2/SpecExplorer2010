using System;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	public sealed class MachineEventArgs : EventArgs
	{
		public IList<Machine> Machines { get; private set; }

		public bool ReExplore { get; set; }

		public IEnumerable<string> PostProcessors { get; private set; }

		public bool IsOnTheFlyReplay { get; private set; }

		public MachineEventArgs(IList<Machine> machines, bool reExplore = false, IEnumerable<string> postProcessors = null, bool isOnTheFlyReplay = false)
		{
			if (machines == null)
			{
				throw new ArgumentNullException("machines");
			}
			if (machines.Count == 0)
			{
				throw new ArgumentException("The number of machines should not be zero.");
			}
			Machines = machines;
			ReExplore = reExplore;
			PostProcessors = postProcessors;
			IsOnTheFlyReplay = isOnTheFlyReplay;
		}
	}
}
