// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.DisplayGraphBuilder
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer.Viewer
{
  internal class DisplayGraphBuilder
  {
    private DisplayGraph displayGraph;
    private IViewQuery selectQuery;
    private HashSet<string> selectLabel;
    private List<IViewQuery> groupQueries;
    private IViewQuery descriptionQuery;
    private IViewQuery hideQuery;
    private Dictionary<string, DisplayNode> labelToNodeDict;
    private Dictionary<string, DisplayNode> equivalentDict;
    private List<DisplayNode> topNodes;
    private bool isDisplayRequirements;

    internal TransitionSystem TransitionSystem { get; set; }

    internal DisplayGraphBuilder(TransitionSystem transitionSystem)
    {
      this.TransitionSystem = transitionSystem;
      this.selectLabel = new HashSet<string>();
      this.groupQueries = new List<IViewQuery>();
      this.labelToNodeDict = new Dictionary<string, DisplayNode>();
      this.equivalentDict = new Dictionary<string, DisplayNode>();
      this.topNodes = new List<DisplayNode>();
    }

    private void Reset()
    {
      this.selectQuery = (IViewQuery) null;
      this.selectLabel.Clear();
      this.groupQueries.Clear();
      this.labelToNodeDict.Clear();
      this.equivalentDict.Clear();
      this.topNodes.Clear();
    }

    private void SetViewDefinition(ViewDefinition viewDefinition)
    {
      this.selectQuery = QueryFactory.GetViewQuery(viewDefinition.SelectQuery.Query);
      this.selectLabel.Clear();
      this.selectLabel.Add(true.ToString());
      this.groupQueries.Clear();
      foreach (Query query in viewDefinition.GroupQuery)
      {
        if (!string.IsNullOrEmpty(query.Param))
          this.groupQueries.Add(QueryFactory.GetViewQuery(query));
      }
      this.descriptionQuery = QueryFactory.GetViewQuery(viewDefinition.StateDescription);
      this.hideQuery = QueryFactory.GetViewQuery(viewDefinition.HideQuery.Query);
      this.isDisplayRequirements = viewDefinition.DisplayRequirements;
    }

    private void ValidateViewDefinition()
    {
      string errorMessage = "";
      if (this.selectQuery != null && !this.selectQuery.ValidateViewQuery(this.TransitionSystem, out errorMessage))
        throw new QueryException(errorMessage);
      foreach (IViewQuery groupQuery in this.groupQueries)
      {
        if (!groupQuery.ValidateViewQuery(this.TransitionSystem, out errorMessage))
          throw new QueryException(errorMessage);
      }
      if (this.descriptionQuery != null && !this.descriptionQuery.ValidateViewQuery(this.TransitionSystem, out errorMessage))
        throw new QueryException(errorMessage);
      if (this.hideQuery != null && !this.hideQuery.ValidateViewQuery(this.TransitionSystem, out errorMessage))
        throw new QueryException(errorMessage);
    }

    internal DisplayGraph BuildDisplayGraph(ViewDefinition viewDefinition)
    {
      this.Reset();
      this.displayGraph = new DisplayGraph();
      this.displayGraph.NodeFillColor = viewDefinition.NodeFillColor;
      this.displayGraph.EdgeColor = viewDefinition.EdgeColor;
      this.SetViewDefinition(viewDefinition);
      this.ValidateViewDefinition();
      this.BuildAndAddNodes();
      this.BuildAndAddEdges(viewDefinition.ShowParameters);
      if (viewDefinition.ShowErrorPathsOnly)
        this.ShowErrorPathsOnly();
      if (viewDefinition.ViewCollapseSteps)
        this.CollapseSteps();
      if (this.hideQuery != null)
        this.ProcessHiddenQuery();
      if (this.groupQueries.Count > 0 && this.groupQueries[0] != null)
        this.ProcessGroupQuery();
      if (viewDefinition.ViewCollapseLabels)
        this.CollapseLabels();
      return this.displayGraph;
    }

    private void ProcessRepresentativeStates()
    {
      foreach (State state in this.TransitionSystem.States)
      {
        if (!string.IsNullOrEmpty(state.RepresentativeState))
        {
          if (state.RelationKind == StateRelationKind.Equivalent)
          {
            DisplayNode displayNode = this.labelToNodeDict[state.RepresentativeState];
            this.equivalentDict[state.Label] = displayNode;
            this.displayGraph.AddEquivalentState(displayNode, state);
          }
          else if (state.RelationKind == StateRelationKind.None)
            this.displayGraph.AddEdge(new DisplayEdge(this.labelToNodeDict[state.Label], this.labelToNodeDict[state.RepresentativeState], DisplayEdgeKind.Subsume));
        }
      }
    }

    private void BuildAndAddNodes()
    {
      foreach (State state in this.TransitionSystem.States)
      {
        DisplayNode node = new DisplayNode(DisplayNodeKind.Normal, state, state.Label, ((IEnumerable<string>) this.TransitionSystem.InitialStates).Contains<string>(state.Label), state.Flags, state.Flags.ToNodeKind());
        this.labelToNodeDict[state.Label] = node;
        if (this.descriptionQuery != null)
        {
          string label = this.descriptionQuery.GetLabel(node.Label);
          node.Text = label ?? "<<Exception>>";
        }
        if (string.IsNullOrEmpty(state.RepresentativeState) || state.RelationKind != StateRelationKind.Equivalent)
        {
          this.displayGraph.AddNode(node, node.IsStart);
          this.topNodes.Add(node);
        }
      }
      this.ProcessRepresentativeStates();
    }

    private void BuildAndAddEdges(bool showParameters)
    {
      foreach (Transition transition in this.TransitionSystem.Transitions)
      {
        DisplayNode source;
        if (this.labelToNodeDict.TryGetValue(transition.Source, out source))
        {
          DisplayNode target;
          if (this.equivalentDict.TryGetValue(transition.Target, out target))
          {
            if (!this.labelToNodeDict.TryGetValue(target.Label.Label, out target))
              continue;
          }
          else if (!this.labelToNodeDict.TryGetValue(transition.Target, out target))
            continue;
          DisplayEdge edge = new DisplayEdge(source, target, transition, this.isDisplayRequirements);
          if (!showParameters)
          {
            switch (edge.Kind - 1)
            {
              case 0:
              case ActionSymbolKind.Throw:
                edge.Text = edge.Text.Remove(edge.Text.IndexOf("("));
                break;
              case ActionSymbolKind.Call:
                if (edge.Text.Contains<char>('/'))
                {
                  edge.Text = edge.Text.Remove(edge.Text.IndexOf("/"));
                  break;
                }
                break;
            }
          }
          this.displayGraph.AddEdge(edge);
        }
      }
    }

    private void ShowErrorPathsOnly()
    {
      HashSet<Node<State>> nodeSet = new HashSet<Node<State>>();
      foreach (DisplayNode node in this.displayGraph.Nodes)
      {
        if ((node.StateFlags & ObjectModel.StateFlags.Error) != null)
          nodeSet.Add((Node<State>) node);
      }
      HashSet<Edge<State, Transition>> edgeSet = new HashSet<Edge<State, Transition>>();
      foreach (DisplayNode startNode in this.displayGraph.StartNodes)
      {
        if (nodeSet.Count != 0)
        {
          ShortestPathAlgorithm<State, Transition> shortestPathAlgorithm = new ShortestPathAlgorithm<State, Transition>((IGraph<State, Transition>) this.displayGraph, (Node<State>) startNode, (IEnumerable<Node<State>>) nodeSet);
          shortestPathAlgorithm.Run();
          Dictionary<Node<State>, Path<State, Transition>> resultDict = shortestPathAlgorithm.ResultDict;
          foreach (Node<State> key in resultDict.Keys)
          {
            foreach (Edge<State, Transition> edge in resultDict[key].Edges)
              edgeSet.Add(edge);
            nodeSet.Remove(key);
          }
        }
        else
          break;
      }
      foreach (Edge<State, Transition> edge in this.displayGraph.Edges.ToArray<Edge<State, Transition>>())
      {
        if (!edgeSet.Contains(edge))
          this.displayGraph.DeleteEdge(edge);
      }
      foreach (Node<State> node in this.displayGraph.Nodes.ToArray<Node<State>>())
      {
        if (this.displayGraph.IncomingCount(node) == 0 && this.displayGraph.OutgoingCount(node) == 0)
          this.displayGraph.DeleteNode(node);
      }
    }

    private void ProcessHiddenQuery()
    {
      Dictionary<Node<State>, HashSet<Node<State>>> dictionary = new Dictionary<Node<State>, HashSet<Node<State>>>();
      foreach (DisplayNode displayNode in this.displayGraph.Nodes.ToArray<Node<State>>())
      {
        if ((displayNode.StateFlags & ObjectModel.StateFlags.Error) == null && string.Compare("true", this.hideQuery.GetLabel(displayNode.Label), true) == 0)
        {
          List<Edge<State, Transition>> edges;
          if (!this.displayGraph.TryGetInComingEdges((Node<State>) displayNode, out edges))
            edges = new List<Edge<State, Transition>>();
          List<Edge<State, Transition>> outEdges;
          if (!this.displayGraph.TryGetOutGoingEdges((Node<State>) displayNode, out outEdges))
            outEdges = new List<Edge<State, Transition>>();
          foreach (Edge<State, Transition> edge1 in edges)
          {
            if (edge1.Source != edge1.Target)
            {
              Node<State> source = edge1.Source;
              HashSet<Node<State>> nodeSet;
              if (!dictionary.TryGetValue(source, out nodeSet))
              {
                nodeSet = new HashSet<Node<State>>();
                dictionary[source] = nodeSet;
              }
              foreach (Edge<State, Transition> edge2 in outEdges)
              {
                if (edge2.Source != edge2.Target)
                {
                  Node<State> target = edge2.Target;
                  if (nodeSet.Add(target))
                    this.displayGraph.AddEdge(new DisplayEdge(source as DisplayNode, target as DisplayNode, DisplayEdgeKind.Hidden));
                }
              }
            }
          }
          this.displayGraph.DeleteNode((Node<State>) displayNode);
          this.topNodes.Remove(displayNode);
        }
      }
    }

    private void CollapseSteps()
    {
      foreach (Node<State> node in this.displayGraph.ChoiceNodes.ToArray<Node<State>>())
      {
        List<Edge<State, Transition>> outEdges;
        if ((node.Label.Flags & StateFlags.BoundStopped) == null && this.displayGraph.TryGetOutGoingEdges(node, out outEdges) && outEdges.Count == 1)
        {
          DisplayEdge outEdge = outEdges[0] as DisplayEdge;
          List<Edge<State, Transition>> edges;
          if (outEdge.Kind == ActionSymbolKind.Return && outEdge.Source != outEdge.Target && this.displayGraph.TryGetInComingEdges(node, out edges))
          {
            bool flag = true;
            List<DisplayEdge> displayEdgeList = new List<DisplayEdge>();
            foreach (Edge<State, Transition> edge in edges)
            {
              DisplayEdge inEdge = edge as DisplayEdge;
              if (inEdge.Kind != ActionSymbolKind.Call)
              {
                flag = false;
                break;
              }
              displayEdgeList.Add(new DisplayEdge(inEdge, outEdge, this.isDisplayRequirements));
              (outEdge.Target as DisplayNode).CollapsedNode = node as DisplayNode;
            }
            if (flag)
            {
              foreach (DisplayEdge edge in displayEdgeList)
                this.displayGraph.AddEdge(edge);
              this.displayGraph.DeleteNode(node);
              this.topNodes.Remove(node as DisplayNode);
            }
          }
        }
      }
    }

    private void CollapseLabels()
    {
      Dictionary<Node<State>, List<Edge<State, Transition>>> dictionary = new Dictionary<Node<State>, List<Edge<State, Transition>>>();
      foreach (Node<State> node in this.displayGraph.Nodes.ToArray<Node<State>>())
      {
        dictionary.Clear();
        List<Edge<State, Transition>> outEdges;
        if (this.displayGraph.TryGetOutGoingEdges(node, out outEdges))
        {
          foreach (Edge<State, Transition> edge in outEdges)
          {
            List<Edge<State, Transition>> edgeList;
            if (!dictionary.TryGetValue(edge.Target, out edgeList))
            {
              edgeList = new List<Edge<State, Transition>>();
              dictionary[edge.Target] = edgeList;
            }
            edgeList.Add(edge);
          }
          foreach (Node<State> key in dictionary.Keys)
          {
            List<Edge<State, Transition>> edgeList = dictionary[key];
            if (edgeList.Count > 1)
            {
              DisplayEdge edge1 = new DisplayEdge(node as DisplayNode, key as DisplayNode, DisplayEdgeKind.Hyper);
              foreach (Edge<State, Transition> edge2 in edgeList)
              {
                this.displayGraph.DeleteEdge(edge2);
                edge1.AddSubEdge(edge2 as DisplayEdge);
              }
              this.displayGraph.AddEdge(edge1);
            }
          }
        }
      }
    }

    private void ProcessSelectQuery()
    {
      if (this.selectQuery == null)
        return;
      IEnumerable<DisplayNode> hyperNodes = this.selectQuery.GetHyperNodes((ICollection<DisplayNode>) this.topNodes);
      this.topNodes.Clear();
      foreach (DisplayNode displayNode in hyperNodes)
      {
        if (this.selectLabel.Contains(displayNode.Label.Label))
        {
          foreach (DisplayNode subNode in displayNode.SubNodes)
          {
            subNode.ResetParent();
            this.topNodes.Add(subNode);
            this.labelToNodeDict[subNode.Label.Label] = subNode;
          }
        }
      }
    }

    private void ProcessGroupQuery()
    {
      List<DisplayNode> displayNodeList1 = new List<DisplayNode>();
      displayNodeList1.AddRange(this.groupQueries[0].GetHyperNodes((ICollection<DisplayNode>) this.topNodes));
      foreach (DisplayNode parentNode in displayNodeList1)
        this.displayGraph.CollapseNode(parentNode);
      List<DisplayNode> displayNodeList2 = new List<DisplayNode>();
      bool flag = true;
      foreach (IViewQuery groupQuery in this.groupQueries)
      {
        if (flag)
        {
          flag = false;
        }
        else
        {
          foreach (DisplayNode parentNode in displayNodeList1)
          {
            groupQuery.DivideHyperNodes(parentNode);
            displayNodeList2.AddRange((IEnumerable<DisplayNode>) parentNode.SubNodes);
          }
          List<DisplayNode> displayNodeList3 = displayNodeList1;
          displayNodeList1 = displayNodeList2;
          displayNodeList2 = displayNodeList3;
          displayNodeList2.Clear();
        }
      }
    }
  }
}
