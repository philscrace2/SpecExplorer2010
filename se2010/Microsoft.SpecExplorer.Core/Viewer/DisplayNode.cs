using System;
using System.Collections.Generic;
using Microsoft.GraphTraversal;
using Microsoft.Msagl.Drawing;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer.Viewer
{
	public class DisplayNode : Node<State>
	{
		internal List<DisplayNode> SubNodes { get; set; }

		internal List<State> LeafNodeStates { get; set; }

		internal DisplayNode ParentNode { get; set; }

		internal DisplayNodeKind DisplayNodeKind { get; set; }

		internal string Text { get; set; }

		internal StateFlags StateFlags { get; set; }

		internal bool IsStart { get; set; }

		internal DisplayNode CollapsedNode { get; set; }

		internal string Id { get; set; }

		internal Microsoft.Msagl.Drawing.Node DrawingNode { get; set; }

		internal DisplayNode(DisplayNodeKind displayNodeKind, State state, string text, bool isStart, StateFlags flags, Microsoft.GraphTraversal.NodeKind nodeKind)
			: base(state, nodeKind)
		{
			DisplayNodeKind = displayNodeKind;
			Text = text;
			IsStart = isStart;
			StateFlags = flags;
			if (DisplayNodeKind == DisplayNodeKind.Hyper)
			{
				SubNodes = new List<DisplayNode>();
				LeafNodeStates = new List<State>();
			}
		}

		internal void AddSubNode(DisplayNode node)
		{
			if (DisplayNodeKind != DisplayNodeKind.Hyper)
			{
				throw new InvalidOperationException("Can not add sub node to non hyper node");
			}
			SubNodes.Add(node);
			if (node.DisplayNodeKind == DisplayNodeKind.Hyper)
			{
				LeafNodeStates.AddRange(node.LeafNodeStates);
			}
			else
			{
				LeafNodeStates.Add(node.Label);
			}
			StateFlags |= node.StateFlags;
			IsStart = IsStart || node.IsStart;
		}

		internal void ResetParent()
		{
			ParentNode = null;
		}
	}
}
