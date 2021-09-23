// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.CommandLine.ConsoleHostDriver
// Assembly: SpecExplorer, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7507FD4E-0ABD-4C37-958B-5FF2521D030B
// Assembly location: C:\Users\pls2\OneDrive\source code\SourceCode\spec_explorer\original_files\Spec Explorer 2010\SpecExplorer.exe

using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Microsoft.SpecExplorer.CommandLine
{
  public class ConsoleHostDriver : IDisposable
  {
    private const string GlobalScopeName = "global";
    private const string DefaultExplorationResultFolderName = "ExplorationResults";
    private const string DefaultTestResultFolderName = "TestResults";
    private const string DefaultExtensionsFolderName = "Extensions";
    private const string DefaultTestSuiteFolderName = "TestSuite";
    private CommandLineParser parser;
    private IHost host;
    private List<string> assemblyList = new List<string>();
    private List<string> scriptList = new List<string>();
    private List<string> machineList = new List<string>();
    private Dictionary<string, Type> postProcessorTypeMap;
    private List<Type> userTaskTypes = new List<Type>();
    private string outputDirectory;
    private string replayExplorationResultPath;
    private ISession globalSession;
    private bool disposed;

    public TaskType Task { get; private set; }

    public ConsoleHostDriver(IHost consoleHost, CommandLineParser clParser)
    {
      if (clParser == null)
        throw new ArgumentNullException(nameof (clParser));
      if (consoleHost == null)
        throw new ArgumentNullException(nameof (consoleHost));
      this.parser = clParser;
      this.host = consoleHost;
      this.globalSession = (ISession) new Session(this.host);
    }

    private bool ValidateArgs()
    {
      bool flag1 = true;
      bool flag2 = this.ValidateTaskType() && flag1;
      bool flag3 = this.ValidateInputScriptFiles() && flag2;
      bool flag4 = this.ValidateInputAssemblies() && flag3;
      if (flag4)
      {
        this.InitializeGlobalCordScope();
        switch (this.Task)
        {
          case TaskType.GenerateTests:
            flag4 = this.ValidateMachineNames() && this.ValidateOutputDirectory();
            break;
          case TaskType.PersistExploration:
            flag4 = flag4 && this.ValidateMachineNames() && this.ValidateOutputDirectory();
            break;
          case TaskType.PerformUserTasks:
            flag4 = flag4 && this.ValidateMachineNames() && this.ValidateOutputDirectory() && this.LoadAndValidateUserTasks();
            break;
          case TaskType.DoOnTheFlyTests:
            flag4 = flag4 && this.ValidateMachineNames() && this.ValidateOutputDirectory() && this.ValidateReplayExplorationResultPath();
            break;
          case TaskType.Validate:
            break;
          default:
            throw new InvalidOperationException("Unhandled task type.");
        }
      }
      return flag4;
    }

    private bool ValidateOutputDirectory()
    {
      if (this.Task == TaskType.DoOnTheFlyTests)
      {
        this.outputDirectory = !string.IsNullOrEmpty(this.parser.TestResultPath) ? this.parser.TestResultPath : Path.Combine(Environment.CurrentDirectory, "TestResults");
        this.outputDirectory = Path.Combine(this.outputDirectory, DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss"));
      }
      else
        this.outputDirectory = !string.IsNullOrEmpty(this.parser.ExplorationResultPath) ? this.parser.ExplorationResultPath : Path.Combine(Environment.CurrentDirectory, "ExplorationResults");
      return this.CreateDirectory(this.outputDirectory);
    }

    private bool LoadAndValidateUserTasks()
    {
      string str = Path.Combine(Path.GetDirectoryName(new Uri(typeof (ConsoleHostDriver).Assembly.CodeBase).LocalPath), "Extensions");
      if (!Directory.Exists(str) || !PostProcessorHelper.LoadCustomizedPostProcessingTypes(str, this.host, out this.postProcessorTypeMap, out Dictionary<string, string> _))
        return false;
      if (this.postProcessorTypeMap.Count <= 0)
      {
        this.host.DiagMessage(DiagnosisKind.Error, string.Format("None of the user tasks were found. Please make sure to place the assemblies for your user tasks at {0}.", (object) str), (object) null);
        return false;
      }
      if (this.parser.UserTasks.Count > 0)
      {
        using (IEnumerator<string> enumerator = this.parser.UserTasks.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            string taskName = enumerator.Current;
            IEnumerable<Type> source = this.postProcessorTypeMap.Values.Where<Type>((Func<Type, bool>) (t => t.Name == taskName || t.FullName == taskName));
            if (source != null && source.Count<Type>() > 0)
            {
              foreach (Type type in source)
              {
                if (!this.userTaskTypes.Contains(type))
                {
                  this.userTaskTypes.Add(type);
                  if (this.parser.VerboseOn)
                    this.host.DiagMessage(DiagnosisKind.Hint, string.Format("User task type \"{0}\" is found.", (object) type.FullName), (object) null);
                }
              }
            }
            else
              this.host.DiagMessage(DiagnosisKind.Warning, string.Format("Skipping user task \"{0}\" specified in switch {1}, but not found. Please make sure to place the assemblies for your user tasks at {2}.They must contain a class implementing interface IPostProcessor whose class name exactly matches the ones specified in switch {3}.", (object) taskName, (object) "/UserTasks:".ToSwitchNameDescription(), (object) str, (object) "/UserTasks:".ToSwitchNameDescription()), (object) null);
          }
        }
        if (this.userTaskTypes.Count == 0)
        {
          this.host.DiagMessage(DiagnosisKind.Error, string.Format("None of the user tasks specified in switch {0} were found. Please make sure to place the assemblies for your user tasks at {1}. They must contain a class implementing interface IPostProcessor whose class name exactly matches the ones specified in switch {2}.", (object) "/UserTasks:".ToSwitchNameDescription(), (object) str, (object) "/UserTasks:".ToSwitchNameDescription()), (object) null);
          return false;
        }
      }
      else
        this.userTaskTypes.AddRange((IEnumerable<Type>) this.postProcessorTypeMap.Values);
      return true;
    }

    private bool ValidateMachineNames()
    {
      ICordDesignTimeManager designTimeManager = this.globalSession.Application.GetRequiredService<ICordDesignTimeScopeManager>().GetCordDesignTimeManager("global");
      designTimeManager.ReportError += new EventHandler<ErrorReport>(this.OnReportError);
      if (!designTimeManager.EnsureParsing())
      {
        designTimeManager.ReportError -= new EventHandler<ErrorReport>(this.OnReportError);
        this.host.DiagMessage(DiagnosisKind.Error, "Failed to parse the requested script(s).", (object) null);
        return false;
      }
      designTimeManager.ReportError -= new EventHandler<ErrorReport>(this.OnReportError);
      bool flag1 = true;
      IEnumerable<string> strings = designTimeManager.AllMachines.Select<MachineDefinition, string>((Func<MachineDefinition, string>) (m => m.Name));
      if (strings == null || strings.Count<string>() == 0)
      {
        this.host.DiagMessage(DiagnosisKind.Error, "No machine found in the requested script(s).", (object) null);
        flag1 = false;
      }
      else
      {
        string[] matchedMachineNames = this.GetMatchedMachineNames(this.GetFormattedRegexList(this.parser.MachineNameRegexList), strings);
        if (matchedMachineNames != null && matchedMachineNames.Length > 0)
        {
          foreach (string machine in matchedMachineNames)
          {
            IDictionary<string, string> machineSwitches = designTimeManager.GetMachineSwitches(machine);
            if (machineSwitches != null)
            {
              string strA;
              bool flag2 = machineSwitches.TryGetValue("ForExploration".ToLower(), out strA) && strA != null && string.Compare(strA, "true", StringComparison.CurrentCultureIgnoreCase) == 0;
              switch (this.Task)
              {
                case TaskType.GenerateTests:
                  bool flag3 = machineSwitches.TryGetValue("TestEnabled".ToLower(), out strA) && strA != null && string.Compare(strA, "true", StringComparison.CurrentCultureIgnoreCase) == 0;
                  if (flag2 && flag3)
                  {
                    this.machineList.Add(machine);
                    continue;
                  }
                  if (!flag2)
                  {
                    this.host.DiagMessage(DiagnosisKind.Warning, string.Format("Skipping machine \"{0}\" selected by switch {1}, but not marked for exploration.", (object) machine, (object) "/machines:".ToSwitchNameDescription()), (object) null);
                    continue;
                  }
                  this.host.DiagMessage(DiagnosisKind.Warning, string.Format("Skipping machine \"{0}\" selected by switch {1}, but not marked as test enabled.", (object) machine, (object) "/machines:".ToSwitchNameDescription()), (object) null);
                  continue;
                case TaskType.PersistExploration:
                case TaskType.PerformUserTasks:
                  if (flag2)
                  {
                    this.machineList.Add(machine);
                    continue;
                  }
                  this.host.DiagMessage(DiagnosisKind.Warning, string.Format("Skipping machine \"{0}\" selected by switch {1}, but not marked for exploration.", (object) machine, (object) "/machines:".ToSwitchNameDescription()), (object) null);
                  continue;
                case TaskType.DoOnTheFlyTests:
                  this.machineList.Add(machine);
                  continue;
                default:
                  continue;
              }
            }
          }
          if (this.machineList.Count == 0)
          {
            switch (this.Task)
            {
              case TaskType.GenerateTests:
                flag1 = false;
                this.host.DiagMessage(DiagnosisKind.Error, string.Format("None of the selected machines are both for exploration and test enabled. Please make sure to select one or more machines marked with switch ForExploration = true and TestEnabled = true in your Cord script."), (object) null);
                break;
              case TaskType.PersistExploration:
              case TaskType.PerformUserTasks:
                flag1 = false;
                this.host.DiagMessage(DiagnosisKind.Error, string.Format("None of the selected machines are for exploration. Please make sure to select one or more machines marked with switch ForExploration = true in your Cord script."), (object) null);
                break;
            }
          }
        }
        else
        {
          this.host.DiagMessage(DiagnosisKind.Error, string.Format("None of the expressions provided for command-line switch \"{0}\" match a machine name contained in the requested script(s).", (object) "/machines:".ToSwitchNameDescription()), (object) null);
          flag1 = false;
        }
      }
      return flag1;
    }

    private void OnReportError(object sender, ErrorReport e) => this.host.DiagMessage(e.Kind.ToDiagnosisKind(), e.ToString(), (object) null);

    private bool ValidateInputAssemblies()
    {
      bool flag = true;
      foreach (string assemblyPath in (IEnumerable<string>) this.parser.AssemblyPathList)
      {
        string str = this.LoadFilePath(assemblyPath, true);
        if (str == null)
        {
          this.host.DiagMessage(DiagnosisKind.Error, string.Format("The specified assembly file \"{0}\" cannot be found.", (object) assemblyPath), (object) null);
          flag = false;
        }
        else
          this.assemblyList.Add(str);
      }
      return flag;
    }

    private bool ValidateInputScriptFiles()
    {
      bool flag = true;
      if (this.parser.ScriptPathList.Count == 0)
      {
        this.host.DiagMessage(DiagnosisKind.Error, "At least one Cord script is required.", (object) null);
        flag = false;
      }
      foreach (string scriptPath in (IEnumerable<string>) this.parser.ScriptPathList)
      {
        string str = this.LoadFilePath(scriptPath, false);
        if (str == null)
        {
          this.host.DiagMessage(DiagnosisKind.Error, string.Format("Unable to locate script file \"{0}\".", (object) scriptPath), (object) null);
          flag = false;
        }
        else
          this.scriptList.Add(str);
      }
      return flag;
    }

    private bool ValidateTaskType()
    {
      TaskType result = TaskType.GenerateTests;
      if (!string.IsNullOrEmpty(this.parser.Task) && !Enum.TryParse<TaskType>(this.parser.Task, true, out result))
      {
        this.host.DiagMessage(DiagnosisKind.Error, string.Format("Unsupported task kind {0}.", (object) this.parser.Task), (object) null);
        return false;
      }
      this.Task = result;
      return true;
    }

    private bool ValidateReplayExplorationResultPath()
    {
      if (string.IsNullOrEmpty(this.parser.ReplayExplorationResultPath))
        return true;
      this.replayExplorationResultPath = this.LoadFilePath(this.parser.ReplayExplorationResultPath, false);
      if (this.replayExplorationResultPath != null)
        return true;
      this.host.DiagMessage(DiagnosisKind.Error, string.Format("Unable to locate Replay Exploration Result file \"{0}\".", (object) this.parser.ReplayExplorationResultPath), (object) null);
      return false;
    }

    private string LoadFilePath(string fileName, bool searchLibDirs)
    {
      if (File.Exists(fileName))
        return Path.GetFullPath(fileName);
      if (searchLibDirs)
      {
        foreach (string libDirectory in (IEnumerable<string>) this.parser.LibDirectoryList)
        {
          string path = Path.Combine(libDirectory, fileName);
          if (File.Exists(path))
            return path;
        }
      }
      return (string) null;
    }

    private static bool ValidateMachine(IExplorer explorer)
    {
      explorer.StartBuilding().WaitOne();
      return explorer.State == ExplorationState.FinishedBuilding;
    }

    private bool RunTaskForOneMachine(string machineName)
    {
      IDictionary<string, string> machineSwitches = this.globalSession.Application.GetRequiredService<ICordDesignTimeScopeManager>().GetCordDesignTimeManager("global").GetMachineSwitches(machineName);
      using (Session session = new Session(this.host))
      {
        ExplorationMode explorationMode = ExplorationMode.Exploration;
        string str = this.outputDirectory;
        if (this.Task == TaskType.DoOnTheFlyTests)
        {
          str = Path.Combine(str, machineName);
          if (!this.CreateDirectory(str))
            return false;
          explorationMode = string.IsNullOrEmpty(this.replayExplorationResultPath) ? ExplorationMode.OnlineTesting : ExplorationMode.OnlineTestingReplay;
        }
        string fullPath = Path.GetFullPath(Path.Combine(this.outputDirectory, machineName + ".seexpl"));
        string currentStamp = "";
        ExplorationResult explorationResult = (ExplorationResult) null;
        bool flag = true;
        if (explorationMode == ExplorationMode.Exploration && !this.parser.Reexplore)
        {
          currentStamp = ExplorationUtility.GetSourceFilesStamp((ICollection<string>) this.assemblyList, (ICollection<string>) this.scriptList);
          if (!ExplorationUtility.NeedsReExploration(currentStamp, fullPath))
          {
            try
            {
              explorationResult = new ExplorationResultLoader(fullPath).LoadExplorationResult();
              this.host.Log(string.Format("All inputs for machine {0} are up-to-date.", (object) machineName));
              this.host.Log(string.Format("Loaded exploration result '{0}' from '{1}'.", (object) machineName, (object) fullPath));
            }
            catch (ExplorationResultLoadingException ex)
            {
              this.host.Log(string.Format("Failed to load file {0}:\n{1}", (object) fullPath, (object) ex.Message));
              return false;
            }
            flag = false;
          }
        }
        if (flag)
        {
          using (IExplorer explorer = this.CreateExplorer(session, explorationMode, machineName, str, machineSwitches))
          {
            if (explorer == null)
              return false;
            explorer.Sandboxed = true;
            this.host.Log(string.Format("\nValidating machine \"{0}\"", (object) machineName));
            if (!ConsoleHostDriver.ValidateMachine(explorer))
              return false;
            this.host.Log(string.Format("Exploring machine \"{0}\"", (object) machineName));
            explorer.StartExploration().WaitOne();
            if (explorer.State != ExplorationState.FinishedExploring)
              return false;
            explorationResult = explorer.ExplorationResult;
            if (explorationResult == null || explorationResult.TransitionSystem == null)
            {
              this.host.DiagMessage(DiagnosisKind.Error, string.Format("{0} for machine \"{1}\" failed because the exploration result or transition system is empty.", (object) this.Task.Description(), (object) machineName), (object) null);
              return false;
            }
            explorationResult.Extensions.Signature = currentStamp;
          }
        }
        switch (this.Task)
        {
          case TaskType.GenerateTests:
            if (!flag)
              return this.GenerateTestCode(machineName, this.Task, (ISession) session, explorationResult.TransitionSystem);
            if (!this.PersistExplorationResult(fullPath, explorationResult))
              return false;
            this.host.Log(string.Format("{0} for machine \"{1}\"", (object) this.Task.Description(), (object) machineName));
            return this.GenerateTestCode(machineName, this.Task, (ISession) session, explorationResult.TransitionSystem);
          case TaskType.PersistExploration:
            if (flag)
            {
              this.host.Log(string.Format("{0} for machine \"{1}\"", (object) this.Task.Description(), (object) machineName));
              return this.PersistExplorationResult(fullPath, explorationResult);
            }
            this.host.Log(string.Format("All inputs for machine {0} are up-to-date.", (object) machineName));
            return true;
          case TaskType.PerformUserTasks:
            if (!flag)
              return this.RunPostProcessors(fullPath);
            if (!this.PersistExplorationResult(fullPath, explorationResult))
              return false;
            this.host.Log(string.Format("{0} for machine \"{1}\"", (object) this.Task.Description(), (object) machineName));
            return this.RunPostProcessors(fullPath);
          case TaskType.DoOnTheFlyTests:
            return true;
          default:
            this.host.DiagMessage(DiagnosisKind.Error, string.Format("Unsupported task kind {0}.", (object) this.parser.Task), (object) null);
            return false;
        }
      }
    }

    private bool RunPostProcessors(string explorationResultFullPath)
    {
      bool flag = true;
      using (PostProcessorHelper postProcessorHelper = new PostProcessorHelper((IDictionary<string, object>) new Dictionary<string, object>()
      {
        ["WorkingDirectory"] = (object) Environment.CurrentDirectory
      }, (ProgressMessageDisplayer) (msg =>
      {
        if (!this.parser.VerboseOn)
          return;
        this.host.DiagMessage(DiagnosisKind.Hint, msg, (object) null);
      })))
      {
        foreach (Type userTaskType in this.userTaskTypes)
        {
          if (postProcessorHelper.ExecutePostProcessing(explorationResultFullPath, userTaskType, this.host))
          {
            this.host.ProgressMessage(VerbosityLevel.Medium, string.Format("\nSuccessfully perform user task '{0}'.", (object) userTaskType.FullName));
          }
          else
          {
            this.host.ProgressMessage(VerbosityLevel.Medium, string.Format("\nFailed to perform user task '{0}'.", (object) userTaskType.FullName));
            flag = false;
          }
        }
      }
      return flag;
    }

    private bool PersistExplorationResult(
      string explorationResultFullPath,
      ExplorationResult explorationResult)
    {
      if (string.IsNullOrEmpty(explorationResultFullPath))
      {
        this.host.DiagMessage(DiagnosisKind.Error, string.Format("Unable to obtain file path \"{0}\".", (object) explorationResultFullPath), (object) null);
        return false;
      }
      try
      {
        new ExplorationResultPacker(explorationResult).Save(explorationResultFullPath);
        this.host.Log(string.Format("\nExploration result is saved to {0}.", (object) explorationResultFullPath));
      }
      catch (Exception ex)
      {
        this.host.DiagMessage(DiagnosisKind.Error, string.Format("Failed to save exploration result to file {0}. \n {1}", (object) explorationResultFullPath, (object) ex.Message), (object) null);
        return false;
      }
      return true;
    }

    private bool GenerateTestCode(
      string machineName,
      TaskType task,
      ISession session,
      TransitionSystem transitionSystem)
    {
      string text = session.CreateTestCodeGenerator(transitionSystem).Generate(machineName);
      if (string.IsNullOrEmpty(text))
      {
        this.host.DiagMessage(DiagnosisKind.Error, string.Format("{0} for machine {1} failed.", (object) task.Description(), (object) machineName), (object) null);
        return false;
      }
      string str = transitionSystem.GetSwitch("generatedtestpath");
      if (string.IsNullOrEmpty(str))
      {
        this.host.DiagMessage(DiagnosisKind.Error, string.Format("Unable to obtain directory \"{0}\".", (object) str), (object) null);
        return false;
      }
      string fullPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, str.Replace("\\\\", "\\")));
      if (!this.CreateDirectory(fullPath))
      {
        this.host.DiagMessage(DiagnosisKind.Error, string.Format("Unable to obtain directory \"{0}\".", (object) fullPath), (object) null);
        return false;
      }
      string path2 = transitionSystem.GetSwitch("generatedtestfile");
      if (string.IsNullOrEmpty(path2))
        path2 = machineName + ".cs";
      return this.WriteTestCodeFile(Path.GetFullPath(Path.Combine(fullPath, path2)), text);
    }

    private void InitializeGlobalCordScope()
    {
      if (this.globalSession == null)
        return;
      ICordDesignTimeManager designTimeManager = this.globalSession.Application.GetRequiredService<ICordDesignTimeScopeManager>().RegisterCordDesignTimeManager("global");
      foreach (string script in this.scriptList)
      {
        string scriptContent = (string) null;
        try
        {
          scriptContent = File.ReadAllText(script);
        }
        catch
        {
          continue;
        }
        designTimeManager.RegisterScript(script, (Func<string>) (() => scriptContent));
      }
    }

    public bool Run(out int successCounter, out int failedCounter)
    {
      successCounter = 0;
      failedCounter = 0;
      this.host.Log("\nValidating the arguments...");
      if (!this.ValidateArgs())
        return false;
      if (this.Task == TaskType.Validate)
        return this.Validate();
      foreach (string machine in this.machineList)
      {
        if (!this.RunTaskForOneMachine(machine))
          ++failedCounter;
        else
          ++successCounter;
      }
      return true;
    }

    private string[] GetMatchedMachineNames(
      IEnumerable<string> machineNameRegexList,
      IEnumerable<string> availableMachineNames)
    {
      if (availableMachineNames == null)
        return (string[]) null;
      HashSet<string> stringSet = new HashSet<string>();
      List<string> stringList = new List<string>();
      foreach (string availableMachineName in availableMachineNames)
      {
        foreach (string machineNameRegex in machineNameRegexList)
        {
          if (Regex.IsMatch(availableMachineName, machineNameRegex))
          {
            stringList.Add(availableMachineName);
            stringSet.Add(machineNameRegex);
            break;
          }
        }
      }
      foreach (string machineNameRegex in machineNameRegexList)
      {
        if (!stringSet.Contains(machineNameRegex))
          this.host.DiagMessage(DiagnosisKind.Warning, string.Format("No machine found with name matching the regular expression \"{0}\".", (object) machineNameRegex), (object) null);
      }
      return stringList.ToArray();
    }

    private IEnumerable<string> GetFormattedRegexList(IList<string> regexInputList)
    {
      List<string> stringList = new List<string>();
      foreach (string regexInput in (IEnumerable<string>) regexInputList)
      {
        try
        {
          Regex regex = new Regex(string.Format("{1}{0}{2}", (object) regexInput, regexInput.StartsWith("^") ? (object) "" : (object) "^", regexInput.EndsWith("$") ? (object) "" : (object) "$"));
          stringList.Add(regex.ToString());
        }
        catch (ArgumentException ex)
        {
          this.host.DiagMessage(DiagnosisKind.Warning, string.Format("Invalid regular expression \"{0}\" in command-line switch \"{1}\"", (object) regexInput, (object) "/machines:".ToSwitchNameDescription()), (object) null);
        }
      }
      return (IEnumerable<string>) stringList;
    }

    private bool CreateDirectory(string path)
    {
      if (string.IsNullOrEmpty(path))
        return false;
      try
      {
        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);
        return true;
      }
      catch (Exception ex)
      {
        this.host.DiagMessage(DiagnosisKind.Error, string.Format("Unable to create the directory \"{0}\".\n {1}", (object) path, (object) ex.Message), (object) null);
        return false;
      }
    }

    private bool WriteTestCodeFile(string fileName, string text)
    {
      try
      {
        File.WriteAllText(fileName, text);
        this.host.Log(string.Format("\nGenerated test code saved to \"{0}\".", (object) fileName));
        return true;
      }
      catch (Exception ex)
      {
        this.host.DiagMessage(DiagnosisKind.Error, string.Format("Unable to save the test suites to \"{0}\"\n {1}", (object) fileName, (object) ex.Message), (object) null);
        return false;
      }
    }

    private static void OnConsoleCancelKeyPressed(object sender, ConsoleCancelEventArgs e) => Console.ResetColor();

    private static void PrintCommandUsage()
    {
      Console.ResetColor();
      Console.WriteLine("-- USAGE --\r\n\r\nSpecExplorer.exe /task:GenerateTests|PersistExploration|PerformUserTasks|DoOnTheFlyTests|Validate /assemblies:Model.dll,Implementation.dll,... /lib:<Dir> /scripts:A.cord,B.cord,... /machines:TestSuite,... UserTasks:MyPostProcessor,... /ExplorationResultPath:<Dir> /TestResultPath:<Dir> /ReplayTestResultFile:<FilePath> /OnTheFlyMaximumExperimentCount:<number> /OnTheFlyAllowEndingAtEventStates /verbose /Reexplore\r\n            \r\n/task:                  'GenerateTests' (default) or 'PersistExploration' \r\n                        or 'PerformUserTasks' or 'DoOnTheFlyTests' or\r\n                        'Validate'. \r\n                        (Short Form: '/t:')\r\n\r\n/assemblies:            Comma separated list of required assemblies.\r\n                        The first assembly must be model assembly, \r\n                        other assemblies with ModelingAssembly attribute \r\n                        will be regarded as model assembly.\r\n                        (Short Form: '/a:')\r\n\r\n/lib:                   Comma separated list of directories to search in\r\n                        for assemblies specified by switch /assemblies.\r\n                        (Short Form: '/l:')\r\n\r\n/scripts:               Comma separated list of cord script files.\r\n                        (Short Form: '/s:')\r\n\r\n/machines:              Comma separated list of regular expressions\r\n                        identifying the machines in the mentioned scripts.\r\n                        Each regular expression will be prefixed with '^'\r\n                        if it is not started with '^', and appended with '$'\r\n                        if it is not ended with '$' automatically.\r\n                        For example, if the argument is /machines:Test,\r\n                        the effective argument used will be /machines:^Test$\r\n                        (which means exactly match machine name \"Test\"). \r\n                        (Short Form: '/m:')\r\n\r\n/UserTasks:             Comma separate list of user task class name,\r\n                        must be used together with /Task:PerformUserTasks.\r\n                        '<SpecExplorer.exe directory>\\Extensions' directory\r\n                        will be used to search user task assemblies.\r\n                        (Short Form: '/u:')\r\n\r\n/ExplorationResultPath: Directory for storing the exploration result files.\r\n                        (default: '<SpecExplorer.exe launch directory>\\\r\n                        ExplorationResults'; Short Form: '/erp:')\r\n\r\n/TestResultPath:        Directory for storing the on-the-fly testing test\r\n                        result files.\r\n                        (default: '<SpecExplorer.exe launch directory>\\\r\n                        TestResults'; Short Form: '/trp:')\r\n/ReplayTestResultFile:\r\n                        Required exploration result file used to replay test \r\n                        case in on-the-fly testing. When this switch is set, \r\n                        DoOnTheFlyTests will replay the test case in the file. \r\n                        This switch only works when /Task:DoOnTheFlyTests is \r\n                        specified. This switch only supports one file each \r\n                        time.\r\n                        (Short Form: '/rtrf:')\r\n\r\n/OnTheFlyMaximumExperimentCount:\r\n                        Defines the maximum number of experiments allowed in\r\n                        one on the fly testing test run before attempting to\r\n                        shut down (at the first encountered accepting state). \r\n                        The value of 'none' means that the number of \r\n                        experiments is not limited.\r\n                        (Short Form: '/otfmec:')\r\n\r\n/OnTheFlyAllowEndingAtEventStates:               \r\n                        Controls whether Spec Explorer may end an OTF test in a\r\n                        state where one or more following events are expected. \r\n                        Setting this switch to true will allow tests to end at \r\n                        an accepting state without waiting for outgoing event \r\n                        steps.\r\n                        (default is false; Short Form: '/otfaeaes')\r\n\r\n/verbose:               Enable full status and warning messages.\r\n                        (default additional status and warning message is \r\n                        turned off; Short Form: '/v')\r\n/ReExplore:             Indicate whether force re-explore the given machine or\r\n                        not. \r\n                        (default is false; Short Form: '/re')\r\n");
    }

    private IExplorer CreateExplorer(
      Session session,
      ExplorationMode explorationMode,
      string machineName,
      string outputDir,
      IDictionary<string, string> machineSwitches)
    {
      IExplorer explorer = session.CreateExplorer((ICollection<string>) this.assemblyList, (ICollection<string>) this.scriptList, explorationMode, machineName, outputDir, this.replayExplorationResultPath, this.parser.OnTheFlyMaximumExperimentCount, machineSwitches, this.parser.AllowEndingAtEventStates);
      explorer.TestingStatisticsProgress += (EventHandler<TestingStatisticsEventArgs>) ((sender, args) =>
      {
        if (!this.parser.VerboseOn)
          return;
        this.host.ProgressMessage(VerbosityLevel.Medium, args.Statistics.ToString());
      });
      explorer.TestCaseFinishedProgress += (EventHandler<TestCaseFinishedEventArgs>) ((sender, args) =>
      {
        if (!this.parser.VerboseOn)
          return;
        this.host.ProgressMessage(VerbosityLevel.Medium, string.Format("Finish test case {0} with {1}.", (object) args.TestCaseName, (object) args.Result));
      });
      explorer.ExplorationResultUpdated += (EventHandler<ExplorationResultEventArgs>) ((sender, args) =>
      {
        if (args.ExplorationResult == null || args.ExplorationResult.TransitionSystem == null)
          this.host.DiagMessage(DiagnosisKind.Error, "Failed to save test result because the exploration result or transition system is empty", (object) null);
        else
          this.PersistExplorationResult(Path.GetFullPath(Path.Combine(this.outputDirectory, machineName, args.ExplorationResult.TransitionSystem.Name + ".seexpl")), args.ExplorationResult);
      });
      return explorer;
    }

    private bool Validate()
    {
      CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
      ICordDesignTimeManager designTimeManager = this.globalSession.Application.GetRequiredService<ICordDesignTimeScopeManager>().GetCordDesignTimeManager("global");
      designTimeManager.ReportError += new EventHandler<ErrorReport>(this.OnReportError);
      try
      {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        return designTimeManager.EnsureValidation((IEnumerable<string>) this.assemblyList);
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = currentCulture;
      }
    }

    [STAThread]
    public static void Main(string[] args)
    {
      Console.CancelKeyPress += new ConsoleCancelEventHandler(ConsoleHostDriver.OnConsoleCancelKeyPressed);
      IHost consoleHost = (IHost) new ConsoleHost();
      CommandLineParser clParser = new CommandLineParser(consoleHost);
      if (!clParser.ParseCommand(args))
      {
        ConsoleHostDriver.PrintCommandUsage();
        Environment.Exit(1);
      }
      int exitCode;
      using (ConsoleHostDriver consoleHostDriver = new ConsoleHostDriver(consoleHost, clParser))
      {
        int successCounter;
        int failedCounter;
        if (!consoleHostDriver.Run(out successCounter, out failedCounter))
        {
          Console.WriteLine(string.Format("\nFailed to process task {0}.", (object) ConsoleHostDriver.BuildTaskName(consoleHostDriver.Task, clParser.ReplayExplorationResultPath)));
          exitCode = 1;
        }
        else
        {
          string str = ConsoleHostDriver.BuildTaskName(consoleHostDriver.Task, clParser.ReplayExplorationResultPath);
          if (consoleHostDriver.Task == TaskType.Validate)
          {
            Console.WriteLine("\nSuccessfully process task {0}.", (object) str);
            exitCode = 0;
          }
          else
          {
            if (successCounter == 0)
            {
              Console.WriteLine(string.Format("\nFailed to process task {0}.", (object) str));
              exitCode = 1;
            }
            if (failedCounter > 0)
            {
              Console.WriteLine("\nSuccessfully process task {0} for {1} machine(s). Failed to process task {0} for {2} machine(s).", (object) str, (object) successCounter, (object) failedCounter);
              exitCode = 1;
            }
            else
            {
              Console.WriteLine("\nSuccessfully process task {0} for {1} machine(s).", (object) str, (object) successCounter);
              exitCode = 0;
            }
          }
        }
      }
      Environment.Exit(exitCode);
    }

    private static string BuildTaskName(TaskType task, string replayExplorationResultPath) => task == TaskType.DoOnTheFlyTests && !string.IsNullOrEmpty(replayExplorationResultPath) ? string.Format("'{0}' with replay mode", (object) task) : string.Format("'{0}'", (object) task);

    public void Dispose()
    {
      if (this.disposed)
        return;
      if (this.globalSession is IDisposable globalSession)
        globalSession.Dispose();
      this.globalSession = (ISession) null;
      this.disposed = true;
      GC.SuppressFinalize((object) this);
    }
  }
}
