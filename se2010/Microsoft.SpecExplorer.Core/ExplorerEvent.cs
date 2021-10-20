using System;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	internal abstract class ExplorerEvent
	{
		public ExplorerEventType Type { get; protected set; }

		protected ExplorerEvent(ExplorerEventType type)
		{
			Type = type;
		}
	}
}
