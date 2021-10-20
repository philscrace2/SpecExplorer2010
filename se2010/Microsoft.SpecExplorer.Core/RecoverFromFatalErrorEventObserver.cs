namespace Microsoft.SpecExplorer
{
	internal class RecoverFromFatalErrorEventObserver : EventObserver
	{
		private IHost host;

		internal RecoverFromFatalErrorEventObserver(IHost host)
		{
			this.host = host;
		}

		internal override bool HandleEvent(ExplorerEvent e)
		{
			if (ExplorerEventType.RecoverFromFatalError != e.Type)
			{
				return false;
			}
			host.RecoverFromFatalError(((RecoverFromFatalErrorEvent)e).Exception);
			return true;
		}
	}
}
