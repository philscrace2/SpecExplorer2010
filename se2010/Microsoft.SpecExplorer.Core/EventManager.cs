using System.Collections.Generic;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	public class EventManager : DisposableMarshalByRefObject
	{
		private List<EventObserver> registeredObservers = new List<EventObserver>();

		internal void HandleEvent(ExplorerEvent e)
		{
			foreach (EventObserver registeredObserver in registeredObservers)
			{
				registeredObserver.HandleEvent(e);
			}
		}

		internal void RegisterEventObserver(EventObserver eventObserver)
		{
			registeredObservers.Add(eventObserver);
		}
	}
}
