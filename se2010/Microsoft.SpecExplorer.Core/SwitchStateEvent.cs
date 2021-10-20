using System;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	internal class SwitchStateEvent : ExplorerEvent
	{
		private ExplorationState state;

		public ExplorationState State
		{
			get
			{
				return state;
			}
		}

		public SwitchStateEvent(ExplorationState state)
			: base(ExplorerEventType.SwitchState)
		{
			this.state = state;
		}
	}
}
