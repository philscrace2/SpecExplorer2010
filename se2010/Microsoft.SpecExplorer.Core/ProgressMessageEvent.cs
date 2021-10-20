using System;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	internal class ProgressMessageEvent : ExplorerEvent
	{
		public VerbosityLevel Verbosity { get; private set; }

		public string Message { get; private set; }

		public ProgressMessageEvent(VerbosityLevel verbosity, string message)
			: base(ExplorerEventType.ProgressMessage)
		{
			Verbosity = verbosity;
			Message = message;
		}
	}
}
