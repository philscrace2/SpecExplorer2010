namespace Microsoft.SpecExplorer
{
	internal class UpdateExplorationResultEventObserver : EventObserver
	{
		private IExplorerUpdateUI explorer;

		internal UpdateExplorationResultEventObserver(IExplorerUpdateUI explorerImpl)
		{
			explorer = explorerImpl;
		}

		internal override bool HandleEvent(ExplorerEvent e)
		{
			if (ExplorerEventType.UpdateExplorationResult != e.Type)
			{
				return false;
			}
			explorer.UpdateExplorationResult(((UpdateExplorationResultEvent)e).ExplorationResult);
			return true;
		}
	}
}
