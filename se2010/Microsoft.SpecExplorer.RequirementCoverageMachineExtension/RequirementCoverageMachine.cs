// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Extensions.RequirementCoverageMachine
// Assembly: Microsoft.SpecExplorer.RequirementCoverageMachineExtension, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 116B3DB6-9682-4BA1-A515-610BFE775CA1
// Assembly location: C:\source\SE_Github_Win\SpecExplorer2010\se2010\Microsoft.SpecExplorer.Commandline\bin\Debug\Extensions\Microsoft.SpecExplorer.RequirementCoverageMachineExtension.dll

using Microsoft.ActionMachines;
using Microsoft.GraphTraversal;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.SpecExplorer.Extensions
{
  public class RequirementCoverageMachine : TransformationMachineBase
  {
    internal RequirementCoverageMachine(
      TransformationMachineProviderBase provider,
      IMachine baseMachine,
      IDictionary<string, string> options)
      : base(provider, baseMachine, options)
    {
    }

    private HashSet<string> RequirementsToCover
    {
      get
      {
        string reqs;
        if (this.Options == null || !this.Options.TryGetValue("requirementstocover", out reqs))
          return (HashSet<string>) null;
        return reqs.Length <= 0 ? new HashSet<string>() : this.SplitRequirements(reqs);
      }
    }

    private HashSet<string> SplitRequirements(string reqs)
    {
      HashSet<string> stringSet = new HashSet<string>();
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = false;
      foreach (char req in reqs)
      {
        switch (req)
        {
          case ',':
            if (flag)
            {
              stringBuilder.Append(',');
            }
            else
            {
              string str = stringBuilder.ToString().Trim();
              stringBuilder.Length = 0;
              if (str.Length != 0)
                stringSet.Add(str);
            }
            flag = false;
            break;
          case '\\':
            if (flag)
            {
              stringBuilder.Append('\\');
              break;
            }
            flag = true;
            break;
          default:
            if (flag)
              stringBuilder.Append('\\');
            flag = false;
            stringBuilder.Append(req);
            break;
        }
      }
      if (stringBuilder.Length > 0)
        stringSet.Add(stringBuilder.ToString().Trim());
      return stringSet;
    }

    private RequirementCoverStrategy Strategy
    {
      get
      {
        string str;
        return this.Options == null || !this.Options.TryGetValue("strategy", out str) || str.ToLower() == "full" ? RequirementCoverStrategy.Full : RequirementCoverStrategy.Selective;
      }
    }

    private uint MinimumRequirementCount
    {
      get
      {
        string s;
        return this.Options != null && this.Options.TryGetValue("minimumrequirementcount", out s) ? uint.Parse(s) : 0U;
      }
    }

    public override IEnumerable<IActionSymbol> OfferedActions => this.Configuration.BasicActions.Intersect<IActionSymbol>(this.BaseMachine.OfferedActions);

    public override IGraph<MachineState, ExplorationStep> BuildTraversal(
      ReferGraph<MachineState, ExplorationStep> referGraph)
    {
      RequirementCoverageMachineProvider provider = this.Provider as RequirementCoverageMachineProvider;
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) provider.RequirementsCoveredByInnerMachine);
      if (this.RequirementsToCover != null)
        stringSet.IntersectWith((IEnumerable<string>) this.RequirementsToCover);
      if ((long) provider.RequirementsCoveredByInnerMachine.Count < (long) this.MinimumRequirementCount)
        throw new ExplorationRuntimeException(string.Format("A minimum of {0} requirements was requested, but only {1} are captured in the original behavior", (object) this.MinimumRequirementCount, (object) provider.RequirementsCoveredByInnerMachine.Count));
      if ((long) stringSet.Count < (long) this.MinimumRequirementCount)
        throw new ExplorationRuntimeException(string.Format("A minimum of {0} requirements was requested, but the intersection of requirements covered by original behavior and those provided by RequirementsToCover switch only contains {1}", (object) this.MinimumRequirementCount, (object) stringSet.Count));
      IGraph<MachineState, ExplorationStep> graph = (IGraph<MachineState, ExplorationStep>) new Graph<MachineState, ExplorationStep>();
      HashSet<string> targetReqSet = new HashSet<string>();
      if (this.RequirementsToCover == null)
        targetReqSet.UnionWith((IEnumerable<string>) this.Provider.RequirementsCoveredByInnerMachine);
      else
        targetReqSet.UnionWith((IEnumerable<string>) this.RequirementsToCover);
      if (targetReqSet.Count > 0)
      {
        HashSet<Node<MachineState>> endNodes = new HashSet<Node<MachineState>>();
        HashSet<Edge<MachineState, ExplorationStep>> edgeSet;
        if (this.Strategy == RequirementCoverStrategy.Full)
        {
          edgeSet = provider.ComputeMustEdgeFully((Graph<MachineState, ExplorationStep>) referGraph, targetReqSet);
          foreach (Edge<MachineState, ExplorationStep> edge in edgeSet)
            endNodes.Add(edge.Target);
        }
        else
          edgeSet = provider.ComputeMustEdgeSelectively((Graph<MachineState, ExplorationStep>) referGraph, targetReqSet, endNodes);
        if (edgeSet != null && edgeSet.Count > 0 && referGraph.HasStartNode)
        {
          if (this.Strategy == RequirementCoverStrategy.Full)
          {
            HashSet<Node<MachineState>> nodeSet = new HashSet<Node<MachineState>>();
            foreach (Edge<MachineState, ExplorationStep> edge in edgeSet)
              nodeSet.Add(edge.Source);
            foreach (Node<MachineState> startNode in referGraph.StartNodes)
            {
              ShortestPathAlgorithm<MachineState, ExplorationStep> shortestPathAlgorithm = new ShortestPathAlgorithm<MachineState, ExplorationStep>((IGraph<MachineState, ExplorationStep>) referGraph, startNode, (IEnumerable<Node<MachineState>>) nodeSet);
              shortestPathAlgorithm.Run();
              foreach (Path<MachineState, ExplorationStep> path in shortestPathAlgorithm.ResultDict.Values)
                edgeSet.UnionWith(path.Edges);
            }
          }
          foreach (Edge<MachineState, ExplorationStep> edge in edgeSet)
          {
            graph.AddNode(edge.Source, referGraph.IsStart(edge.Source));
            graph.AddNode(edge.Target, false);
            graph.AddEdge(edge);
          }
          new CompleteChoiceAlgorithm<MachineState, ExplorationStep>(referGraph, graph).Run();
          foreach (Node<MachineState> node in graph.Nodes)
          {
            if (node.Kind == NodeKind.Regular && graph.OutgoingCount(node) == 0)
              endNodes.Add(node);
          }
          new CompleteAcceptingAlgorithm<MachineState, ExplorationStep>(referGraph, graph, (IEnumerable<Node<MachineState>>) endNodes).Run();
          if (this.Strategy == RequirementCoverStrategy.Selective)
            graph = RequirementCoverageMachine.ReduceGraphWithoutReducingRequirementCoverage(graph);
        }
      }
      return graph;
    }

    private static IGraph<MachineState, ExplorationStep> ReduceGraphWithoutReducingRequirementCoverage(
      IGraph<MachineState, ExplorationStep> graph)
    {
      RequirementsWinningStrategy<MachineState, ExplorationStep> requirementsWinningStrategy = new RequirementsWinningStrategyBuilder<MachineState, ExplorationStep>(graph).BuildWinningStrategy();
      HashSet<Node<MachineState>> nodeSet = new HashSet<Node<MachineState>>();
      foreach (Node<MachineState> node in graph.StartNodes.Where<Node<MachineState>>((Func<Node<MachineState>, bool>) (s => graph.IncomingCount(s) == 0)))
      {
        RequirementsCoverMap<Edge<MachineState, ExplorationStep>> source = requirementsWinningStrategy.CoverMapOfNode(node);
        if (source.Count > 0)
        {
          foreach (Node<MachineState> startNode in graph.StartNodes)
          {
            if (node != startNode && !nodeSet.Contains(startNode))
            {
              RequirementsCoverMap<Edge<MachineState, ExplorationStep>> otherCoverMap = requirementsWinningStrategy.CoverMapOfNode(startNode);
              if (source.Count <= otherCoverMap.Count && source.All<KeyValuePair<string, StrategyCoverMode<Edge<MachineState, ExplorationStep>>>>((Func<KeyValuePair<string, StrategyCoverMode<Edge<MachineState, ExplorationStep>>>, bool>) (elem =>
              {
                if (!otherCoverMap.Keys.Contains<string>(elem.Key))
                  return false;
                return otherCoverMap[elem.Key].CoverKind == elem.Value.CoverKind || otherCoverMap[elem.Key].CoverKind == StrategyCoverKind.AssuredCovered;
              })))
                nodeSet.Add(node);
            }
          }
        }
        else
          nodeSet.Add(node);
      }
      if (nodeSet.Count <= 0)
        return graph;
      SubGraphReductionAlgorithm<MachineState, ExplorationStep> reductionAlgorithm = new SubGraphReductionAlgorithm<MachineState, ExplorationStep>(graph, (IEnumerable<Node<MachineState>>) nodeSet);
      reductionAlgorithm.Run();
      return reductionAlgorithm.TargetGraph;
    }
  }
}
