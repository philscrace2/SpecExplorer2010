// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.OperatorBase
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.Threading;

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

    protected OperatorBase(
      IMachine machine,
      IConfiguration config,
      EventAdapter eventAdapter,
      ExplorerConfiguration explorerConfig,
      ExplorationOptions options,
      EventWaitHandle workerWaitHandle,
      IRemoteExplorer explorer)
    {
      this.config = config;
      this.host = this.config.Host;
      this.machine = machine;
      this.eventAdapter = eventAdapter;
      this.explorerConfig = explorerConfig;
      this.options = options;
      this.workerWaitHandle = workerWaitHandle;
      this.explorer = explorer;
      this.termDescriptionContext = this.host.GetRequiredService<IBackground>().UserDescription;
    }

    public abstract void Explore();

    protected void OnSuspension(object sender, SuspensionEventArgs args)
    {
      this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job suspended.");
      this.eventAdapter.SwitchState(ExplorationState.SuspendedExploring);
      using (IEnumerator<MachineState> enumerator = args.suspendedStates.GetEnumerator())
      {
        if (enumerator.MoveNext())
          this.continuedState = enumerator.Current;
      }
      this.workerWaitHandle.WaitOne();
      lock (this.explorer)
      {
        if (this.explorer.State != ExplorationState.Exploring)
          return;
        args.Resume(this.continuedState, 0);
      }
    }

    protected void OnState(object sender, StateEventArgs e) => this.AddStateToTransitionSystem(e.State, e.Flags);

    protected void AddStateToTransitionSystem(MachineState state, ExplorationStateFlags flags)
    {
      if (this.transitionSystemBuilder != null)
      {
        lock (this.explorer.AbortLock)
          this.transitionSystemBuilder.AddState(state, flags);
      }
      if (state.Control.Kind != ControlStateKind.Error)
        return;
      this.ReportErrorState(state);
    }

    protected void ReportErrorState(MachineState state)
    {
      if (this.machineExplorer.CurrentErrorStateCount == 1)
        this.eventAdapter.DiagMessage(DiagnosisKind.Error, "reached error state(s) (see graph/output window)", (object) null);
      this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "*** Reached error state ***");
      this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, state.Control.Description);
    }

    protected void OnStep(object sender, StepEventArgs args)
    {
      this.AddStepToTransitionSystem(args.Step, args.SubsumptionRelatedState, args.SubsumptionRelationKind);
      if (!this.suspendOnNextStep)
        return;
      args.SuspendExploration = true;
      this.suspendOnNextStep = false;
    }

    protected void AddStepToTransitionSystem(
      ExplorationStep step,
      MachineState? subsumptionRelatedState,
      SubsumptionResult subsumptionRelationKind)
    {
      if (step.IntermediateTargetState.Control.Kind == ControlStateKind.Error)
        this.ReportErrorStep(step);
      if (this.transitionSystemBuilder == null)
        return;
      lock (this.explorer.AbortLock)
        this.transitionSystemBuilder.AddStep(step, subsumptionRelatedState, subsumptionRelationKind);
    }

    protected void ReportErrorStep(ExplorationStep step)
    {
      if (this.machineExplorer.CurrentErrorStateCount == 1)
        this.eventAdapter.DiagMessage(DiagnosisKind.Error, "reached error state(s) (see graph/output window)", (object) null);
      this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "*** Reached error state ***");
      this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, step.TargetState.Control.Description);
      if (!this.options.DumpErrorPaths)
        return;
      this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Begin error path");
      ExplorationStep[] explorationStepArray = this.machineExplorer.DumpCurrentPath();
      this.eventAdapter.Log("error path: ");
      for (int index = 0; index < explorationStepArray.Length; ++index)
        this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, string.Format("  {0}. {1}", (object) (index + 1), (object) explorationStepArray[index].Action.ToString(this.termDescriptionContext)));
      this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, string.Format("  {0}. {1}", (object) (explorationStepArray.Length + 1), (object) step.Action.ToString(this.termDescriptionContext)));
      this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "End error path");
    }

    public void SuspendExploration()
    {
      lock (this.explorer)
        this.suspendOnNextStep = true;
    }

    public ExplorationResult ExplorationResult { get; protected set; }

    public event EventHandler<ExplorationResultEventArgs> ExplorationResultUpdated;

    protected virtual void RaiseExplorationResultUpdatedEvent(ExplorationResultEventArgs args)
    {
      EventHandler<ExplorationResultEventArgs> explorationResultUpdated = this.ExplorationResultUpdated;
      if (explorationResultUpdated == null)
        return;
      explorationResultUpdated((object) this, args);
    }
  }
}
