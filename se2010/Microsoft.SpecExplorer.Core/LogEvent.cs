using System;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	internal class LogEvent : ExplorerEvent
	{
		public string Message { get; private set; }

		public LogEvent(string message)
			: base(ExplorerEventType.Log)
		{
			Message = message;
		}
	}
}
