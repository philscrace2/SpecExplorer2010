using System;
using Microsoft.ActionMachines;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	internal class ShowTestingStatistics : ExplorerEvent
	{
		public TestingStatistics Statistics { get; private set; }

		public ShowTestingStatistics(TestingStatistics statistics)
			: base(ExplorerEventType.ShowTestingStatistics)
		{
			if (statistics == null)
			{
				throw new ArgumentNullException("statistics");
			}
			Statistics = statistics;
		}
	}
}
