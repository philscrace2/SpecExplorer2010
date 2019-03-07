// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ExplorationWorker
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.VS;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Xrt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.SpecExplorer
{
  internal class ExplorationWorker
  {
    private Dictionary<string, ExplorationWorker.ProjectUnit> compiledProjects = new Dictionary<string, ExplorationWorker.ProjectUnit>();
    private List<MachineExecuteItem> sucessfullyExecutedMachines = new List<MachineExecuteItem>();
    private List<MachineExecuteItem> failedExecutedMachines = new List<MachineExecuteItem>();
    private List<MachineExecuteItem> abortedExecutedMachines = new List<MachineExecuteItem>();
    private List<MachineExecuteItem> ignoredExecutedMachines = new List<MachineExecuteItem>();
    private const int TimerPeriod = 1000;
    private const string htmlNewLineString = "<br/>";
    private SpecExplorerPackage package;
    private IVsFileChangeEx vsFileChangeEx;
    private ExplorationManagerControl control;
    private IList<Machine> machines;
    private bool reExplore;
    private DateTime startTime;
    private DateTime startExecuteDate;
    private DateTime endExecuteDate;
    private IExplorer explorer;
    private Timer progressTimer;
    private ExplorationStatistics progressData;
    private Machine runningMachine;
    private MachineExecuteItem runningMachineExecuteItem;
    private PostProcessorHelper postProcessorHelper;
    private TaskTypes taskType;
    private IEnumerable<string> postProcessors;
    private string replayFilePath;
    private bool needSaveExplorationResult;
    private string explorationResultFilePath;
    private string onlineTestingLogfilePath;

    internal ExplorationWorker(
      SpecExplorerPackage package,
      ExplorationManagerControl control,
      IList<Machine> machines,
      bool reExplore,
      IVsFileChangeEx vsFileChangeEx,
      TaskTypes taskType,
      IEnumerable<string> postProcessors,
      string replayFilePath)
    {
      this.machines = machines;
      this.control = control;
      this.package = package;
      this.reExplore = reExplore;
      this.vsFileChangeEx = vsFileChangeEx;
      this.taskType = taskType;
      this.postProcessors = postProcessors;
      this.replayFilePath = replayFilePath;
    }

    internal void Execute()
    {
      this.startExecuteDate = DateTime.Now;
      this.PrepareProjects();
      using (IEnumerator<Machine> enumerator = ((IEnumerable<Machine>) this.machines).GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          Machine current = enumerator.Current;
          this.needSaveExplorationResult = false;
          this.runningMachine = current;
          this.runningMachineExecuteItem = new MachineExecuteItem()
          {
            Project = current.Container.Name,
            Machine = current.Name,
            Details = string.Empty
          };
          if (this.taskType == TaskTypes.GeneratingTestCode && string.Compare("true", current.TestEnabled) != 0)
          {
            this.AppendRuningMachineSummary("Machine is not testable.");
            this.HandleIgnoredMachine(this.runningMachineExecuteItem);
          }
          else
            this.ExecuteMachine(current);
          this.runningMachine = (Machine) null;
          this.runningMachineExecuteItem = (MachineExecuteItem) null;
        }
      }
    }

    internal void Cleanup(bool isAborted)
    {
      this.endExecuteDate = DateTime.Now;
      this.progressData = (ExplorationStatistics) null;
      if (this.progressTimer != null)
      {
        this.progressTimer.Dispose();
        this.progressTimer = (Timer) null;
      }
      if (this.explorer != null)
      {
        this.explorer.Abort();
        ExplorationResult explorationResult = this.explorer.ExplorationResult;
        if (this.needSaveExplorationResult)
        {
          if (explorationResult != null && !string.IsNullOrEmpty(this.explorationResultFilePath))
          {
            if (!this.SaveExplorationResult(explorationResult, this.explorationResultFilePath, false))
              this.explorationResultFilePath = (string) null;
          }
          else
            this.explorationResultFilePath = (string) null;
        }
        ((IDisposable) this.explorer).Dispose();
        this.explorer = (IExplorer) null;
      }
      string message = string.Empty;
      if (((ICollection<Machine>) this.machines).Count > 1)
      {
        using (IEnumerator<Machine> enumerator = ((IEnumerable<Machine>) this.machines).GetEnumerator())
        {
          while (((IEnumerator) enumerator).MoveNext())
          {
            Machine m = enumerator.Current;
            if (!this.sucessfullyExecutedMachines.Any<MachineExecuteItem>((Func<MachineExecuteItem, bool>) (item =>
            {
              if (item.Project == m.Container.Name)
                return item.Machine == m.Name;
              return false;
            })) && !this.ignoredExecutedMachines.Any<MachineExecuteItem>((Func<MachineExecuteItem, bool>) (item =>
            {
              if (item.Project == m.Container.Name)
                return item.Machine == m.Name;
              return false;
            })) && !this.failedExecutedMachines.Any<MachineExecuteItem>((Func<MachineExecuteItem, bool>) (item =>
            {
              if (item.Project == m.Container.Name)
                return item.Machine == m.Name;
              return false;
            })))
            {
              if (m == this.runningMachine)
              {
                this.AppendRuningMachineSummary(Resources.ExplorationAborted);
                this.HandleAbortedToExecuteMachine(this.runningMachineExecuteItem);
              }
              else
                this.HandleAbortedToExecuteMachine(new MachineExecuteItem()
                {
                  Project = m.Container.Name,
                  Machine = m.Name,
                  Details = "Not Started."
                });
            }
          }
        }
        string path = this.SaveSummary();
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
          VsShellUtilities.OpenDocument((IServiceProvider) this.package, path);
        message = isAborted ? Resources.ExecutionAborted : Resources.ExecutionFinished;
      }
      else
      {
        switch (this.taskType)
        {
          case TaskTypes.Exploring:
            if (isAborted)
              message = Resources.ExplorationAborted;
            if (!string.IsNullOrEmpty(this.explorationResultFilePath) && File.Exists(this.explorationResultFilePath))
            {
              if (this.vsFileChangeEx != null)
                this.vsFileChangeEx.SyncFile(this.explorationResultFilePath);
              VsShellUtilities.OpenDocument((IServiceProvider) this.package, this.explorationResultFilePath);
              break;
            }
            break;
          case TaskTypes.GeneratingTestCode:
            if (isAborted)
            {
              message = Resources.TestCodeGenerationAborted;
              break;
            }
            break;
          case TaskTypes.RunningPostProcessors:
            if (isAborted)
            {
              message = string.Format(Resources.PostProcessorAborted, (object) this.postProcessorHelper.CurrentPostProcesser);
              break;
            }
            break;
          case TaskTypes.OnTheFlyTesting:
            message = isAborted ? Resources.OnTheFlyTestAborted : Resources.OnTheFlyTestFinished;
            if (!string.IsNullOrEmpty(this.onlineTestingLogfilePath) && File.Exists(this.onlineTestingLogfilePath))
            {
              VsShellUtilities.OpenDocument((IServiceProvider) this.package, this.onlineTestingLogfilePath);
              break;
            }
            break;
          case TaskTypes.OnTheFlyReplayTest:
            message = isAborted ? Resources.OnTheFlyReplayAborted : Resources.OnTheFlyReplayFinished;
            if (!string.IsNullOrEmpty(this.onlineTestingLogfilePath) && File.Exists(this.onlineTestingLogfilePath))
            {
              VsShellUtilities.OpenDocument((IServiceProvider) this.package, this.onlineTestingLogfilePath);
              break;
            }
            break;
          default:
            throw new InvalidOperationException("Invalid task type.");
        }
      }
      if (!string.IsNullOrEmpty(message))
      {
        this.control.ProgressMessage(message);
        this.package.ProgressMessage((VerbosityLevel) 0, message);
      }
      if (this.package.ErrorList.Tasks.Count <= 0)
        return;
      this.package.MakeErrorListVisible();
    }

    private void ExecuteMachine(Machine machine)
    {
      ExplorationWorker.ProjectUnit projectUnit;
      if (!this.compiledProjects.TryGetValue(machine.Container.UniqueName, out projectUnit))
      {
        string str = string.Format(Resources.SkippingNoProjectMachineFormat, (object) machine.Name);
        this.control.ProgressMessage(str);
        this.package.ProgressMessage((VerbosityLevel) 0, str);
        this.AppendRuningMachineSummary(str);
        this.HandleFailedToExecuteMachine(this.runningMachineExecuteItem);
      }
      else if (projectUnit.BuildFailed)
      {
        string str = string.Format(Resources.SkippingFailedProjectMachineFormat, (object) machine.Name, (object) projectUnit.Project[]);
        this.control.ProgressMessage(str);
        this.package.ProgressMessage((VerbosityLevel) 0, str);
        this.AppendRuningMachineSummary(str);
        this.HandleFailedToExecuteMachine(this.runningMachineExecuteItem);
      }
      else
      {
        switch (this.taskType)
        {
          case TaskTypes.Exploring:
          case TaskTypes.GeneratingTestCode:
          case TaskTypes.RunningPostProcessors:
            this.ExecuteExploration(machine, projectUnit);
            break;
          case TaskTypes.OnTheFlyTesting:
          case TaskTypes.OnTheFlyReplayTest:
            this.ExecuteOnTheFlyTest(machine, projectUnit);
            break;
        }
      }
    }

    private void ExecuteExploration(Machine machine, ExplorationWorker.ProjectUnit projectUnit)
    {
      string str1 = Path.Combine(Path.GetDirectoryName(projectUnit.Project.FullName), "ExplorationResults");
      this.explorationResultFilePath = Path.Combine(str1, machine.Name + ".seexpl");
      if (!this.reExplore && !ExplorationUtility.NeedsReExploration(projectUnit.LatestStamp, this.explorationResultFilePath))
      {
        string str2 = string.Format(Resources.MachineResultUpToDateFormat, (object) machine.Name);
        this.control.ProgressMessage(str2);
        this.package.ProgressMessage((VerbosityLevel) 0, str2);
        this.AppendRuningMachineSummary(str2);
        TransitionSystem transitionSystem;
        try
        {
          transitionSystem = new ExplorationResultLoader(this.explorationResultFilePath).LoadTransitionSystem();
          this.package.ProgressMessage((VerbosityLevel) 0, string.Format("Loaded exploration result from '{0}'.", (object) this.explorationResultFilePath));
          this.AppendRuningMachineSummary(string.Format("Loaded exploration result from <a href=\"_file:///{0}\">'{0}'</a>.", (object) this.explorationResultFilePath));
        }
        catch (ExplorationResultLoadingException ex)
        {
          this.control.ProgressMessage(string.Format("Failed to load exploration result from {0}.", (object) this.explorationResultFilePath));
          this.package.ProgressMessage((VerbosityLevel) 0, string.Format("Failed to load exploration result from {0}:\n{1}", (object) this.explorationResultFilePath, (object) ex.Message));
          this.AppendRuningMachineSummary(string.Format("Failed to load exploration result from <a href=\"_file:///{0}\">{0}</a><br/>{1}", (object) this.explorationResultFilePath, (object) ex.Message));
          this.HandleFailedToExecuteMachine(this.runningMachineExecuteItem);
          return;
        }
        this.PostProcessingExplorationResult(machine, this.explorationResultFilePath, projectUnit, transitionSystem);
      }
      else
      {
        ICordDesignTimeManager designTimeForProject = this.package.GetDesignTimeForProject(projectUnit.Project);
        this.InitializeExplorer(projectUnit.References, projectUnit.Scripts, str1, machine.Name, designTimeForProject.GetMachineSwitches(machine.Name), (string) null);
        this.startTime = DateTime.Now;
        this.progressTimer = new Timer(new TimerCallback(this.UpdateExplorationProgress), (object) null, 1000, 1000);
        string message1 = string.Format(Resources.ValidatingMachineFormat, (object) machine.Name);
        this.control.ProgressMessage(message1);
        this.package.ProgressMessage((VerbosityLevel) 0, message1);
        this.explorer.StartBuilding().WaitOne();
        if (this.explorer.State == 3)
        {
          string message2 = string.Format(Resources.ValidationMachineSucceededFormat, (object) machine.Name);
          this.control.ProgressMessage(message2);
          this.package.ProgressMessage((VerbosityLevel) 0, message2);
          string message3 = string.Format(Resources.ExploringMachineFormat, (object) machine.Name);
          this.control.ProgressMessage(message3);
          this.package.ProgressMessage((VerbosityLevel) 0, message3);
          this.progressData = new ExplorationStatistics();
          this.needSaveExplorationResult = true;
          this.explorer.StartExploration().WaitOne();
          if (this.explorer.State == 6)
          {
            if (this.progressTimer != null)
            {
              this.progressTimer.Dispose();
              this.progressTimer = (Timer) null;
            }
            string str2 = string.Format(Resources.ExplorationMachineSucceededFormat, (object) machine.Name);
            this.control.ProgressMessage(str2);
            this.package.ProgressMessage((VerbosityLevel) 0, str2);
            this.AppendRuningMachineSummary(str2);
            this.UpdateExplorationProgress((object) null);
            ExplorationResult explorationResult = this.explorer.ExplorationResult;
            if (explorationResult != null)
            {
              explorationResult.Extensions.Signature = projectUnit.LatestStamp;
              if (this.SaveExplorationResult(explorationResult, this.explorationResultFilePath, false))
                this.PostProcessingExplorationResult(machine, this.explorationResultFilePath, projectUnit, explorationResult.TransitionSystem);
              else
                this.HandleFailedToExecuteMachine(this.runningMachineExecuteItem);
              this.needSaveExplorationResult = false;
            }
            else
            {
              string str3 = string.Format(Resources.InvalidMachineExplorationResultFormat, (object) machine.Name);
              this.control.ProgressMessage(str3);
              this.package.ProgressMessage((VerbosityLevel) 0, str3);
              this.AppendRuningMachineSummary(str3);
              this.explorationResultFilePath = (string) null;
              this.HandleFailedToExecuteMachine(this.runningMachineExecuteItem);
            }
          }
          else
          {
            string str2 = string.Format(Resources.ExplorationMachineFailedFormat, (object) machine.Name);
            this.control.ProgressMessage(str2);
            this.package.ProgressMessage((VerbosityLevel) 0, str2);
            this.AppendRuningMachineSummary(str2);
            this.explorationResultFilePath = (string) null;
            this.HandleFailedToExecuteMachine(this.runningMachineExecuteItem);
          }
        }
        else
        {
          string str2 = string.Format(Resources.ValidationMachineFailedFormat, (object) machine.Name);
          this.control.ProgressMessage(str2);
          this.package.ProgressMessage((VerbosityLevel) 0, str2);
          this.AppendRuningMachineSummary(str2);
          this.explorationResultFilePath = (string) null;
          this.HandleFailedToExecuteMachine(this.runningMachineExecuteItem);
        }
        if (this.explorer != null)
        {
          ((IDisposable) this.explorer).Dispose();
          this.explorer = (IExplorer) null;
        }
        if (this.progressTimer == null)
          return;
        this.progressTimer.Dispose();
        this.progressTimer = (Timer) null;
      }
    }

    private void ExecuteOnTheFlyTest(Machine machine, ExplorationWorker.ProjectUnit projectUnit)
    {
      string str1 = Path.Combine(Path.GetDirectoryName(projectUnit.Project.FullName), "TestResults", machine.Name, DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss"));
      this.onlineTestingLogfilePath = (string) null;
      ICordDesignTimeManager designTimeForProject = this.package.GetDesignTimeForProject(projectUnit.Project);
      this.InitializeExplorer(projectUnit.References, projectUnit.Scripts, str1, machine.Name, designTimeForProject.GetMachineSwitches(machine.Name), this.replayFilePath);
      string message1 = string.Format(Resources.ValidatingMachineFormat, (object) machine.Name);
      this.control.ProgressMessage(message1);
      this.package.ProgressMessage((VerbosityLevel) 0, message1);
      this.explorer.StartBuilding().WaitOne();
      if (this.explorer.State == 3)
      {
        string message2 = string.Format(Resources.ValidationMachineSucceededFormat, (object) machine.Name);
        this.control.ProgressMessage(message2);
        this.package.ProgressMessage((VerbosityLevel) 0, message2);
        string message3 = string.Format(Resources.ExploringMachineFormat, (object) machine.Name);
        this.control.ProgressMessage(message3);
        this.package.ProgressMessage((VerbosityLevel) 0, message3);
        this.progressData = new ExplorationStatistics();
        this.onlineTestingLogfilePath = Path.Combine(str1, machine.Name + ".log");
        this.explorer.StartExploration().WaitOne();
        this.AppendRuningMachineSummary(string.Format("The log file is saved to: <a href=\"_file:///{0}\">{0}</a>", (object) this.onlineTestingLogfilePath));
        this.HandleFinishedToExecuteMachine(this.runningMachineExecuteItem);
      }
      else
      {
        string str2 = string.Format(Resources.ValidationMachineFailedFormat, (object) machine.Name);
        this.control.ProgressMessage(str2);
        this.package.ProgressMessage((VerbosityLevel) 0, str2);
        this.AppendRuningMachineSummary(str2);
        this.HandleFailedToExecuteMachine(this.runningMachineExecuteItem);
      }
      if (this.explorer == null)
        return;
      ((IDisposable) this.explorer).Dispose();
      this.explorer = (IExplorer) null;
    }

    private void InitializeExplorer(
      ICollection<string> references,
      List<string> scripts,
      string outputDir,
      string machineName,
      IDictionary<string, string> machineSwitches,
      string replayPath)
    {
      ExplorationMode explorationMode;
      switch (this.taskType)
      {
        case TaskTypes.OnTheFlyTesting:
          explorationMode = (ExplorationMode) 1;
          break;
        case TaskTypes.OnTheFlyReplayTest:
          explorationMode = (ExplorationMode) 2;
          break;
        default:
          explorationMode = (ExplorationMode) 0;
          break;
      }
      this.explorer = this.package.Session.CreateExplorer(references, (ICollection<string>) scripts, explorationMode, machineName, outputDir, replayPath, new int?(), machineSwitches, false);
      switch ((int) explorationMode)
      {
        case 0:
          this.explorer.add_ExplorationStatisticsProgress((EventHandler<ExplorationStatisticsEventArgs>) ((sender, args) => this.progressData = args.Statistics));
          break;
        case 1:
        case 2:
          this.explorer.add_ExplorationResultUpdated((EventHandler<ExplorationResultEventArgs>) ((sender, args) =>
          {
            string resultPath = Path.Combine(outputDir, args.ExplorationResult.TransitionSystem.Name + ".seexpl");
            this.SaveExplorationResult(args.ExplorationResult, resultPath, true);
          }));
          break;
      }
    }

    private bool SaveExplorationResult(
      ExplorationResult explorationResult,
      string resultPath,
      bool isTestTrace)
    {
      if (explorationResult == null || explorationResult.TransitionSystem == null)
        return false;
      string directoryName = Path.GetDirectoryName(resultPath);
      if (!Directory.Exists(directoryName))
      {
        try
        {
          Directory.CreateDirectory(directoryName);
        }
        catch (IOException ex)
        {
          return false;
        }
        catch (UnauthorizedAccessException ex)
        {
          return false;
        }
        catch (ArgumentException ex)
        {
          return false;
        }
        catch (NotSupportedException ex)
        {
          return false;
        }
      }
      this.package.Assert(!string.IsNullOrEmpty(explorationResult.TransitionSystem.Name));
      try
      {
        ExplorationResultPacker explorationResultPacker = new ExplorationResultPacker(explorationResult);
        FileInfo fileInfo = new FileInfo(resultPath);
        if (fileInfo.Exists && fileInfo.IsReadOnly)
        {
          MessageResult messageResult = this.package.DecisionDialog(Resources.SpecExplorer, string.Format(Resources.SaveOfReadOnlyFileFormat, (object) resultPath), (MessageButton) 3);
          if (messageResult == 6)
            fileInfo.IsReadOnly = false;
          else if (messageResult != 7)
          {
            this.package.Assert(messageResult == 2, "message result must be CANCEL.");
            if (isTestTrace)
              this.package.ProgressMessage((VerbosityLevel) 0, "On-The-Fly test result is not saved.");
            else
              this.package.ProgressMessage((VerbosityLevel) 0, "Exploration result is not saved.");
            return false;
          }
        }
        explorationResultPacker.Save(resultPath);
        if (isTestTrace)
        {
          this.package.ProgressMessage((VerbosityLevel) 0, string.Format("On-The-Fly test result is saved to {0}.", (object) resultPath));
        }
        else
        {
          this.package.ProgressMessage((VerbosityLevel) 0, string.Format("Exploration result is saved to {0}.", (object) resultPath));
          this.AppendRuningMachineSummary(string.Format("Exploration result is saved to <a href=\"_file:///{0}\">{0}</a>.", (object) resultPath));
        }
        return true;
      }
      catch (Exception ex)
      {
        if (isTestTrace)
        {
          string str = string.Format("Failed to save On-The-Fly test result to file {0}. \n {1}", (object) resultPath, (object) ex.Message);
          this.package.ProgressMessage((VerbosityLevel) 0, str);
          this.AppendRuningMachineSummary(str);
        }
        else
        {
          string str = string.Format("Failed to save exploration result to file {0}. \n {1}", (object) resultPath, (object) ex.Message);
          this.package.ProgressMessage((VerbosityLevel) 0, str);
          this.AppendRuningMachineSummary(str);
        }
        return false;
      }
    }

    private void UpdateExplorationProgress(object obj)
    {
      if (this.progressData == null)
        return;
      string progressDurationFormat = ExtensionMethods.ToProgressDurationFormat(DateTime.Now - this.startTime);
      this.control.ProgressMessage(string.Format("{0} {1} seconds, {2}", this.progressData.Finished ? (object) Resources.ExplorationFinished : (object) Resources.ExplorationInProgress, (object) progressDurationFormat, (object) ((object) this.progressData).ToString()));
      if (!this.progressData.Finished)
        return;
      this.progressData = (ExplorationStatistics) null;
    }

    private void PostProcessingExplorationResult(
      Machine machine,
      string resultPath,
      ExplorationWorker.ProjectUnit projectUnit,
      TransitionSystem transitionSystem = null)
    {
      switch (this.taskType)
      {
        case TaskTypes.Exploring:
          this.AppendRuningMachineSummary(this.GetTransitionSystemStatusString(transitionSystem));
          this.HandleFinishedToExecuteMachine(this.runningMachineExecuteItem);
          break;
        case TaskTypes.GeneratingTestCode:
          if (this.GenerateTestCode(machine.Name, transitionSystem, projectUnit.Project.FileName))
          {
            this.HandleFinishedToExecuteMachine(this.runningMachineExecuteItem);
            break;
          }
          this.HandleFailedToExecuteMachine(this.runningMachineExecuteItem);
          break;
        case TaskTypes.RunningPostProcessors:
          this.ExecutePostProcessors(resultPath, projectUnit.Project.FullName);
          this.HandleFinishedToExecuteMachine(this.runningMachineExecuteItem);
          break;
      }
    }

    private bool GenerateTestCode(
      string machineName,
      TransitionSystem transitionSystem,
      string projectFileName)
    {
      string message1 = string.Format(Resources.TestCodeGenerationInProgressFormat, (object) machineName);
      this.control.ProgressMessage(message1);
      this.package.ProgressMessage((VerbosityLevel) 0, message1);
      if (transitionSystem == null)
      {
        string str1 = string.Format(Resources.InvalidMachineExplorationResultFormat, (object) machineName);
        this.package.DiagMessage((DiagnosisKind) 0, str1, (object) null);
        this.control.ProgressMessage(str1);
        this.package.ProgressMessage((VerbosityLevel) 0, str1);
        this.AppendRuningMachineSummary(str1);
        string str2 = string.Format(Resources.TestCodeGenerationFailedFormat, (object) machineName);
        this.control.ProgressMessage(str2);
        this.package.ProgressMessage((VerbosityLevel) 0, str2);
        this.AppendRuningMachineSummary(str2);
        return false;
      }
      string str3 = transitionSystem.GetSwitch("generatedtestpath");
      this.package.Assert(str3 != null);
      string path2_1 = str3.Replace("\\\\", "\\");
      string fullPath1 = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectFileName), path2_1));
      if (!Directory.Exists(fullPath1))
      {
        try
        {
          Directory.CreateDirectory(fullPath1);
        }
        catch (Exception ex)
        {
          string str1 = string.Format("Failed to create subfolder {0} for storing generated test suite:\n{1}", (object) path2_1, (object) ex.Message);
          this.package.NotificationDialog(Resources.SpecExplorer, str1);
          string str2 = string.Format(Resources.TestCodeGenerationFailedFormat, (object) machineName);
          this.control.ProgressMessage(str2);
          this.package.ProgressMessage((VerbosityLevel) 0, str2);
          this.AppendRuningMachineSummary(str2);
          this.AppendRuningMachineSummary(str1);
          return false;
        }
      }
      string path2_2 = transitionSystem.GetSwitch("generatedtestfile");
      if (string.IsNullOrEmpty(path2_2))
        path2_2 = string.Format("{0}.cs", (object) transitionSystem.Name);
      string fullPath2 = Path.GetFullPath(Path.Combine(fullPath1, path2_2));
      string contents = (!Convert.ToBoolean(transitionSystem.GetSwitch("generatedynamictest")) ? (TestCodeGenerateBase) new StaticTestCodeGenerator(this.package.Session.Host, transitionSystem) : (TestCodeGenerateBase) new DynamicTraversalTestCodeGenerator(this.package.Session.Host, transitionSystem)).Generate(machineName);
      if (!string.IsNullOrEmpty(contents))
      {
        try
        {
          FileInfo fileInfo = new FileInfo(fullPath2);
          if (fileInfo.Exists && fileInfo.IsReadOnly)
          {
            MessageResult messageResult = this.package.DecisionDialog(Resources.SpecExplorer, string.Format(Resources.SaveOfReadOnlyFileFormat, (object) fullPath2), (MessageButton) 3);
            if (messageResult == 6)
              fileInfo.IsReadOnly = false;
            else if (messageResult != 7)
            {
              this.package.Assert(messageResult == 2, "message result must be CANCEL.");
              string str1 = "Generated code is not saved.";
              this.control.ProgressMessage(str1);
              this.package.ProgressMessage((VerbosityLevel) 0, str1);
              this.AppendRuningMachineSummary(str1);
              return false;
            }
          }
          File.WriteAllText(fullPath2, contents);
          if (this.vsFileChangeEx != null)
            this.vsFileChangeEx.SyncFile(fullPath2);
          string message2 = string.Format("Generated code is saved to {0}.", (object) fullPath2);
          this.control.ProgressMessage(message2);
          this.package.ProgressMessage((VerbosityLevel) 0, message2);
          this.AppendRuningMachineSummary(string.Format("Generated code is saved to <a href=\"_file:///{0}\">{0}</a>.", (object) fullPath2));
          return true;
        }
        catch (Exception ex)
        {
          string str1 = string.Format(Resources.TestCodeGenerationFailedFormat, (object) machineName);
          this.control.ProgressMessage(str1);
          this.package.ProgressMessage((VerbosityLevel) 0, str1);
          this.AppendRuningMachineSummary(str1);
          this.AppendRuningMachineSummary(string.Format("Failed to write test suite to file {0}. \n{1}", (object) fullPath2, (object) ex.Message));
          return false;
        }
      }
      else
      {
        string str1 = "Failed to generate test code, please see Error List for detail info.";
        this.control.ProgressMessage(str1);
        this.package.ProgressMessage((VerbosityLevel) 0, str1);
        this.AppendRuningMachineSummary(str1);
        return false;
      }
    }

    private void ExecutePostProcessors(string resultPath, string projectFullName)
    {
      if (this.postProcessors == null || this.postProcessors.Count<string>() <= 0)
        return;
      string str = Path.Combine(this.package.Session.InstallDir, Resources.ExtensionDirectoryName);
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
      dictionary1["WorkingDirectory"] = (object) Path.GetDirectoryName(projectFullName);
      Dictionary<string, Type> source;
      Dictionary<string, string> dictionary2;
      if (!PostProcessorHelper.LoadCustomizedPostProcessingTypes(str, (IHost) this.package, ref source, ref dictionary2))
        return;
      // ISSUE: method pointer
      this.postProcessorHelper = new PostProcessorHelper((IDictionary<string, object>) dictionary1, new ProgressMessageDisplayer((object) this.control, __methodptr(ProgressMessage)));
      this.postProcessorHelper.ExecutePostProcessing(resultPath, source.Where<KeyValuePair<string, Type>>((Func<KeyValuePair<string, Type>, bool>) (item => this.postProcessors.Contains<string>(item.Key))).Select<KeyValuePair<string, Type>, Type>((Func<KeyValuePair<string, Type>, Type>) (p => p.Value)), (IHost) this.package);
      this.postProcessorHelper.Dispose();
    }

    private string GetTransitionSystemStatusString(TransitionSystem transitionSystem)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      foreach (Microsoft.SpecExplorer.ObjectModel.State state in transitionSystem.States)
      {
        if (!state.IsVirtual && (state.RepresentativeState == null || state.RelationKind != StateRelationKind.Equivalent))
        {
          ++num1;
          if ((state.Flags & StateFlags.Error) != StateFlags.None)
            ++num2;
          if ((state.Flags & StateFlags.NonAcceptingEnd) != StateFlags.None)
            ++num3;
          if ((state.Flags & StateFlags.BoundStopped) != StateFlags.None)
            ++num4;
        }
      }
      int num5 = 0;
      HashSet<string> stringSet = new HashSet<string>();
      foreach (Transition transition in transitionSystem.Transitions)
      {
        ++num5;
        foreach (string capturedRequirement in transition.CapturedRequirements)
          stringSet.Add(capturedRequirement);
        foreach (string capturedRequirement in transition.AssumeCapturedRequirements)
          stringSet.Add(capturedRequirement);
      }
      int count = stringSet.Count;
      return string.Format("'{0}' includes {1} states, {2} steps, {3} requirements, {4} errors, {5} non-accepting end states, {6} bounds hit.", (object) transitionSystem.Name, (object) num1, (object) num5, (object) count, (object) num2, (object) num3, (object) num4);
    }

    private string SaveSummary()
    {
      string oldValue1 = "%SUMMARYHEADERBLOCK%";
      string oldValue2 = "%STARTTIMESTAMP%";
      string oldValue3 = "%ENDTIMESTAMP%";
      string oldValue4 = "%TASKTYPEBLOCK%";
      string oldValue5 = "%NUMBEROFFINISHEDEXPLOREDMACHINES%";
      string oldValue6 = "%NUMBEROFFAILEDEXPLOREDMACHINES%";
      string oldValue7 = " %NUMBEROFABORTEDEXPLOREDMACHINES%";
      string oldValue8 = " %NUMBEROFIGNOREDEXPLOREDMACHINES%";
      string oldValue9 = "%SUCCESSFULLYEXPLOREDMACHINEBLOCK%";
      string oldValue10 = "%FAILEDEXPLOREDMACHINEBLOCK%";
      string oldValue11 = "%ABORTEDEXPLOREDMACHINEBLOCK%";
      string oldValue12 = "%IGNOREDEXPLOREDMACHINEBLOCK%";
      string summaryHeaderString;
      string taskTypeString;
      this.GetSummaryHeaderStringAndTaskTypeString(out summaryHeaderString, out taskTypeString);
      string str1 = Path.Combine(Path.GetDirectoryName(this.package.DTE.Solution.FullName), "Summary", summaryHeaderString);
      try
      {
        if (!Directory.Exists(str1))
          Directory.CreateDirectory(str1);
      }
      catch (Exception ex)
      {
        this.package.DecisionDialog(Resources.SpecExplorer, string.Format("Failed to create summary folder: {0}. Summary file cannot be saved.", (object) str1), (MessageButton) 0);
        return (string) null;
      }
      string path = Path.Combine(str1, DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss") + ".sesum");
      string str2 = Resources.SummaryTemplate.Replace(oldValue1, summaryHeaderString);
      CultureInfo cultureInfo = new CultureInfo("en-US");
      string str3 = str2.Replace(oldValue2, this.startExecuteDate.ToString("F", (IFormatProvider) cultureInfo)).Replace(oldValue3, this.endExecuteDate.ToString("F", (IFormatProvider) cultureInfo)).Replace(oldValue4, taskTypeString).Replace(oldValue5, this.sucessfullyExecutedMachines.Count.ToString()).Replace(oldValue7, this.abortedExecutedMachines.Count.ToString()).Replace(oldValue6, this.failedExecutedMachines.Count.ToString()).Replace(oldValue8, this.ignoredExecutedMachines.Count.ToString()).Replace(oldValue9, this.GetHTMLContent(string.Format("Successfully {0} Machine(s):", (object) taskTypeString), this.sucessfullyExecutedMachines)).Replace(oldValue10, this.GetHTMLContent(string.Format("Failed {0} Machine(s):", (object) taskTypeString), this.failedExecutedMachines)).Replace(oldValue11, this.GetHTMLContent(string.Format("Aborted {0} Machine(s):", (object) taskTypeString), this.abortedExecutedMachines)).Replace(oldValue12, this.GetHTMLContent(string.Format("Ignored {0} Machine(s):", (object) taskTypeString), this.ignoredExecutedMachines));
      using (StreamWriter streamWriter = new StreamWriter(path, false))
      {
        try
        {
          streamWriter.WriteLine(str3);
          streamWriter.Flush();
        }
        catch (Exception ex)
        {
          path = (string) null;
        }
      }
      return path;
    }

    private string GetHTMLContent(string title, List<MachineExecuteItem> list)
    {
      if (list == null || list.Count == 0)
        return string.Empty;
      int num1 = 150;
      int num2 = 200;
      string str1 = "Project";
      string str2 = "Machine";
      string str3 = "Details";
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Format("<h2>{0}</h2>", (object) title));
      stringBuilder.AppendLine("<table cellpadding=\"2\" cellspacing=\"0\" width=\"98%\" border=\"1\" class=\"infotable\">");
      stringBuilder.AppendLine("<tr>");
      stringBuilder.AppendLine("<td nowrap=\"1\" class=\"header\" width=\"" + num1.ToString() + "px\">" + str1 + "</td>");
      stringBuilder.AppendLine("<td nowrap=\"1\" class=\"header\" width=\"" + num2.ToString() + "px\">" + str2 + "</td>");
      stringBuilder.AppendLine("<td nowrap=\"1\" class=\"header\">" + str3 + "</td>");
      stringBuilder.AppendLine("</tr>");
      foreach (MachineExecuteItem machineExecuteItem in list)
      {
        stringBuilder.AppendLine("<tr class=\"row\">");
        stringBuilder.AppendLine("<td class=\"content\">" + machineExecuteItem.Project + "</td>");
        stringBuilder.AppendLine("<td class=\"content\">" + machineExecuteItem.Machine + "</td>");
        stringBuilder.AppendLine("<td class=\"content\">" + machineExecuteItem.Details + "</td>");
        stringBuilder.AppendLine("</tr>");
      }
      stringBuilder.AppendLine("</table>");
      return stringBuilder.ToString();
    }

    private void GetSummaryHeaderStringAndTaskTypeString(
      out string summaryHeaderString,
      out string taskTypeString)
    {
      switch (this.taskType)
      {
        case TaskTypes.Exploring:
          summaryHeaderString = "Exploration";
          taskTypeString = "explored ";
          break;
        case TaskTypes.GeneratingTestCode:
          summaryHeaderString = "Test Code Generation";
          taskTypeString = "test code generated";
          break;
        case TaskTypes.RunningPostProcessors:
          summaryHeaderString = "PostProcessing";
          taskTypeString = "post processed";
          break;
        case TaskTypes.OnTheFlyTesting:
          summaryHeaderString = "On-The-Fly Testing";
          taskTypeString = "on-the-fly testing";
          break;
        case TaskTypes.OnTheFlyReplayTest:
          summaryHeaderString = "On-The-Fly Replay Testing";
          taskTypeString = "on-the-fly replay testing";
          break;
        default:
          throw new Exception("Unknown task type.");
      }
    }

    private void PrepareProjects()
    {
      this.package.ClearErrorList();
      using (IEnumerator<Machine> enumerator = ((IEnumerable<Machine>) this.machines).GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          string uniqueName = enumerator.Current.Container.UniqueName;
          if (!this.compiledProjects.ContainsKey(uniqueName))
          {
            Project project = ProjectUtils.GetAllRealProjects(this.package.DTE).FirstOrDefault<Project>((Func<Project, bool>) (p => p.UniqueName == uniqueName));
            if (project != null)
            {
              string message1 = string.Format(Resources.BuildingProjectFormat, (object) project[]);
              this.control.ProgressMessage(message1);
              this.package.ProgressMessage((VerbosityLevel) 0, message1);
              if (this.package.BuildProject(project))
              {
                ICollection<string> strings = this.package.CollectReferences(project);
                List<string> scripts = new List<string>();
                this.package.CollectScripts(scripts, project.ProjectItems);
                ExplorationWorker.ProjectUnit projectUnit = new ExplorationWorker.ProjectUnit()
                {
                  Project = project,
                  References = strings,
                  Scripts = scripts,
                  LatestStamp = ExplorationUtility.GetSourceFilesStamp(strings, (ICollection<string>) scripts),
                  BuildFailed = false
                };
                this.compiledProjects.Add(uniqueName, projectUnit);
                string message2 = string.Format(Resources.BuildProjectSucceededFormat, (object) project[]);
                this.control.ProgressMessage(message2);
                this.package.ProgressMessage((VerbosityLevel) 0, message2);
              }
              else
              {
                string message2 = string.Format(Resources.BuildProjectFailedFormat, (object) project[]);
                this.control.ProgressMessage(message2);
                this.package.ProgressMessage((VerbosityLevel) 0, message2);
                this.compiledProjects.Add(uniqueName, new ExplorationWorker.ProjectUnit()
                {
                  Project = project,
                  BuildFailed = true
                });
              }
            }
          }
        }
      }
    }

    private void HandleIgnoredMachine(MachineExecuteItem executeItem)
    {
      this.ignoredExecutedMachines.Add(executeItem);
    }

    private void HandleFinishedToExecuteMachine(MachineExecuteItem executeItem)
    {
      this.sucessfullyExecutedMachines.Add(executeItem);
    }

    private void HandleFailedToExecuteMachine(MachineExecuteItem executeItem)
    {
      this.failedExecutedMachines.Add(executeItem);
    }

    private void HandleAbortedToExecuteMachine(MachineExecuteItem executeItem)
    {
      this.abortedExecutedMachines.Add(executeItem);
    }

    private void AppendRuningMachineSummary(string text)
    {
      if (this.runningMachineExecuteItem == null)
        return;
      MachineExecuteItem machineExecuteItem = this.runningMachineExecuteItem;
      machineExecuteItem.Details = machineExecuteItem.Details + text + "<br/>";
    }

    internal class ProjectUnit
    {
      internal Project Project { get; set; }

      internal ICollection<string> References { get; set; }

      internal List<string> Scripts { get; set; }

      internal string LatestStamp { get; set; }

      internal bool BuildFailed { get; set; }
    }
  }
}
