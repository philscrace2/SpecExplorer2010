using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	internal abstract class OperatorBase : IExploringOperator
	{
		protected IRemoteExplorer explorer;

		private bool suspendOnNextStep;

		private MachineState continuedState;

		private EventWaitHandle workerWaitHandle;

		private ITermDescriptionContext termDescriptionContext;

		protected ComponentBase host;

		protected IConfiguration config;

		protected IMachine machine;

		protected ExplorerConfiguration explorerConfig;

		protected ExplorationOptions options;

		protected DateTime lastStatisticsTime;

		protected IActionMachineExplorer machineExplorer;

		protected EventAdapter eventAdapter;

		protected TransitionSystemBuilder transitionSystemBuilder;

		public ExplorationResult ExplorationResult { get; protected set; }

		public event EventHandler<ExplorationResultEventArgs> ExplorationResultUpdated;

		protected OperatorBase(IMachine machine, IConfiguration config, EventAdapter eventAdapter, ExplorerConfiguration explorerConfig, ExplorationOptions options, EventWaitHandle workerWaitHandle, IRemoteExplorer explorer)
		{
			this.config = config;
			host = this.config.Host;
			this.machine = machine;
			this.eventAdapter = eventAdapter;
			this.explorerConfig = explorerConfig;
			this.options = options;
			this.workerWaitHandle = workerWaitHandle;
			this.explorer = explorer;
			termDescriptionContext = host.GetRequiredService<IBackground>().UserDescription;
		}

		public abstract void Explore();

		protected void OnSuspension(object sender, SuspensionEventArgs args)
		{
			eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job suspended.");
			eventAdapter.SwitchState(ExplorationState.SuspendedExploring);
			using (IEnumerator<MachineState> enumerator = args.suspendedStates.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					MachineState machineState = (continuedState = enumerator.Current);
				}
			}
			workerWaitHandle.WaitOne();
			lock (explorer)
			{
				if (explorer.State == ExplorationState.Exploring)
				{
					args.Resume(continuedState, 0);
				}
			}
		}

		protected void OnState(object sender, StateEventArgs e)
		{
			AddStateToTransitionSystem(e.State, e.Flags);
		}

		protected void AddStateToTransitionSystem(MachineState state, ExplorationStateFlags flags)
		{
			if (transitionSystemBuilder != null)
			{
				lock (explorer.AbortLock)
				{
					transitionSystemBuilder.AddState(state, flags);
				}
			}
			if (state.Control.Kind == ControlStateKind.Error)
			{
				ReportErrorState(state);
			}
		}

		protected void ReportErrorState(MachineState state)
		{
			if (machineExplorer.CurrentErrorStateCount == 1)
			{
				eventAdapter.DiagMessage(DiagnosisKind.Error, "reached error state(s) (see graph/output window)", null);
			}
			eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "*** Reached error state ***");
			eventAdapter.ProgressMessage(VerbosityLevel.Minimal, state.Control.Description);
		}

		protected void OnStep(object sender, StepEventArgs args)
		{
			AddStepToTransitionSystem(args.Step, args.SubsumptionRelatedState, args.SubsumptionRelationKind);
			if (suspendOnNextStep)
			{
				args.SuspendExploration = true;
				suspendOnNextStep = false;
			}
		}

		protected void AddStepToTransitionSystem(ExplorationStep step, MachineState? subsumptionRelatedState, SubsumptionResult subsumptionRelationKind)
		{
			MachineState intermediateTargetState = step.IntermediateTargetState;
			if (intermediateTargetState.Control.Kind == ControlStateKind.Error)
			{
				ReportErrorStep(step);
			}
			if (transitionSystemBuilder != null)
			{
				lock (explorer.AbortLock)
				{
					transitionSystemBuilder.AddStep(step, subsumptionRelatedState, subsumptionRelationKind);
				}
			}
		}

		protected void ReportErrorStep(ExplorationStep step)
		{
			if (machineExplorer.CurrentErrorStateCount == 1)
			{
				eventAdapter.DiagMessage(DiagnosisKind.Error, "reached error state(s) (see graph/output window)", null);
			}
			eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "*** Reached error state ***");
			eventAdapter.ProgressMessage(VerbosityLevel.Minimal, step.TargetState.Control.Description);
			if (options.DumpErrorPaths)
			{
				eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Begin error path");
				ExplorationStep[] array = machineExplorer.DumpCurrentPath();
				eventAdapter.Log("error path: ");
				for (int i = 0; i < array.Length; i++)
				{
					eventAdapter.ProgressMessage(VerbosityLevel.Minimal, string.Format("  {0}. {1}", i + 1, array[i].Action.ToString(termDescriptionContext)));
				}
				eventAdapter.ProgressMessage(VerbosityLevel.Minimal, string.Format("  {0}. {1}", array.Length + 1, step.Action.ToString(termDescriptionContext)));
				eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "End error path");
			}
		}

		public void SuspendExploration()
		{
			lock (explorer)
			{
				suspendOnNextStep = true;
			}
		}

		protected virtual void RaiseExplorationResultUpdatedEvent(ExplorationResultEventArgs args)
		{
			EventHandler<ExplorationResultEventArgs> explorationResultUpdated = this.ExplorationResultUpdated;
			if (explorationResultUpdated != null)
			{
				explorationResultUpdated(this, args);
			}
		}
	}
}
