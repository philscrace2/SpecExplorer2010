using System;
using Microsoft.ActionMachines;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	internal class ShowTestCaseFinishedProgress : ExplorerEvent
	{
		public TestCaseFinishedEventArgs Progress { get; private set; }

		public ShowTestCaseFinishedProgress(TestCaseFinishedEventArgs args)
			: base(ExplorerEventType.ShowTestCaseFinishedProgress)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			Progress = args;
		}
	}
}
