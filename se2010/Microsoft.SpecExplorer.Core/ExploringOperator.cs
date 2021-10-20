using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.ActionMachines;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;

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

		internal ExploringOperator(IMachine machine, IConfiguration config, EventAdapter eventAdapter, ExplorerConfiguration explorerConfig, ExplorationOptions options, EventWaitHandle workerWaitHandle, IRemoteExplorer explorer)
			: base(machine, config, eventAdapter, explorerConfig, options, workerWaitHandle, explorer)
		{
			graphUsedForCleanup = new ReferGraph<MachineState, ExplorationStep>();
			cleanupAlgorithm = new ExplorationCleanupAlgorithm(graphUsedForCleanup, explorer.AbortLock, base.eventAdapter);
			stateFlagsDictionary = new Dictionary<Node<MachineState>, ExplorationStateFlags>();
			subsumptionStateDictionary = new Dictionary<Node<MachineState>, Node<MachineState>>();
			subsumptionKindDictionary = new Dictionary<Node<MachineState>, SubsumptionResult>();
			enableCleanup = options.EnableExplorationCleanup;
		}

		public override void Explore()
		{
			bool flag = false;
			try
			{
				eventAdapter.SwitchState(ExplorationState.Exploring);
				host.GetRequiredService<IStateProvider>().RandomSeed = machine.Configuration.OptionSet.GetOptions<ExplorationOptions>().RandomSeed;
				machineExplorer = host.GetRequiredService<IMachineExplorationExplorerProvider>().CreateExplorer(config);
				machineExplorer.OnSuspension += base.OnSuspension;
				if (enableCleanup)
				{
					machineExplorer.OnStep += AddStepForCleanup;
					machineExplorer.OnState += AddStateForCleanup;
				}
				else
				{
					machineExplorer.OnStep += base.OnStep;
					machineExplorer.OnState += base.OnState;
				}
				eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job started.");
				lastStatisticsTime = DateTime.Now;
				transitionSystemBuilder = new TransitionSystemBuilder(explorerConfig.Machine, machine.Configuration, host, eventAdapter);
				eventAdapter.ShowStatistics(new ExplorationStatistics());
				machineExplorer.Explore(machine);
				IMachineExplorationExplorer machineExplorationExplorer = machineExplorer as IMachineExplorationExplorer;
				if (machineExplorationExplorer != null)
				{
					ExplorationStatistics statistics = new ExplorationStatistics(machineExplorationExplorer.Statistics.ExplorationStatus, machineExplorationExplorer.Statistics.ExplorerNumber, machineExplorationExplorer.Statistics.StateCount, machineExplorationExplorer.Statistics.StepCount, machineExplorationExplorer.Statistics.RequirementCount, machineExplorationExplorer.Statistics.ErrorCount, machineExplorationExplorer.Statistics.BoundedStateCount, true);
					eventAdapter.ShowStatistics(statistics);
				}
				if (enableCleanup)
				{
					cleanupAlgorithm.Initialize();
					cleanupAlgorithm.Run();
				}
			}
			catch (ThreadAbortException)
			{
				flag = true;
				Thread.ResetAbort();
			}
			BuildTransitionSystem();
			if (flag)
			{
				if (base.ExplorationResult != null)
				{
					base.ExplorationResult.Extensions.IgnoreSignature = true;
				}
			}
			else
			{
				eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job finished.");
				eventAdapter.SwitchState(ExplorationState.FinishedExploring);
			}
		}

		private void BuildTransitionSystem()
		{
			if (transitionSystemBuilder == null)
			{
				return;
			}
			if (enableCleanup && graphUsedForCleanup != null)
			{
				if (cleanupAlgorithm.Cousins != null && cleanupAlgorithm.Cousins.Count > 0)
				{
					foreach (Node<MachineState> key2 in cleanupAlgorithm.Cousins.Keys)
					{
						Node<MachineState> key = cleanupAlgorithm.Cousins[key2];
						stateFlagsDictionary[key] |= stateFlagsDictionary[key2];
					}
					foreach (Node<MachineState> item in new List<Node<MachineState>>(subsumptionStateDictionary.Keys))
					{
						Node<MachineState> mergedToNode = GetMergedToNode(item);
						if (mergedToNode != item)
						{
							subsumptionStateDictionary[mergedToNode] = GetMergedToNode(subsumptionStateDictionary[item]);
							subsumptionKindDictionary[mergedToNode] = subsumptionKindDictionary[item];
						}
					}
				}
				DepthFirstSearchAlgorithm<MachineState, ExplorationStep> depthFirstSearchAlgorithm = new DepthFirstSearchAlgorithm<MachineState, ExplorationStep>(graphUsedForCleanup);
				depthFirstSearchAlgorithm.DiscoverNode += delegate(object sender, NodeEventArgs<MachineState> args)
				{
					transitionSystemBuilder.AddState(args.Node.Label, stateFlagsDictionary[args.Node]);
					if (args.Node.Label.Control.Kind == ControlStateKind.Error)
					{
						ReportErrorState(args.Node.Label);
					}
				};
				depthFirstSearchAlgorithm.VisitEdge += delegate(object sender, EdgeEventArgs<MachineState, ExplorationStep> args)
				{
					Edge<MachineState, ExplorationStep> edge = args.Edge;
					ExplorationStep step = new ExplorationStep(edge.Source.Label, stateFlagsDictionary[edge.Source], edge.Label.Action, edge.Label.Requirements, edge.Label.IntermediateTargetState, edge.Target.Label, stateFlagsDictionary[edge.Target], edge.Label.FreedVariables, edge.Label.PreConstraints);
					Node<MachineState> value;
					if (subsumptionStateDictionary.TryGetValue(edge.Target, out value))
					{
						transitionSystemBuilder.AddStep(step, value.Label, subsumptionKindDictionary[edge.Target]);
					}
					else
					{
						transitionSystemBuilder.AddStep(step, null, SubsumptionResult.Inconclusive);
					}
					if (step.IntermediateTargetState.Control.Kind == ControlStateKind.Error)
					{
						ReportErrorStep(step);
					}
				};
				depthFirstSearchAlgorithm.Run();
			}
			ExplorationResult explorationResult = transitionSystemBuilder.BuildTransitionSystem();
			explorationResult.Extensions.IgnoreSignature = machine.AlwaysReexplore;
			base.ExplorationResult = explorationResult;
		}

		private Node<MachineState> GetMergedToNode(Node<MachineState> node)
		{
			if (cleanupAlgorithm.Cousins.ContainsKey(node))
			{
				return GetMergedToNode(cleanupAlgorithm.Cousins[node]);
			}
			return node;
		}

		private void AddStepForCleanup(object sender, StepEventArgs args)
		{
			lock (explorer.AbortLock)
			{
				Node<MachineState> node = graphUsedForCleanup.AddNode(args.Step.SourceState, args.Step.SourceState.Control.Kind.ToNodeKind(), (args.Step.SourceFlags & ExplorationStateFlags.IsStart) != 0);
				stateFlagsDictionary[node] = args.Step.SourceFlags;
				Node<MachineState> node2 = graphUsedForCleanup.AddNode(args.Step.TargetState, args.Step.TargetState.Control.Kind.ToNodeKind(), (args.Step.TargetFlags & ExplorationStateFlags.IsStart) != 0);
				stateFlagsDictionary[node2] = args.Step.TargetFlags;
				if (args.SubsumptionRelatedState.HasValue)
				{
					Node<MachineState> value = graphUsedForCleanup.AddNode(args.SubsumptionRelatedState.Value, args.SubsumptionRelatedState.Value.Control.Kind.ToNodeKind(), (args.Step.TargetFlags & ExplorationStateFlags.IsStart) != 0);
					subsumptionStateDictionary[node2] = value;
					subsumptionKindDictionary[node2] = args.SubsumptionRelationKind;
				}
				graphUsedForCleanup.AddEdge(new Edge<MachineState, ExplorationStep>(node, node2, args.Step, (args.Step.Action.Symbol.Kind & ActionKind.AllObserved) != 0, null));
			}
		}

		private void AddStateForCleanup(object sender, StateEventArgs args)
		{
			lock (explorer.AbortLock)
			{
				Node<MachineState> key = graphUsedForCleanup.AddNode(args.State, args.State.Control.Kind.ToNodeKind(), (args.Flags & ExplorationStateFlags.IsStart) != 0);
				stateFlagsDictionary[key] = args.Flags;
			}
		}
	}
}
