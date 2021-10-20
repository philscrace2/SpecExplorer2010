using System;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer.Viewer
{
	public sealed class CompareStateEventArgs : EventArgs
	{
		public State Left { get; private set; }

		public State Right { get; private set; }

		public string CompareLabel { get; private set; }

		public CompareStateEventArgs(State left, State right)
		{
			Left = left;
			Right = right;
			CompareLabel = string.Format("{0} : {1}", left.Label, right.Label);
		}
	}
}
