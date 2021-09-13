// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.DisplayGraph
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.SpecExplorer.Viewer
{
  internal class DisplayGraph : Graph<State, Transition>
  {
    internal const string ExceptionNodeLabel = "<<Exception>>";
    internal const string ErrorNodeLabel = "<<Error>>";
    private Dictionary<string, DisplayNode> nodeIdDict;
    private Dictionary<string, DisplayEdge> edgeIdDict;
    private Dictionary<DisplayNode, State> equivalentStateDict;
    private int nodeId;
    private int edgeId;

    internal Color NodeFillColor { get; set; }

    internal Color EdgeColor { get; set; }

    internal DisplayGraph()
    {
      this.nodeIdDict = new Dictionary<string, DisplayNode>();
      this.edgeIdDict = new Dictionary<string, DisplayEdge>();
      this.equivalentStateDict = new Dictionary<DisplayNode, State>();
    }

    internal void CollapseNode(DisplayNode parentNode)
    {
      if (this.ContainsNode((Node<State>) parentNode))
        throw new InvalidOperationException("Can not collapse node to existing node");
      if (parentNode.DisplayNodeKind != DisplayNodeKind.Hyper)
        throw new InvalidOperationException("Can not collapse non-Hyper node");
      this.AddNode(parentNode, parentNode.IsStart);
      HashSet<Node<State>> nodeSet1 = new HashSet<Node<State>>();
      HashSet<Node<State>> nodeSet2 = new HashSet<Node<State>>();
      nodeSet2.Add((Node<State>) parentNode);
      foreach (DisplayNode subNode in parentNode.SubNodes)
      {
        if (!this.ContainsNode((Node<State>) subNode))
          throw new InvalidOperationException("Can not collapse non-existing node");
        List<Edge<State, Transition>> edges;
        if (this.TryGetInComingEdges((Node<State>) subNode, out edges))
        {
          foreach (Edge<State, Transition> edge1 in edges.ToArray())
          {
            this.DeleteEdge(edge1);
            DisplayEdge edge2 = new DisplayEdge(edge1.Source as DisplayNode, parentNode, edge1 as DisplayEdge);
            if (edge2.displayEdgeKind != DisplayEdgeKind.Hidden || nodeSet1.Add(edge1.Source))
              this.AddEdge(edge2);
          }
        }
        List<Edge<State, Transition>> outEdges;
        if (this.TryGetOutGoingEdges((Node<State>) subNode, out outEdges))
        {
          foreach (Edge<State, Transition> edge1 in outEdges.ToArray())
          {
            this.DeleteEdge(edge1);
            DisplayEdge edge2 = new DisplayEdge(parentNode, edge1.Target as DisplayNode, edge1 as DisplayEdge);
            if (edge2.displayEdgeKind != DisplayEdgeKind.Hidden || nodeSet2.Add(edge1.Target))
              this.AddEdge(edge2);
          }
        }
        this.DeleteNode((Node<State>) subNode);
      }
      if (parentNode.SubNodes.Count != 0)
        return;
      this.DeleteNode((Node<State>) parentNode);
    }

    internal DisplayNode GetNodeById(string id)
    {
      DisplayNode displayNode;
      if (!this.nodeIdDict.TryGetValue(id, out displayNode))
        throw new InvalidOperationException(string.Format("The display graph does not contains node: '{0}'", (object) id));
      return displayNode;
    }

    internal DisplayEdge GetEdgeById(string id)
    {
      DisplayEdge displayEdge;
      if (!this.edgeIdDict.TryGetValue(id, out displayEdge))
        throw new InvalidOperationException(string.Format("The display graph does not contains node: '{0}'", (object) id));
      return displayEdge;
    }

    internal void AddNode(DisplayNode node, bool isStart)
    {
      this.AddNode((Node<State>) node, isStart);
      if (node.Id == null)
      {
        node.Id = this.nodeId.ToString();
        ++this.nodeId;
      }
      this.nodeIdDict[node.Id] = node;
    }

    internal void AddEdge(DisplayEdge edge)
    {
      this.AddEdge((Edge<State, Transition>) edge);
      if (edge.Id == null)
      {
        edge.Id = this.edgeId.ToString();
        ++this.edgeId;
      }
      this.edgeIdDict[edge.Id] = edge;
    }

    internal void AddEquivalentState(DisplayNode displayNode, State state) => this.equivalentStateDict[displayNode] = state;

    internal State GetEquivalentState(DisplayNode displayNode)
    {
      State state;
      return this.equivalentStateDict.TryGetValue(displayNode, out state) ? state : (State) null;
    }
  }
}
