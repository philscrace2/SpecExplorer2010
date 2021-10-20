namespace Microsoft.SpecExplorer
{
	internal class SwitchStateEventObserver : EventObserver
	{
		private IExplorerUpdateUI explorer;

		internal SwitchStateEventObserver(IExplorerUpdateUI explorerImpl)
		{
			explorer = explorerImpl;
		}

		internal override bool HandleEvent(ExplorerEvent e)
		{
			if (e.Type != 0)
			{
				return false;
			}
			explorer.SwitchState(((SwitchStateEvent)e).State);
			return true;
		}
	}
}
