// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.TestingOperator
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Microsoft.SpecExplorer
{
  internal class TestingOperator : OperatorBase, IDisposable
  {
    private StreamWriter testRunLogSink;
    private IMachineTestExplorer testExplorer;
    private bool disposed;

    internal TestingOperator(
      IMachine machine,
      IConfiguration config,
      EventAdapter eventAdapter,
      ExplorerConfiguration explorerConfig,
      ExplorationOptions options,
      EventWaitHandle workerWaitHandle,
      IRemoteExplorer explorer)
      : base(machine, config, eventAdapter, explorerConfig, options, workerWaitHandle, explorer)
    {
    }

    public override void Explore()
    {
      this.eventAdapter.SwitchState(ExplorationState.Exploring);
      this.host.GetRequiredService<IStateProvider>().RandomSeed = this.machine.Configuration.OptionSet.GetOptions<ExplorationOptions>().RandomSeed;
      this.CreateMachineTestExplorer();
      this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job started.");
      this.lastStatisticsTime = DateTime.Now;
      this.ShowStatistics(new TestingStatistics());
      this.machineExplorer.Explore(this.machine);
      this.testExplorer.Statistics.Finished = true;
      this.ShowStatistics(this.testExplorer.Statistics);
      if (this.testRunLogSink != null)
      {
        this.testRunLogSink.Close();
        this.testRunLogSink = (StreamWriter) null;
      }
      this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job finished.");
      this.eventAdapter.SwitchState(ExplorationState.FinishedExploring);
    }

    private void CreateMachineTestExplorer()
    {
      IMachineTestExplorerProvider requiredService = this.host.GetRequiredService<IMachineTestExplorerProvider>();
      string path = Path.Combine(this.explorerConfig.OutputDirectory, this.explorerConfig.Machine + ".log");
      if (!Directory.Exists(this.explorerConfig.OutputDirectory))
        Directory.CreateDirectory(this.explorerConfig.OutputDirectory);
      this.testRunLogSink = new StreamWriter(path, false);
      this.testRunLogSink.AutoFlush = true;
      ITestStrategy strategy;
      switch (this.explorerConfig.ExplorationMode)
      {
        case ExplorationMode.OnlineTesting:
          strategy = requiredService.CreateStandardTestStrategy(this.config, (TextWriter) this.testRunLogSink);
          break;
        case ExplorationMode.OnlineTestingReplay:
          IList<ReplayStep> replaySteps = new ReplayStepsBuilder(this.host).CreateReplaySteps(this.explorerConfig.ReplayResultPath);
          strategy = requiredService.CreateReplayTestStrategy(this.config, (IEnumerable<ReplayStep>) replaySteps, (TextWriter) this.testRunLogSink);
          break;
        default:
          throw new InvalidOperationException("no explorer defined");
      }
      this.testExplorer = requiredService.CreateTestExplorer(requiredService.CreateStandardTestHarness(this.config), strategy, (TextWriter) this.testRunLogSink);
      this.testExplorer.OnTestingStatisticsChanged += (EventHandler<TestingStatisticsEventArgs>) ((sender, args) => this.ShowStatistics(args.Statistics));
      this.testExplorer.StartTestCase += new EventHandler<TestCaseStartedEventArgs>(this.OnStartTestCase);
      this.testExplorer.FinishTestCase += new EventHandler<TestCaseFinishedEventArgs>(this.OnFinishTestCase);
      this.machineExplorer = (IActionMachineExplorer) this.testExplorer;
      this.machineExplorer.OnSuspension += new EventHandler<SuspensionEventArgs>(((OperatorBase) this).OnSuspension);
      this.machineExplorer.OnStep += new EventHandler<StepEventArgs>(((OperatorBase) this).OnStep);
      this.machineExplorer.OnState += new EventHandler<StateEventArgs>(((OperatorBase) this).OnState);
    }

    private void OnFinishTestCase(object sender, TestCaseFinishedEventArgs e)
    {
      this.eventAdapter.ShowTestCaseFinishedProgress(e);
      ExplorationResult explorationResult = this.transitionSystemBuilder.BuildTransitionSystem(this.options.OnTheFlySaveState);
      explorationResult.Extensions.IgnoreSignature = this.machine.AlwaysReexplore;
      this.ExplorationResult = explorationResult;
      if (!e.Result.ShouldSaveTestResult(this.options.OnTheFlySaveExperimentTrace))
        return;
      this.RaiseExplorationResultUpdatedEvent(new ExplorationResultEventArgs(this.ExplorationResult));
    }

    private void OnStartTestCase(object sender, TestCaseStartedEventArgs e) => this.transitionSystemBuilder = new TransitionSystemBuilder(e.TestCaseName, this.machine.Configuration, this.host, this.eventAdapter);

    private void ShowStatistics(TestingStatistics statistics) => this.eventAdapter.ShowStatistics(statistics);

    public void Dispose()
    {
      if (this.disposed)
        return;
      if (this.testRunLogSink != null)
      {
        this.testRunLogSink.Close();
        this.testRunLogSink = (StreamWriter) null;
      }
      this.disposed = true;
    }
  }
}
