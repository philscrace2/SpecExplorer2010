// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.GViewerControlBuilder
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.GraphTraversal;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NodeKind = Microsoft.GraphTraversal.NodeKind;

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

    internal GViewerControlBuilder() => this.displayNodeToDrawNodeDict = new Dictionary<DisplayNode, Microsoft.Msagl.Drawing.Node>();

    internal GViewer BuildGViewerControl(int timeout)
    {
      GViewer viewer = new GViewer();
      viewer.ToolBarIsVisible = false;
      viewer.ZoomWhenMouseWheelScroll = false;
      viewer.AsyncLayout = timeout > 0;
      if (viewer.AsyncLayout)
        new Timer((TimerCallback) (state => viewer.AbortAsyncLayout())).Change(1000 * timeout, -1);
      viewer.WindowZoomButtonPressed = true;
      return viewer;
    }

    internal Graph BuildControlGraph(DisplayGraph displayGraph)
    {
      this.displayNodeToDrawNodeDict.Clear();
      this.displayGraph = displayGraph;
      this.drawingGraph = new Graph("label");
      this.nodeFillColor = new Color(displayGraph.NodeFillColor.R, displayGraph.NodeFillColor.G, displayGraph.NodeFillColor.B);
      this.edgeColor = new Color(displayGraph.EdgeColor.R, displayGraph.EdgeColor.G, displayGraph.EdgeColor.B);
      this.BuildDrawingGraph();
      return this.drawingGraph;
    }

    private Graph BuildDrawingGraph()
    {
      foreach (DisplayNode node in this.displayGraph.Nodes)
        this.AddNode(node);
      foreach (Edge<State, Transition> edge1 in this.displayGraph.Edges)
      {
        DisplayEdge displayEdge = edge1 as DisplayEdge;
        Microsoft.Msagl.Drawing.Node node1 = (Microsoft.Msagl.Drawing.Node) null;
        if (!this.displayNodeToDrawNodeDict.TryGetValue(edge1.Source as DisplayNode, out node1))
          throw new InvalidOperationException("Source node of edge are not added.");
        Microsoft.Msagl.Drawing.Node node2 = (Microsoft.Msagl.Drawing.Node) null;
        if (!this.displayNodeToDrawNodeDict.TryGetValue(edge1.Target as DisplayNode, out node2))
          throw new InvalidOperationException("Target node of edge are not added.");
        Edge edge2 = this.drawingGraph.AddEdge(node1.Id, displayEdge.Text, node2.Id);
        edge2.UserData = (object) displayEdge.Id;
        edge2.Attr.ArrowheadAtSource = ArrowStyle.None;
        edge2.Attr.ArrowheadAtTarget = ArrowStyle.Normal;
        switch (displayEdge.displayEdgeKind)
        {
          case DisplayEdgeKind.Normal:
          case DisplayEdgeKind.Hyper:
          case DisplayEdgeKind.Collapsed:
            edge2.Attr.AddStyle(Style.Filled);
            break;
          case DisplayEdgeKind.Hidden:
            edge2.Attr.AddStyle(Style.Dashed);
            break;
          case DisplayEdgeKind.Subsume:
            edge2.Attr.AddStyle(Style.Dotted);
            break;
        }
        edge2.Attr.Color = this.edgeColor;
        if (edge2.Label != null)
          edge2.Label.FontColor = this.edgeColor;
      }
      return this.drawingGraph;
    }

    private void AddNode(DisplayNode node)
    {
      Microsoft.Msagl.Drawing.Node node1 = this.drawingGraph.AddNode(node.Id);
      node1.LabelText = node.Text;
      node1.Attr.FillColor = this.nodeFillColor;
      node.DrawingNode = node1;
      this.displayNodeToDrawNodeDict[node] = node1;
      if (node.IsStart)
        node1.Attr.FillColor = Color.LightGray;
      if ((node.StateFlags & ObjectModel.StateFlags.BoundStopped) != null)
        node1.Attr.FillColor = Color.Orange;
      if ((node.StateFlags & ObjectModel.StateFlags.Error) > 0)
        node1.Attr.FillColor = Color.Red;
      if (node.Kind == NodeKind.Accepting)
      {
        node1.Attr.Color = Color.Green;
        node1.Attr.LineWidth = 2;
        node1.Attr.LabelMargin = -1;
      }
      if (node.DisplayNodeKind == DisplayNodeKind.Hyper)
      {
        node1.Attr.Color = Color.DarkRed;
        node1.Attr.Padding = 10.0;
        node1.Attr.LabelMargin = -1;
      }
      if (this.displayGraph.ChoiceNodes.Contains<Microsoft.GraphTraversal.Node<State>>((Microsoft.GraphTraversal.Node<State>) node))
        node1.Attr.Shape = Shape.Diamond;
      if ((node.StateFlags & ObjectModel.StateFlags.NonAcceptingEnd) <= 0)
        return;
      node1.Attr.Color = Color.Red;
      node1.Attr.LineWidth = 2;
      node1.Attr.LabelMargin = -1;
    }

    public void Dispose()
    {
      if (this.disposed)
        return;
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      int num = disposing ? 1 : 0;
      this.disposed = true;
    }
  }
}
