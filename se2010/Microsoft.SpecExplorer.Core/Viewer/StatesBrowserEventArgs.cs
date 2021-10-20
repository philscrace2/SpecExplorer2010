using System;
using System.Collections.Generic;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer.Viewer
{
	public sealed class StatesBrowserEventArgs : EventArgs
	{
		public IEnumerable<State> States { get; private set; }

		public bool ShouldDisplayLeftTree { get; private set; }

		public string StateLabel { get; private set; }

		public StatesBrowserEventArgs(DisplayNode node)
		{
			List<State> list = (List<State>)(States = new List<State>());
			switch (node.DisplayNodeKind)
			{
			case DisplayNodeKind.Hyper:
				list.AddRange(node.LeafNodeStates);
				break;
			case DisplayNodeKind.Normal:
				list.Add(node.Label);
				break;
			}
			ShouldDisplayLeftTree = node.DisplayNodeKind == DisplayNodeKind.Hyper;
			StateLabel = node.Text;
		}
	}
}
