using System.Collections.Generic;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;

namespace Microsoft.SpecExplorer.Viewer
{
	internal class FindStateSettings
	{
		public int CurrentNodeIndex { get; set; }

		public GViewer CurrentGViewer { get; set; }

		public Dictionary<Node, NodeAttr> CurrentHighlightedNodes { get; set; }

		public List<DisplayNode> OrderedDisplayNode { get; set; }

		public int FindScopeIndex { get; set; }

		public string SearchString { get; set; }

		public bool MatchCaseChecked { get; set; }

		public bool MatchWholeWordChecked { get; set; }

		public bool SearchUpChecked { get; set; }

		public bool UseOptionChecked { get; set; }

		internal FindStateSettings(GViewer viewer)
		{
			CurrentNodeIndex = -1;
			CurrentGViewer = viewer;
			SearchString = string.Empty;
			CurrentHighlightedNodes = new Dictionary<Node, NodeAttr>();
		}
	}
}
