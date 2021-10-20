using System;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	internal class RecoverFromFatalErrorEvent : ExplorerEvent
	{
		public Exception Exception { get; private set; }

		public RecoverFromFatalErrorEvent(Exception exception)
			: base(ExplorerEventType.RecoverFromFatalError)
		{
			Exception = exception;
		}
	}
}
