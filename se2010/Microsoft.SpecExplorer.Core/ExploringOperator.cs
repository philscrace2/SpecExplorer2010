// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ExploringOperator
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.SpecExplorer
{
  internal class ExploringOperator : OperatorBase
  {
    private ReferGraph<MachineState, ExplorationStep> graphUsedForCleanup;
    private ExplorationCleanupAlgorithm cleanupAlgorithm;
    private Dictionary<Node<MachineState>, ExplorationStateFlags> stateFlagsDictionary;
    private Dictionary<Node<MachineState>, Node<MachineState>> subsumptionStateDictionary;
    private Dictionary<Node<MachineState>, SubsumptionResult> subsumptionKindDictionary;
    private bool enableCleanup = true;

    internal ExploringOperator(
      IMachine machine,
      IConfiguration config,
      EventAdapter eventAdapter,
      ExplorerConfiguration explorerConfig,
      ExplorationOptions options,
      EventWaitHandle workerWaitHandle,
      IRemoteExplorer explorer)
      : base(machine, config, eventAdapter, explorerConfig, options, workerWaitHandle, explorer)
    {
      this.graphUsedForCleanup = new ReferGraph<MachineState, ExplorationStep>();
      this.cleanupAlgorithm = new ExplorationCleanupAlgorithm((IGraph<MachineState, ExplorationStep>) this.graphUsedForCleanup, explorer.AbortLock, this.eventAdapter);
      this.stateFlagsDictionary = new Dictionary<Node<MachineState>, ExplorationStateFlags>();
      this.subsumptionStateDictionary = new Dictionary<Node<MachineState>, Node<MachineState>>();
      this.subsumptionKindDictionary = new Dictionary<Node<MachineState>, SubsumptionResult>();
      this.enableCleanup = options.EnableExplorationCleanup;
    }

    public override void Explore()
    {
      bool flag = false;
      try
      {
        this.eventAdapter.SwitchState(ExplorationState.Exploring);
        this.host.GetRequiredService<IStateProvider>().RandomSeed = this.machine.Configuration.OptionSet.GetOptions<ExplorationOptions>().RandomSeed;
        this.machineExplorer = (IActionMachineExplorer) this.host.GetRequiredService<IMachineExplorationExplorerProvider>().CreateExplorer(this.config);
        this.machineExplorer.OnSuspension += new EventHandler<SuspensionEventArgs>(((OperatorBase) this).OnSuspension);
        if (this.enableCleanup)
        {
          this.machineExplorer.OnStep += new EventHandler<StepEventArgs>(this.AddStepForCleanup);
          this.machineExplorer.OnState += new EventHandler<StateEventArgs>(this.AddStateForCleanup);
        }
        else
        {
          this.machineExplorer.OnStep += new EventHandler<StepEventArgs>(((OperatorBase) this).OnStep);
          this.machineExplorer.OnState += new EventHandler<StateEventArgs>(((OperatorBase) this).OnState);
        }
        this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job started.");
        this.lastStatisticsTime = DateTime.Now;
        this.transitionSystemBuilder = new TransitionSystemBuilder(this.explorerConfig.Machine, this.machine.Configuration, this.host, this.eventAdapter);
        this.eventAdapter.ShowStatistics(new ExplorationStatistics());
        this.machineExplorer.Explore(this.machine);
        if (this.machineExplorer is IMachineExplorationExplorer machineExplorer2)
          this.eventAdapter.ShowStatistics(new ExplorationStatistics(machineExplorer2.Statistics.ExplorationStatus, machineExplorer2.Statistics.ExplorerNumber, machineExplorer2.Statistics.StateCount, machineExplorer2.Statistics.StepCount, machineExplorer2.Statistics.RequirementCount, machineExplorer2.Statistics.ErrorCount, machineExplorer2.Statistics.BoundedStateCount, true));
        if (this.enableCleanup)
        {
          this.cleanupAlgorithm.Initialize();
          this.cleanupAlgorithm.Run();
        }
      }
      catch (ThreadAbortException ex)
      {
        flag = true;
        Thread.ResetAbort();
      }
      this.BuildTransitionSystem();
      if (flag)
      {
        if (this.ExplorationResult == null)
          return;
        this.ExplorationResult.Extensions.IgnoreSignature = (__Null) 1;
      }
      else
      {
        this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job finished.");
        this.eventAdapter.SwitchState(ExplorationState.FinishedExploring);
      }
    }

    private void BuildTransitionSystem()
    {
      if (this.transitionSystemBuilder == null)
        return;
      if (this.enableCleanup && this.graphUsedForCleanup != null)
      {
        if (this.cleanupAlgorithm.Cousins != null && this.cleanupAlgorithm.Cousins.Count > 0)
        {
          foreach (Node<MachineState> key1 in this.cleanupAlgorithm.Cousins.Keys)
          {
            Node<MachineState> cousin = this.cleanupAlgorithm.Cousins[key1];
            Dictionary<Node<MachineState>, ExplorationStateFlags> stateFlagsDictionary;
            Node<MachineState> key2;
            (stateFlagsDictionary = this.stateFlagsDictionary)[key2 = cousin] = stateFlagsDictionary[key2] | this.stateFlagsDictionary[key1];
          }
          foreach (Node<MachineState> node in new List<Node<MachineState>>((IEnumerable<Node<MachineState>>) this.subsumptionStateDictionary.Keys))
          {
            Node<MachineState> mergedToNode = this.GetMergedToNode(node);
            if (mergedToNode != node)
            {
              this.subsumptionStateDictionary[mergedToNode] = this.GetMergedToNode(this.subsumptionStateDictionary[node]);
              this.subsumptionKindDictionary[mergedToNode] = this.subsumptionKindDictionary[node];
            }
          }
        }
        DepthFirstSearchAlgorithm<MachineState, ExplorationStep> firstSearchAlgorithm = new DepthFirstSearchAlgorithm<MachineState, ExplorationStep>((IGraph<MachineState, ExplorationStep>) this.graphUsedForCleanup);
        firstSearchAlgorithm.DiscoverNode += (EventHandler<NodeEventArgs<MachineState>>) ((sender, args) =>
        {
          this.transitionSystemBuilder.AddState(args.Node.Label, this.stateFlagsDictionary[args.Node]);
          if (args.Node.Label.Control.Kind != ControlStateKind.Error)
            return;
          this.ReportErrorState(args.Node.Label);
        });
        firstSearchAlgorithm.VisitEdge += (EventHandler<EdgeEventArgs<MachineState, ExplorationStep>>) ((sender, args) =>
        {
          Edge<MachineState, ExplorationStep> edge = args.Edge;
          ExplorationStep step = new ExplorationStep(edge.Source.Label, this.stateFlagsDictionary[edge.Source], edge.Label.Action, edge.Label.Requirements, edge.Label.IntermediateTargetState, edge.Target.Label, this.stateFlagsDictionary[edge.Target], edge.Label.FreedVariables, edge.Label.PreConstraints);
          Node<MachineState> node;
          if (this.subsumptionStateDictionary.TryGetValue(edge.Target, out node))
            this.transitionSystemBuilder.AddStep(step, new MachineState?(node.Label), this.subsumptionKindDictionary[edge.Target]);
          else
            this.transitionSystemBuilder.AddStep(step, new MachineState?(), SubsumptionResult.Inconclusive);
          if (step.IntermediateTargetState.Control.Kind != ControlStateKind.Error)
            return;
          this.ReportErrorStep(step);
        });
        firstSearchAlgorithm.Run();
      }
      ExplorationResult explorationResult = this.transitionSystemBuilder.BuildTransitionSystem();
      explorationResult.Extensions.IgnoreSignature = (__Null) (this.machine.AlwaysReexplore ? 1 : 0);
      this.ExplorationResult = explorationResult;
    }

    private Node<MachineState> GetMergedToNode(Node<MachineState> node) => this.cleanupAlgorithm.Cousins.ContainsKey(node) ? this.GetMergedToNode(this.cleanupAlgorithm.Cousins[node]) : node;

    private void AddStepForCleanup(object sender, StepEventArgs args)
    {
      lock (this.explorer.AbortLock)
      {
        Node<MachineState> node1 = this.graphUsedForCleanup.AddNode(args.Step.SourceState, args.Step.SourceState.Control.Kind.ToNodeKind(), (args.Step.SourceFlags & ExplorationStateFlags.IsStart) != ExplorationStateFlags.None);
        this.stateFlagsDictionary[node1] = args.Step.SourceFlags;
        Node<MachineState> node2 = this.graphUsedForCleanup.AddNode(args.Step.TargetState, args.Step.TargetState.Control.Kind.ToNodeKind(), (args.Step.TargetFlags & ExplorationStateFlags.IsStart) != ExplorationStateFlags.None);
        this.stateFlagsDictionary[node2] = args.Step.TargetFlags;
        if (args.SubsumptionRelatedState.HasValue)
        {
          Node<MachineState> node3 = this.graphUsedForCleanup.AddNode(args.SubsumptionRelatedState.Value, args.SubsumptionRelatedState.Value.Control.Kind.ToNodeKind(), (args.Step.TargetFlags & ExplorationStateFlags.IsStart) != ExplorationStateFlags.None);
          this.subsumptionStateDictionary[node2] = node3;
          this.subsumptionKindDictionary[node2] = args.SubsumptionRelationKind;
        }
        this.graphUsedForCleanup.AddEdge(new Edge<MachineState, ExplorationStep>(node1, node2, args.Step, (args.Step.Action.Symbol.Kind & ActionKind.AllObserved) != (ActionKind) 0, (IEnumerable<string>) null));
      }
    }

    private void AddStateForCleanup(object sender, StateEventArgs args)
    {
      lock (this.explorer.AbortLock)
        this.stateFlagsDictionary[this.graphUsedForCleanup.AddNode(args.State, args.State.Control.Kind.ToNodeKind(), (args.Flags & ExplorationStateFlags.IsStart) != ExplorationStateFlags.None)] = args.Flags;
    }
  }
}
