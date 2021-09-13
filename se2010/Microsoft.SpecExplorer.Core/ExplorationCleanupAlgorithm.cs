// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ExplorationCleanupAlgorithm
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.GraphTraversal;
using Microsoft.Xrt;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer
{
  internal class ExplorationCleanupAlgorithm : IAlgorithm<MachineState, ExplorationStep>
  {
    private Dictionary<ICompressedState, HashSet<Node<MachineState>>> equivalentDataStates;
    private HashSet<ExplorationCleanupAlgorithm.Cousin> candidateCousins;
    private IGraph<MachineState, ExplorationStep> visitGraph;
    private object abortLock;
    private EventAdapter eventAdapter;

    public Dictionary<Node<MachineState>, Node<MachineState>> Cousins { get; private set; }

    public ExplorationCleanupAlgorithm(
      IGraph<MachineState, ExplorationStep> visitGraph,
      object abortLock,
      EventAdapter eventAdapter)
    {
      this.visitGraph = visitGraph;
      this.abortLock = abortLock;
      this.eventAdapter = eventAdapter;
    }

    public void Run()
    {
      this.PrepareCandidateCousins();
      while (this.candidateCousins.Count > 0)
      {
        ExplorationCleanupAlgorithm.Cousin cousin = this.candidateCousins.First<ExplorationCleanupAlgorithm.Cousin>();
        this.candidateCousins.Remove(cousin);
        if (!this.Cousins.ContainsKey(cousin.Key) && !this.Cousins.ContainsKey(cousin.Value) && this.IsMergeAble(cousin.Key, cousin.Value, true))
          this.Merge(cousin.Key, cousin.Value, true);
      }
      foreach (Node<MachineState> node in this.visitGraph.Nodes)
      {
        List<Edge<MachineState, ExplorationStep>> edges = new List<Edge<MachineState, ExplorationStep>>();
        List<Edge<MachineState, ExplorationStep>> outEdges;
        if (this.visitGraph.TryGetOutGoingEdges(node, out outEdges))
        {
          Dictionary<Action, HashSet<Node<MachineState>>> dictionary = new Dictionary<Action, HashSet<Node<MachineState>>>();
          foreach (Edge<MachineState, ExplorationStep> edge in outEdges)
          {
            HashSet<Node<MachineState>> nodeSet;
            if (!dictionary.TryGetValue(edge.Label.Action, out nodeSet))
            {
              nodeSet = new HashSet<Node<MachineState>>();
              dictionary[edge.Label.Action] = nodeSet;
            }
            if (!nodeSet.Add(edge.Target))
              edges.Add(edge);
          }
        }
        this.RemoveDuplicateEdges(edges);
      }
      this.eventAdapter.ShowStatistics(new ExplorationStatistics(ExplorationStatus.Cleanup, 0, this.visitGraph.Nodes.Count<Node<MachineState>>(), this.visitGraph.Edges.Count<Edge<MachineState, ExplorationStep>>(), 0, 0, 0, true));
    }

    public void Initialize()
    {
      this.equivalentDataStates = new Dictionary<ICompressedState, HashSet<Node<MachineState>>>();
      this.candidateCousins = new HashSet<ExplorationCleanupAlgorithm.Cousin>();
      this.Cousins = new Dictionary<Node<MachineState>, Node<MachineState>>();
    }

    private void PrepareCandidateCousins()
    {
      foreach (Node<MachineState> node1 in this.visitGraph.Nodes)
      {
        HashSet<Node<MachineState>> nodeSet = (HashSet<Node<MachineState>>) null;
        if (!this.equivalentDataStates.TryGetValue(node1.Label.Data, out nodeSet))
        {
          nodeSet = new HashSet<Node<MachineState>>();
          this.equivalentDataStates[node1.Label.Data] = nodeSet;
        }
        else
        {
          foreach (Node<MachineState> node2 in nodeSet)
            this.candidateCousins.Add(new ExplorationCleanupAlgorithm.Cousin(node1, node2));
        }
        nodeSet.Add(node1);
      }
    }

    private bool IsMergeAble(Node<MachineState> node1, Node<MachineState> node2, bool isForFuture)
    {
      if (node1.Label.Control.Kind != node2.Label.Control.Kind)
        return false;
      Dictionary<Action, HashSet<Node<MachineState>>> dictionary1 = new Dictionary<Action, HashSet<Node<MachineState>>>();
      List<Edge<MachineState, ExplorationStep>> edgeList = (List<Edge<MachineState, ExplorationStep>>) null;
      List<Edge<MachineState, ExplorationStep>> edges = new List<Edge<MachineState, ExplorationStep>>();
      if ((isForFuture ? (this.visitGraph.TryGetOutGoingEdges(node1, out edgeList) ? 1 : 0) : (this.visitGraph.TryGetInComingEdges(node1, out edgeList) ? 1 : 0)) != 0)
      {
        foreach (Edge<MachineState, ExplorationStep> edge in edgeList)
        {
          HashSet<Node<MachineState>> nodeSet;
          if (!dictionary1.TryGetValue(edge.Label.Action, out nodeSet))
          {
            nodeSet = new HashSet<Node<MachineState>>();
            dictionary1[edge.Label.Action] = nodeSet;
          }
          if (!nodeSet.Add(isForFuture ? edge.Target : edge.Source))
            edges.Add(edge);
        }
        this.RemoveDuplicateEdges(edges);
        edges.Clear();
      }
      if ((isForFuture ? (this.visitGraph.TryGetOutGoingEdges(node2, out edgeList) ? 1 : 0) : (this.visitGraph.TryGetInComingEdges(node2, out edgeList) ? 1 : 0)) != 0)
      {
        Dictionary<Action, HashSet<Node<MachineState>>> dictionary2 = new Dictionary<Action, HashSet<Node<MachineState>>>();
        foreach (Edge<MachineState, ExplorationStep> edge in edgeList)
        {
          HashSet<Node<MachineState>> nodeSet1;
          if (!dictionary2.TryGetValue(edge.Label.Action, out nodeSet1))
          {
            nodeSet1 = new HashSet<Node<MachineState>>();
            dictionary2[edge.Label.Action] = nodeSet1;
          }
          if (!nodeSet1.Add(isForFuture ? edge.Target : edge.Source))
          {
            edges.Add(edge);
          }
          else
          {
            HashSet<Node<MachineState>> nodeSet2;
            if (dictionary1.TryGetValue(edge.Label.Action, out nodeSet2))
            {
              if (nodeSet2.Remove(isForFuture ? edge.Target : edge.Source))
              {
                if (nodeSet2.Count == 0)
                  dictionary1.Remove(edge.Label.Action);
              }
              else
              {
                this.RemoveDuplicateEdges(edges);
                return false;
              }
            }
            else
            {
              this.RemoveDuplicateEdges(edges);
              return false;
            }
          }
        }
        this.RemoveDuplicateEdges(edges);
      }
      return dictionary1.Count == 0;
    }

    private void Merge(Node<MachineState> node1, Node<MachineState> node2, bool isForFuture)
    {
      List<Edge<MachineState, ExplorationStep>> edgeList1 = (List<Edge<MachineState, ExplorationStep>>) null;
      foreach (Node<MachineState> key in this.equivalentDataStates[node1.Label.Data])
      {
        if (key != node1 && key != node2)
          this.candidateCousins.Add(new ExplorationCleanupAlgorithm.Cousin(key, node1));
      }
      List<Edge<MachineState, ExplorationStep>> edgeList2;
      if ((isForFuture ? (this.visitGraph.TryGetInComingEdges(node1, out edgeList2) ? 1 : 0) : (this.visitGraph.TryGetOutGoingEdges(node1, out edgeList2) ? 1 : 0)) != 0)
      {
        foreach (Edge<MachineState, ExplorationStep> edge1 in edgeList2)
        {
          HashSet<Node<MachineState>> equivalentDataState = this.equivalentDataStates[isForFuture ? edge1.Source.Label.Data : edge1.Target.Label.Data];
          if ((isForFuture ? (this.visitGraph.TryGetInComingEdges(node2, out edgeList1) ? 1 : 0) : (this.visitGraph.TryGetOutGoingEdges(node2, out edgeList1) ? 1 : 0)) != 0)
          {
            foreach (Edge<MachineState, ExplorationStep> edge2 in edgeList1)
            {
              Node<MachineState> key = isForFuture ? edge1.Source : edge1.Target;
              Node<MachineState> node = isForFuture ? edge2.Source : edge2.Target;
              if (key != node && equivalentDataState.Contains(edge2.Source))
                this.candidateCousins.Add(new ExplorationCleanupAlgorithm.Cousin(key, node));
            }
          }
        }
      }
      lock (this.abortLock)
      {
        if ((isForFuture ? (this.visitGraph.TryGetInComingEdges(node2, out edgeList1) ? 1 : 0) : (this.visitGraph.TryGetOutGoingEdges(node2, out edgeList1) ? 1 : 0)) != 0)
        {
          foreach (Edge<MachineState, ExplorationStep> edge in new List<Edge<MachineState, ExplorationStep>>((IEnumerable<Edge<MachineState, ExplorationStep>>) edgeList1))
          {
            if (isForFuture)
            {
              if (edge.Target != node1)
                this.visitGraph.RelinkEdge(edge, edge.Source, false, node1);
            }
            else if (edge.Source != node1)
              this.visitGraph.RelinkEdge(edge, node1, this.visitGraph.IsStart(node2), edge.Target);
          }
        }
        this.Cousins[node2] = node1;
        if (this.visitGraph.IsStart(node2))
          this.visitGraph.AddNode(node1, true);
        this.visitGraph.DeleteNode(node2);
        this.equivalentDataStates[node2.Label.Data].Remove(node2);
      }
      this.eventAdapter.ShowStatistics(new ExplorationStatistics(ExplorationStatus.Cleanup, 0, this.visitGraph.Nodes.Count<Node<MachineState>>(), this.visitGraph.Edges.Count<Edge<MachineState, ExplorationStep>>(), 0, 0, 0, false));
    }

    private void RemoveDuplicateEdges(List<Edge<MachineState, ExplorationStep>> edges)
    {
      lock (this.abortLock)
      {
        foreach (Edge<MachineState, ExplorationStep> edge in edges)
          this.visitGraph.DeleteEdge(edge);
      }
    }

    private class Cousin
    {
      private int? hashCode = new int?();

      internal Node<MachineState> Key { get; private set; }

      internal Node<MachineState> Value { get; private set; }

      internal Cousin(Node<MachineState> key, Node<MachineState> value)
      {
        this.Key = key;
        this.Value = value;
      }

      public override bool Equals(object obj)
      {
        if (!(obj is ExplorationCleanupAlgorithm.Cousin))
          return false;
        ExplorationCleanupAlgorithm.Cousin cousin = obj as ExplorationCleanupAlgorithm.Cousin;
        return this.Key == cousin.Key && this.Value == cousin.Value || this.Key == cousin.Value && this.Value == cousin.Key;
      }

      public override int GetHashCode()
      {
        if (!this.hashCode.HasValue)
          this.hashCode = new int?(this.Key.GetHashCode() + this.Value.GetHashCode());
        return this.hashCode.Value;
      }
    }
  }
}
