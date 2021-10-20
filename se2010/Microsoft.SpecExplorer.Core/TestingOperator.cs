using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	internal class TestingOperator : OperatorBase, IDisposable
	{
		private StreamWriter testRunLogSink;

		private IMachineTestExplorer testExplorer;

		private bool disposed;

		internal TestingOperator(IMachine machine, IConfiguration config, EventAdapter eventAdapter, ExplorerConfiguration explorerConfig, ExplorationOptions options, EventWaitHandle workerWaitHandle, IRemoteExplorer explorer)
			: base(machine, config, eventAdapter, explorerConfig, options, workerWaitHandle, explorer)
		{
		}

		public override void Explore()
		{
			eventAdapter.SwitchState(ExplorationState.Exploring);
			host.GetRequiredService<IStateProvider>().RandomSeed = machine.Configuration.OptionSet.GetOptions<ExplorationOptions>().RandomSeed;
			CreateMachineTestExplorer();
			eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job started.");
			lastStatisticsTime = DateTime.Now;
			ShowStatistics(new TestingStatistics());
			machineExplorer.Explore(machine);
			testExplorer.Statistics.Finished = true;
			ShowStatistics(testExplorer.Statistics);
			if (testRunLogSink != null)
			{
				testRunLogSink.Close();
				testRunLogSink = null;
			}
			eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job finished.");
			eventAdapter.SwitchState(ExplorationState.FinishedExploring);
		}

		private void CreateMachineTestExplorer()
		{
			IMachineTestExplorerProvider requiredService = host.GetRequiredService<IMachineTestExplorerProvider>();
			string path = Path.Combine(explorerConfig.OutputDirectory, explorerConfig.Machine + ".log");
			if (!Directory.Exists(explorerConfig.OutputDirectory))
			{
				Directory.CreateDirectory(explorerConfig.OutputDirectory);
			}
			testRunLogSink = new StreamWriter(path, false);
			testRunLogSink.AutoFlush = true;
			ITestStrategy testStrategy = null;
			switch (explorerConfig.ExplorationMode)
			{
			case ExplorationMode.OnlineTesting:
				testStrategy = requiredService.CreateStandardTestStrategy(config, testRunLogSink);
				break;
			case ExplorationMode.OnlineTestingReplay:
			{
				ReplayStepsBuilder replayStepsBuilder = new ReplayStepsBuilder(host);
				IList<ReplayStep> replaySteps = replayStepsBuilder.CreateReplaySteps(explorerConfig.ReplayResultPath);
				testStrategy = requiredService.CreateReplayTestStrategy(config, replaySteps, testRunLogSink);
				break;
			}
			default:
				throw new InvalidOperationException("no explorer defined");
			}
			testExplorer = requiredService.CreateTestExplorer(requiredService.CreateStandardTestHarness(config), testStrategy, testRunLogSink);
			testExplorer.OnTestingStatisticsChanged += delegate(object sender, TestingStatisticsEventArgs args)
			{
				ShowStatistics(args.Statistics);
			};
			testExplorer.StartTestCase += OnStartTestCase;
			testExplorer.FinishTestCase += OnFinishTestCase;
			machineExplorer = testExplorer;
			machineExplorer.OnSuspension += base.OnSuspension;
			machineExplorer.OnStep += base.OnStep;
			machineExplorer.OnState += base.OnState;
		}

		private void OnFinishTestCase(object sender, TestCaseFinishedEventArgs e)
		{
			eventAdapter.ShowTestCaseFinishedProgress(e);
			TransitionSystemBuilder obj = transitionSystemBuilder;
			bool onTheFlySaveState = options.OnTheFlySaveState;
			ExplorationResult explorationResult = obj.BuildTransitionSystem(onTheFlySaveState);
			explorationResult.Extensions.IgnoreSignature = machine.AlwaysReexplore;
			base.ExplorationResult = explorationResult;
			if (e.Result.ShouldSaveTestResult(options.OnTheFlySaveExperimentTrace))
			{
				RaiseExplorationResultUpdatedEvent(new ExplorationResultEventArgs(base.ExplorationResult));
			}
		}

		private void OnStartTestCase(object sender, TestCaseStartedEventArgs e)
		{
			transitionSystemBuilder = new TransitionSystemBuilder(e.TestCaseName, machine.Configuration, host, eventAdapter);
		}

		private void ShowStatistics(TestingStatistics statistics)
		{
			eventAdapter.ShowStatistics(statistics);
		}

		public void Dispose()
		{
			if (!disposed)
			{
				if (testRunLogSink != null)
				{
					testRunLogSink.Close();
					testRunLogSink = null;
				}
				disposed = true;
			}
		}
	}
}
