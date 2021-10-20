namespace Microsoft.SpecExplorer
{
	internal class DiagMessageEventObserver : EventObserver
	{
		private IHost host;

		internal DiagMessageEventObserver(IHost host)
		{
			this.host = host;
		}

		internal override bool HandleEvent(ExplorerEvent e)
		{
			if (ExplorerEventType.DiagMessage != e.Type)
			{
				return false;
			}
			DiagMessageEvent diagMessageEvent = (DiagMessageEvent)e;
			host.DiagMessage(diagMessageEvent.Kind, diagMessageEvent.Message, diagMessageEvent.Location);
			return true;
		}
	}
}
