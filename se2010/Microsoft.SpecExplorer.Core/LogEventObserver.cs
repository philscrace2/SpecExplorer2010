namespace Microsoft.SpecExplorer
{
	internal class LogEventObserver : EventObserver
	{
		private IHost host;

		internal LogEventObserver(IHost host)
		{
			this.host = host;
		}

		internal override bool HandleEvent(ExplorerEvent e)
		{
			if (ExplorerEventType.Log != e.Type)
			{
				return false;
			}
			host.Log(((LogEvent)e).Message);
			return true;
		}
	}
}
