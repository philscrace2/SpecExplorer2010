using System;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	internal class DiagMessageEvent : ExplorerEvent
	{
		public DiagnosisKind Kind { get; private set; }

		public string Message { get; private set; }

		public object Location { get; private set; }

		public DiagMessageEvent(DiagnosisKind kind, string message, object location)
			: base(ExplorerEventType.DiagMessage)
		{
			Kind = kind;
			Message = message;
			Location = location;
		}
	}
}
