namespace Microsoft.SpecExplorer
{
	internal class ShowTestCaseFinishedProgressEventObserver : EventObserver
	{
		private IExplorerUpdateUI explorer;

		internal ShowTestCaseFinishedProgressEventObserver(IExplorerUpdateUI explorerImpl)
		{
			explorer = explorerImpl;
		}

		internal override bool HandleEvent(ExplorerEvent e)
		{
			if (ExplorerEventType.ShowTestCaseFinishedProgress != e.Type)
			{
				return false;
			}
			ShowTestCaseFinishedProgress showTestCaseFinishedProgress = (ShowTestCaseFinishedProgress)e;
			explorer.ShowTestCaseFinishedProgress(showTestCaseFinishedProgress.Progress);
			return true;
		}
	}
}
