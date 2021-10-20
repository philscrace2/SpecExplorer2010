using System;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer.Viewer
{
	public sealed class StepBrowserEventArgs : EventArgs
	{
		public IEnumerable<BrowserEdge> BrowserEdges { get; private set; }

		public string StepLabel { get; private set; }

		public StepBrowserEventArgs(IEnumerable<BrowserEdge> browserEdges, string stepLabel)
		{
			BrowserEdges = browserEdges;
			StepLabel = stepLabel;
		}
	}
}
