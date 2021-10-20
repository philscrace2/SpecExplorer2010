using System;

namespace Microsoft.SpecExplorer
{
	public class ExplorationStateChangedEventArgs : EventArgs
	{
		public ExplorationState OldState { get; private set; }

		public ExplorationState NewState { get; private set; }

		internal ExplorationStateChangedEventArgs(ExplorationState oldState, ExplorationState newState)
		{
			OldState = oldState;
			NewState = newState;
		}
	}
}
