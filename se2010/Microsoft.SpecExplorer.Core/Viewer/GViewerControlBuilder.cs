using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.GraphTraversal;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer.Viewer
{
	internal class GViewerControlBuilder : IDisposable
	{
		private DisplayGraph displayGraph;

		private Graph drawingGraph;

		private Dictionary<DisplayNode, Microsoft.Msagl.Drawing.Node> displayNodeToDrawNodeDict;

		private Color nodeFillColor;

		private Color edgeColor;

		private bool disposed;

		internal GViewerControlBuilder()
		{
			displayNodeToDrawNodeDict = new Dictionary<DisplayNode, Microsoft.Msagl.Drawing.Node>();
		}

		internal GViewer BuildGViewerControl(int timeout)
		{
			GViewer viewer = new GViewer();
			viewer.ToolBarIsVisible = false;
			viewer.ZoomWhenMouseWheelScroll = false;
			viewer.AsyncLayout = timeout > 0;
			if (viewer.AsyncLayout)
			{
				Timer timer = new Timer(delegate
				{
					viewer.AbortAsyncLayout();
				});
				timer.Change(1000 * timeout, -1);
			}
			viewer.WindowZoomButtonPressed = true;
			return viewer;
		}

		internal Graph BuildControlGraph(DisplayGraph displayGraph)
		{
			displayNodeToDrawNodeDict.Clear();
			this.displayGraph = displayGraph;
			drawingGraph = new Graph("label");
			nodeFillColor = new Color(displayGraph.NodeFillColor.R, displayGraph.NodeFillColor.G, displayGraph.NodeFillColor.B);
			edgeColor = new Color(displayGraph.EdgeColor.R, displayGraph.EdgeColor.G, displayGraph.EdgeColor.B);
			BuildDrawingGraph();
			return drawingGraph;
		}

		private Graph BuildDrawingGraph()
		{
			foreach (DisplayNode node in displayGraph.Nodes)
			{
				AddNode(node);
			}
			foreach (Edge<State, Transition> edge2 in displayGraph.Edges)
			{
				DisplayEdge displayEdge = edge2 as DisplayEdge;
				Microsoft.Msagl.Drawing.Node value = null;
				if (!displayNodeToDrawNodeDict.TryGetValue(edge2.Source as DisplayNode, out value))
				{
					throw new InvalidOperationException("Source node of edge are not added.");
				}
				Microsoft.Msagl.Drawing.Node value2 = null;
				if (!displayNodeToDrawNodeDict.TryGetValue(edge2.Target as DisplayNode, out value2))
				{
					throw new InvalidOperationException("Target node of edge are not added.");
				}
				Edge edge = drawingGraph.AddEdge(value.Id, displayEdge.Text, value2.Id);
				edge.UserData = displayEdge.Id;
				edge.Attr.ArrowheadAtSource = ArrowStyle.None;
				edge.Attr.ArrowheadAtTarget = ArrowStyle.Normal;
				switch (displayEdge.displayEdgeKind)
				{
				case DisplayEdgeKind.Normal:
				case DisplayEdgeKind.Hyper:
				case DisplayEdgeKind.Collapsed:
					edge.Attr.AddStyle(Style.Filled);
					break;
				case DisplayEdgeKind.Hidden:
					edge.Attr.AddStyle(Style.Dashed);
					break;
				case DisplayEdgeKind.Subsume:
					edge.Attr.AddStyle(Style.Dotted);
					break;
				}
				edge.Attr.Color = edgeColor;
				if (edge.Label != null)
				{
					edge.Label.FontColor = edgeColor;
				}
			}
			return drawingGraph;
		}

		private void AddNode(DisplayNode node)
		{
			Microsoft.Msagl.Drawing.Node node2 = drawingGraph.AddNode(node.Id);
			node2.LabelText = node.Text;
			node2.Attr.FillColor = nodeFillColor;
			node.DrawingNode = node2;
			displayNodeToDrawNodeDict[node] = node2;
			if (node.IsStart)
			{
				node2.Attr.FillColor = Color.LightGray;
			}
			if ((node.StateFlags & StateFlags.BoundStopped) != 0)
			{
				node2.Attr.FillColor = Color.Orange;
			}
			if ((node.StateFlags & StateFlags.Error) > StateFlags.None)
			{
				node2.Attr.FillColor = Color.Red;
			}
			if (node.Kind == Microsoft.GraphTraversal.NodeKind.Accepting)
			{
				node2.Attr.Color = Color.Green;
				node2.Attr.LineWidth = 2;
				node2.Attr.LabelMargin = -1;
			}
			if (node.DisplayNodeKind == DisplayNodeKind.Hyper)
			{
				node2.Attr.Color = Color.DarkRed;
				node2.Attr.Padding = 10.0;
				node2.Attr.LabelMargin = -1;
			}
			if (displayGraph.ChoiceNodes.Contains(node))
			{
				node2.Attr.Shape = Shape.Diamond;
			}
			if ((node.StateFlags & StateFlags.NonAcceptingEnd) > StateFlags.None)
			{
				node2.Attr.Color = Color.Red;
				node2.Attr.LineWidth = 2;
				node2.Attr.LabelMargin = -1;
			}
		}

		public void Dispose()
		{
			if (!disposed)
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
			}
		}
	}
}
