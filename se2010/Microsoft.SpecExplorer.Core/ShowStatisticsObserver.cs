namespace Microsoft.SpecExplorer
{
	internal class ShowStatisticsObserver : EventObserver
	{
		private IExplorerUpdateUI explorer;

		internal ShowStatisticsObserver(IExplorerUpdateUI explorerImpl)
		{
			explorer = explorerImpl;
		}

		internal override bool HandleEvent(ExplorerEvent e)
		{
			switch (e.Type)
			{
			case ExplorerEventType.ShowExplorationStatistics:
			{
				ShowExplorationStatistics showExplorationStatistics = (ShowExplorationStatistics)e;
				explorer.ShowStatistics(showExplorationStatistics.Statistics);
				return true;
			}
			case ExplorerEventType.ShowTestingStatistics:
			{
				ShowTestingStatistics showTestingStatistics = (ShowTestingStatistics)e;
				explorer.ShowStatistics(showTestingStatistics.Statistics);
				return true;
			}
			default:
				return false;
			}
		}
	}
}
