namespace Microsoft.SpecExplorer
{
	internal abstract class EventObserver
	{
		internal abstract bool HandleEvent(ExplorerEvent e);
	}
}
