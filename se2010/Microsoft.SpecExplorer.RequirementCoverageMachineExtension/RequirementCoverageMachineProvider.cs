// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Extensions.RequirementCoverageMachineProvider
// Assembly: Microsoft.SpecExplorer.RequirementCoverageMachineExtension, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 116B3DB6-9682-4BA1-A515-610BFE775CA1
// Assembly location: C:\source\SE_Github_Win\SpecExplorer2010\se2010\Microsoft.SpecExplorer.Commandline\bin\Debug\Extensions\Microsoft.SpecExplorer.RequirementCoverageMachineExtension.dll

using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.ActionMachines.Cord.Construct;
using Microsoft.GraphTraversal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer.Extensions
{
  [SpecExplorerExtension]
  public class RequirementCoverageMachineProvider : 
    TransformationMachineProviderBase,
    IConstructProvider
  {
    public override void Initialize()
    {
      base.Initialize();
      this.GetRequiredService<IConstructManager>().RegisterConstruct("requirement coverage", (IConstructProvider) this);
    }

    public void CheckConstruct(Checker checker, MachineBehavior.Construct construct)
    {
      if (construct.Behavior == null)
        checker.ReportSemanticError(construct.Location, "requirement coverage machine construct must have a behavior");
      if (construct.CodeBlock != null)
        checker.ReportSemanticError(construct.CodeBlock.Location, "requirement coverage machine construct must not have a code block.");
      construct.ResolvedOptions = (IDictionary<string, string>) new Dictionary<string, string>();
      if (construct.Options == null)
        return;
      foreach (ConfigClause.DeclareSwitch option in (IEnumerable<ConfigClause.DeclareSwitch>) construct.Options)
      {
        construct.ResolvedOptions[option.Name.ToLower()] = option.Value;
        if (string.Compare(option.Name, "requirementstocover", true) != 0)
        {
          if (string.Compare(option.Name, "strategy", true) == 0)
          {
            string lower = option.Value.ToLower();
            if (lower != "full" && lower != "selective")
              checker.ReportSemanticError(option.Location, "the value of option \"strategy\" can only be \"full\" or \"selective\"");
          }
          else if (string.Compare(option.Name, "minimumrequirementcount", true) == 0)
          {
            try
            {
              int uint32 = (int) Convert.ToUInt32(option.Value);
            }
            catch (FormatException ex)
            {
              checker.ReportSemanticError(option.Location, string.Format("the value of option \"MinimumRequirementCount\" can only be integer between 0 and {0}", (object) uint.MaxValue));
            }
            catch (OverflowException ex)
            {
              checker.ReportSemanticError(option.Location, string.Format("the value of option \"MinimumRequirementCount\" can only be integer between 0 and {0}", (object) uint.MaxValue));
            }
          }
          else
            checker.ReportSemanticError(option.Location, "requirement coverage machine construct does not support option \"{0}\"", (object) option.Name);
        }
      }
    }

    public IMachine BuildConstruct(
      MachineBehavior.Construct construct,
      IMachine baseMachine)
    {
      return (IMachine) new RequirementCoverageMachine((TransformationMachineProviderBase) this, baseMachine, construct.ResolvedOptions);
    }

    public override IMachine CreateMachine(
      IMachine baseMachine,
      IDictionary<string, string> options)
    {
      return (IMachine) null;
    }

    public HashSet<Edge<MachineState, ExplorationStep>> ComputeMustEdgeFully(
      Graph<MachineState, ExplorationStep> graph,
      HashSet<string> targetReqSet)
    {
      HashSet<Edge<MachineState, ExplorationStep>> edgeSet = new HashSet<Edge<MachineState, ExplorationStep>>();
      foreach (Edge<MachineState, ExplorationStep> edge in graph.Edges)
      {
        HashSet<string> stringSet = new HashSet<string>(edge.Requirements);
        stringSet.IntersectWith((IEnumerable<string>) targetReqSet);
        if (stringSet.Count > 0)
          edgeSet.Add(edge);
      }
      return edgeSet;
    }

    public HashSet<Edge<MachineState, ExplorationStep>> ComputeMustEdgeSelectively(
      Graph<MachineState, ExplorationStep> graph,
      HashSet<string> targetReqSet,
      HashSet<Node<MachineState>> endNodes)
    {
      if (endNodes == null)
        throw new ArgumentNullException(nameof (endNodes));
      RequirementsWinningStrategy<MachineState, ExplorationStep> winningStrategy = new RequirementsWinningStrategyBuilder<MachineState, ExplorationStep>((IGraph<MachineState, ExplorationStep>) graph).BuildWinningStrategy();
      HashSet<Edge<MachineState, ExplorationStep>> edgeSet = new HashSet<Edge<MachineState, ExplorationStep>>();
      RequirementsCoverMap<Edge<MachineState, ExplorationStep>> targetCoverMap = new RequirementsCoverMap<Edge<MachineState, ExplorationStep>>();
      foreach (Node<MachineState> startNode in graph.StartNodes)
      {
        RequirementsCoverMap<Edge<MachineState, ExplorationStep>> other = winningStrategy.CoverMapOfNode(startNode);
        targetCoverMap.UnionWith(other);
      }
      foreach (string key in targetCoverMap.Keys.ToArray<string>())
      {
        if (!targetReqSet.Contains(key))
          targetCoverMap.Remove(key);
      }
      if (targetCoverMap.Count <= 0)
        return edgeSet;
      Queue<Dictionary<Edge<MachineState, ExplorationStep>, RequirementsCoverMap<Edge<MachineState, ExplorationStep>>>> dictionaryQueue = new Queue<Dictionary<Edge<MachineState, ExplorationStep>, RequirementsCoverMap<Edge<MachineState, ExplorationStep>>>>();
      foreach (Node<MachineState> startNode in graph.StartNodes)
      {
        if (targetCoverMap.Count > 0)
        {
          Dictionary<Edge<MachineState, ExplorationStep>, RequirementsCoverMap<Edge<MachineState, ExplorationStep>>> edgeTargetReqsMap;
          if (this.TryGetEdgesToCoverRequirements((IGraph<MachineState, ExplorationStep>) graph, startNode, winningStrategy, targetCoverMap, out edgeTargetReqsMap))
          {
            dictionaryQueue.Enqueue(edgeTargetReqsMap);
            foreach (string coverRequirement in winningStrategy.CoverMapOfNode(startNode).WillCoverRequirements)
              targetCoverMap.Remove(coverRequirement);
          }
        }
        else
          break;
      }
      while (dictionaryQueue.Count > 0)
      {
        foreach (KeyValuePair<Edge<MachineState, ExplorationStep>, RequirementsCoverMap<Edge<MachineState, ExplorationStep>>> keyValuePair in dictionaryQueue.Dequeue())
        {
          if (edgeSet.Add(keyValuePair.Key))
          {
            RequirementsCoverMap<Edge<MachineState, ExplorationStep>> targetCoverMapOfEdge = keyValuePair.Value;
            foreach (string key in new HashSet<string>(keyValuePair.Key.Requirements))
              targetCoverMapOfEdge.Remove(key);
            if (targetCoverMapOfEdge.Count <= 0)
            {
              endNodes.Add(keyValuePair.Key.Target);
            }
            else
            {
              RequirementsCoverMap<Edge<MachineState, ExplorationStep>> targetCoverMapOfNode;
              Dictionary<Edge<MachineState, ExplorationStep>, RequirementsCoverMap<Edge<MachineState, ExplorationStep>>> edgeTargetReqsMap;
              if (this.TryGetTargetCoverMapOfNode(keyValuePair.Key.Target, targetCoverMapOfEdge, winningStrategy, out targetCoverMapOfNode) && this.TryGetEdgesToCoverRequirements((IGraph<MachineState, ExplorationStep>) graph, keyValuePair.Key.Target, winningStrategy, targetCoverMapOfNode, out edgeTargetReqsMap))
                dictionaryQueue.Enqueue(edgeTargetReqsMap);
            }
          }
        }
      }
      return edgeSet;
    }

    private bool TryGetTargetCoverMapOfNode(
      Node<MachineState> node,
      RequirementsCoverMap<Edge<MachineState, ExplorationStep>> targetCoverMapOfEdge,
      RequirementsWinningStrategy<MachineState, ExplorationStep> winningStrategy,
      out RequirementsCoverMap<Edge<MachineState, ExplorationStep>> targetCoverMapOfNode)
    {
      targetCoverMapOfNode = (RequirementsCoverMap<Edge<MachineState, ExplorationStep>>) null;
      if (targetCoverMapOfEdge == null || targetCoverMapOfEdge.Count <= 0)
        return false;
      RequirementsCoverMap<Edge<MachineState, ExplorationStep>> requirementsCoverMap1 = new RequirementsCoverMap<Edge<MachineState, ExplorationStep>>();
      RequirementsCoverMap<Edge<MachineState, ExplorationStep>> requirementsCoverMap2 = winningStrategy.CoverMapOfNode(node);
      foreach (string coverRequirement in targetCoverMapOfEdge.WillCoverRequirements)
        requirementsCoverMap1[coverRequirement] = requirementsCoverMap2[coverRequirement];
      if (requirementsCoverMap1.Count <= 0)
        return false;
      targetCoverMapOfNode = requirementsCoverMap1;
      return true;
    }

    private bool TryGetEdgesToCoverRequirements(
      IGraph<MachineState, ExplorationStep> graph,
      Node<MachineState> node,
      RequirementsWinningStrategy<MachineState, ExplorationStep> winningStrategy,
      RequirementsCoverMap<Edge<MachineState, ExplorationStep>> targetCoverMap,
      out Dictionary<Edge<MachineState, ExplorationStep>, RequirementsCoverMap<Edge<MachineState, ExplorationStep>>> edgeTargetReqsMap)
    {
      edgeTargetReqsMap = (Dictionary<Edge<MachineState, ExplorationStep>, RequirementsCoverMap<Edge<MachineState, ExplorationStep>>>) null;
      if (targetCoverMap == null || targetCoverMap.Count <= 0 || winningStrategy.CoverMapOfNode(node).Count == 0)
        return false;
      edgeTargetReqsMap = new Dictionary<Edge<MachineState, ExplorationStep>, RequirementsCoverMap<Edge<MachineState, ExplorationStep>>>();
      List<Edge<MachineState, ExplorationStep>> outEdges;
      if (graph.TryGetOutGoingEdges(node, out outEdges))
      {
        if (graph.IsChoice(node))
        {
          foreach (Edge<MachineState, ExplorationStep> key in outEdges)
            edgeTargetReqsMap[key] = new RequirementsCoverMap<Edge<MachineState, ExplorationStep>>();
          foreach (KeyValuePair<string, StrategyCoverMode<Edge<MachineState, ExplorationStep>>> targetCover in (Dictionary<string, StrategyCoverMode<Edge<MachineState, ExplorationStep>>>) targetCoverMap)
          {
            if (targetCover.Value.TargetEdge.Source != node)
            {
              edgeTargetReqsMap = (Dictionary<Edge<MachineState, ExplorationStep>, RequirementsCoverMap<Edge<MachineState, ExplorationStep>>>) null;
              return false;
            }
            if (targetCover.Value.CoverKind == StrategyCoverKind.AssuredCovered)
            {
              foreach (Edge<MachineState, ExplorationStep> edge in outEdges)
              {
                RequirementsCoverMap<Edge<MachineState, ExplorationStep>> requirementsCoverMap = winningStrategy.CoverMapOfEdge(edge);
                if (requirementsCoverMap.RequirementCoverKind(targetCover.Key) == StrategyCoverKind.AssuredCovered)
                  edgeTargetReqsMap[edge].Add(targetCover.Key, new StrategyCoverMode<Edge<MachineState, ExplorationStep>>(StrategyCoverKind.AssuredCovered, requirementsCoverMap[targetCover.Key].PathLength - 1, (Edge<MachineState, ExplorationStep>) null));
              }
            }
            else if (targetCover.Value.CoverKind == StrategyCoverKind.PossibleCovered)
              edgeTargetReqsMap[targetCover.Value.TargetEdge].Add(targetCover.Key, new StrategyCoverMode<Edge<MachineState, ExplorationStep>>(StrategyCoverKind.PossibleCovered, targetCover.Value.PathLength - 1, (Edge<MachineState, ExplorationStep>) null));
          }
        }
        else
        {
          HashSet<Edge<MachineState, ExplorationStep>> targetEdges = new HashSet<Edge<MachineState, ExplorationStep>>(targetCoverMap.Values.Where<StrategyCoverMode<Edge<MachineState, ExplorationStep>>>((Func<StrategyCoverMode<Edge<MachineState, ExplorationStep>>, bool>) (item => item.CoverKind != StrategyCoverKind.None)).Select<StrategyCoverMode<Edge<MachineState, ExplorationStep>>, Edge<MachineState, ExplorationStep>>((Func<StrategyCoverMode<Edge<MachineState, ExplorationStep>>, Edge<MachineState, ExplorationStep>>) (item => item.TargetEdge)));
          foreach (Edge<MachineState, ExplorationStep> edge in outEdges.Where<Edge<MachineState, ExplorationStep>>((Func<Edge<MachineState, ExplorationStep>, bool>) (e => targetEdges.Contains(e))))
          {
            foreach (KeyValuePair<string, StrategyCoverMode<Edge<MachineState, ExplorationStep>>> targetCover in (Dictionary<string, StrategyCoverMode<Edge<MachineState, ExplorationStep>>>) targetCoverMap)
            {
              StrategyCoverMode<Edge<MachineState, ExplorationStep>> strategyCoverMode = targetCover.Value;
              if (strategyCoverMode.CoverKind != StrategyCoverKind.None && strategyCoverMode.TargetEdge == edge)
              {
                RequirementsCoverMap<Edge<MachineState, ExplorationStep>> requirementsCoverMap;
                if (!edgeTargetReqsMap.TryGetValue(strategyCoverMode.TargetEdge, out requirementsCoverMap))
                {
                  requirementsCoverMap = new RequirementsCoverMap<Edge<MachineState, ExplorationStep>>();
                  edgeTargetReqsMap[strategyCoverMode.TargetEdge] = requirementsCoverMap;
                }
                requirementsCoverMap.Add(targetCover.Key, new StrategyCoverMode<Edge<MachineState, ExplorationStep>>(StrategyCoverKind.AssuredCovered, strategyCoverMode.PathLength - 1, strategyCoverMode.TargetEdge));
              }
            }
          }
        }
      }
      if (edgeTargetReqsMap.Count > 0)
        return true;
      edgeTargetReqsMap = (Dictionary<Edge<MachineState, ExplorationStep>, RequirementsCoverMap<Edge<MachineState, ExplorationStep>>>) null;
      return false;
    }
  }
}
