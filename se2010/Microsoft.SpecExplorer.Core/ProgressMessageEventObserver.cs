namespace Microsoft.SpecExplorer
{
	internal class ProgressMessageEventObserver : EventObserver
	{
		private IHost host;

		internal ProgressMessageEventObserver(IHost host)
		{
			this.host = host;
		}

		internal override bool HandleEvent(ExplorerEvent e)
		{
			if (ExplorerEventType.ProgressMessage != e.Type)
			{
				return false;
			}
			ProgressMessageEvent progressMessageEvent = (ProgressMessageEvent)e;
			host.ProgressMessage(progressMessageEvent.Verbosity, progressMessageEvent.Message);
			return true;
		}
	}
}
