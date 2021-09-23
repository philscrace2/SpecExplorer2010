// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.CommandLine.CommandLineParser
// Assembly: SpecExplorer, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7507FD4E-0ABD-4C37-958B-5FF2521D030B
// Assembly location: C:\Users\pls2\OneDrive\source code\SourceCode\spec_explorer\original_files\Spec Explorer 2010\SpecExplorer.exe

using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer.CommandLine
{
  public class CommandLineParser
  {
    private List<string> machineNameRegexList = new List<string>();
    private List<string> scriptPathList = new List<string>();
    private List<string> assemblyPathList = new List<string>();
    private List<string> libDirList = new List<string>();
    private List<string> userTasks = new List<string>();
    private IHost host;

    public bool AllowEndingAtEventStates { get; private set; }

    public bool VerboseOn { get; private set; }

    public string ExplorationResultPath { get; private set; }

    public string TestResultPath { get; private set; }

    public string ReplayExplorationResultPath { get; private set; }

    public string Task { get; private set; }

    public int? OnTheFlyMaximumExperimentCount { get; set; }

    public bool Reexplore { get; private set; }

    public CommandLineParser(IHost consoleHost) => this.host = consoleHost;

    public bool ParseCommand(string[] args)
    {
      HashSet<string> first1 = new HashSet<string>();
      if (args == null)
        return false;
      foreach (string s in args)
      {
        int cutOffset;
        if (CommandLineParser.IsCommandLineSwitch(s, "/task:", "/t:", out cutOffset))
        {
          if (!first1.Add("/task:"))
          {
            this.ReportDuplicateCommandLineSwitchError("/task:", "/t:");
            return false;
          }
          if (s.Length == cutOffset)
          {
            this.host.DiagMessage(DiagnosisKind.Error, string.Format("Missing task specification for switch \"{0}\".", (object) "/task:".ToSwitchNameDescription()), (object) null);
            return false;
          }
          this.Task = s.Substring(cutOffset);
        }
        else if (CommandLineParser.IsCommandLineSwitch(s, "/assemblies:", "/a:", out cutOffset))
        {
          if (!first1.Add("/assemblies:"))
          {
            this.ReportDuplicateCommandLineSwitchError("/assemblies:", "/a:");
            return false;
          }
          if (s.Length == cutOffset)
          {
            this.host.DiagMessage(DiagnosisKind.Error, string.Format("Missing assemblies specification for switch \"{0}\".", (object) "/assemblies:".ToSwitchNameDescription()), (object) null);
            return false;
          }
          this.assemblyPathList.AddRange(CommandLineParser.GetSwitchValues(s, cutOffset).Distinct<string>());
        }
        else if (CommandLineParser.IsCommandLineSwitch(s, "/lib:", "/l:", out cutOffset))
        {
          if (!first1.Add("/lib:"))
          {
            this.ReportDuplicateCommandLineSwitchError("/lib:", "/l:");
            return false;
          }
          if (s.Length == cutOffset)
          {
            this.host.DiagMessage(DiagnosisKind.Error, string.Format("Missing library path specification for switch \"{0}\".", (object) "/lib:".ToSwitchNameDescription()), (object) null);
            return false;
          }
          this.libDirList.AddRange(CommandLineParser.GetSwitchValues(s, cutOffset).Distinct<string>());
        }
        else if (CommandLineParser.IsCommandLineSwitch(s, "/scripts:", "/s:", out cutOffset))
        {
          if (!first1.Add("/scripts:"))
          {
            this.ReportDuplicateCommandLineSwitchError("/scripts:", "/s:");
            return false;
          }
          if (s.Length == cutOffset)
          {
            this.host.DiagMessage(DiagnosisKind.Error, string.Format("Missing scripts specification for switch \"{0}\".", (object) "/scripts:".ToSwitchNameDescription()), (object) null);
            return false;
          }
          this.scriptPathList.AddRange(CommandLineParser.GetSwitchValues(s, cutOffset).Distinct<string>());
        }
        else if (CommandLineParser.IsCommandLineSwitch(s, "/machines:", "/m:", out cutOffset))
        {
          if (!first1.Add("/machines:"))
          {
            this.ReportDuplicateCommandLineSwitchError("/machines:", "/m:");
            return false;
          }
          if (s.Length == cutOffset)
          {
            this.host.DiagMessage(DiagnosisKind.Error, string.Format("Missing machines specification for switch \"{0}\".", (object) "/machines:".ToSwitchNameDescription()), (object) null);
            return false;
          }
          this.machineNameRegexList.AddRange(CommandLineParser.GetSwitchValues(s, cutOffset).Distinct<string>());
        }
        else if (CommandLineParser.IsCommandLineSwitch(s, "/ExplorationResultPath:", "/erp:", out cutOffset))
        {
          if (!first1.Add("/ExplorationResultPath:"))
          {
            this.ReportDuplicateCommandLineSwitchError("/ExplorationResultPath:", "/erp:");
            return false;
          }
          if (s.Length == cutOffset)
          {
            this.host.DiagMessage(DiagnosisKind.Error, string.Format("Missing exploration result path specification for switch \"{0}\".", (object) "/ExplorationResultPath:".ToSwitchNameDescription()), (object) null);
            return false;
          }
          this.ExplorationResultPath = s.Length == cutOffset ? "" : s.Substring(cutOffset);
        }
        else if (CommandLineParser.IsCommandLineSwitch(s, "/TestResultPath:", "/trp:", out cutOffset))
        {
          if (!first1.Add("/TestResultPath:"))
          {
            this.ReportDuplicateCommandLineSwitchError("/TestResultPath:", "/trp:");
            return false;
          }
          if (s.Length == cutOffset)
          {
            this.host.DiagMessage(DiagnosisKind.Error, string.Format("Missing on-the-fly test result path specification for switch \"{0}\".", (object) "/TestResultPath:".ToSwitchNameDescription()), (object) null);
            return false;
          }
          this.TestResultPath = s.Length == cutOffset ? "" : s.Substring(cutOffset);
        }
        else if (CommandLineParser.IsCommandLineSwitch(s, "/ReplayTestResultFile:", "/rtrf:", out cutOffset))
        {
          if (!first1.Add("/ReplayTestResultFile:"))
          {
            this.ReportDuplicateCommandLineSwitchError("/ReplayTestResultFile:", "/rtrf:");
            return false;
          }
          if (s.Length == cutOffset)
          {
            this.host.DiagMessage(DiagnosisKind.Error, string.Format("Missing on-the-fly test replay exploration result path specification for switch \"{0}\".", (object) "/ReplayTestResultFile:".ToSwitchNameDescription()), (object) null);
            return false;
          }
          this.ReplayExplorationResultPath = s.Length == cutOffset ? "" : s.Substring(cutOffset);
        }
        else if (CommandLineParser.IsCommandLineSwitch(s, "/UserTasks:", "/u:", out cutOffset))
        {
          if (!first1.Add("/UserTasks:"))
          {
            this.ReportDuplicateCommandLineSwitchError("/UserTasks:", "/u:");
            return false;
          }
          if (s.Length == cutOffset)
          {
            this.host.DiagMessage(DiagnosisKind.Error, string.Format("Missing user tasks specification for switch \"{0}\".", (object) "/UserTasks:".ToSwitchNameDescription()), (object) null);
            return false;
          }
          this.userTasks.AddRange(CommandLineParser.GetSwitchValues(s, cutOffset).Distinct<string>());
        }
        else if (CommandLineParser.IsCommandLineSwitch(s, "/OnTheFlyAllowEndingAtEventStates", "/otfaeaes", out cutOffset) && s.Length == cutOffset)
        {
          if (!first1.Add("/OnTheFlyAllowEndingAtEventStates"))
          {
            this.ReportDuplicateCommandLineSwitchError("/OnTheFlyAllowEndingAtEventStates", "/otfaeaes");
            return false;
          }
          this.AllowEndingAtEventStates = true;
        }
        else if (CommandLineParser.IsCommandLineSwitch(s, "/verbose", "/v", out cutOffset) && s.Length == cutOffset)
        {
          if (!first1.Add("/verbose"))
          {
            this.ReportDuplicateCommandLineSwitchError("/verbose", "/v");
            return false;
          }
          this.VerboseOn = true;
        }
        else if (CommandLineParser.IsCommandLineSwitch(s, "/OnTheFlyMaximumExperimentCount:", "/otfmec:", out cutOffset))
        {
          if (!first1.Add("/OnTheFlyMaximumExperimentCount:"))
          {
            this.ReportDuplicateCommandLineSwitchError("/OnTheFlyMaximumExperimentCount:", (string) null);
            return false;
          }
          if (s.Length == cutOffset)
          {
            this.host.DiagMessage(DiagnosisKind.Error, string.Format("Missing value specification for switch \"{0}\".", (object) "/OnTheFlyMaximumExperimentCount:".ToSwitchNameDescription()), (object) null);
            return false;
          }
          string text = s.Substring(cutOffset);
          try
          {
            this.OnTheFlyMaximumExperimentCount = new int?((int) new NoneablePositiveIntegerConverter().ConvertFromString(text));
          }
          catch (Exception ex)
          {
            this.host.DiagMessage(DiagnosisKind.Error, string.Format("Invalid value for switch '{0}': {1}", (object) "/OnTheFlyMaximumExperimentCount:".ToSwitchNameDescription(), (object) ex.Message), (object) null);
            return false;
          }
        }
        else
        {
          if (!CommandLineParser.IsCommandLineSwitch(s, "/Reexplore", "/re", out cutOffset) || s.Length != cutOffset)
            return false;
          if (!first1.Add("/Reexplore"))
          {
            this.ReportDuplicateCommandLineSwitchError("/Reexplore", "/re");
            return false;
          }
          this.Reexplore = true;
        }
      }
      List<string> first2;
      if (this.Task == "Validate")
        first2 = new List<string>() { "/scripts:" };
      else
        first2 = new List<string>()
        {
          "/scripts:",
          "/machines:"
        };
      IEnumerable<string> strings = first1.Intersect<string>((IEnumerable<string>) first2);
      if (strings.Count<string>() == first2.Count)
        return true;
      this.host.DiagMessage(DiagnosisKind.Error, string.Format("Command line switch(s) \"{0}\" must be specified.", (object) string.Join(", ", first2.Except<string>(strings).Select<string, string>((Func<string, string>) (s => s.ToSwitchNameDescription())))), (object) null);
      return false;
    }

    private void ReportDuplicateCommandLineSwitchError(string switchName, string switchShortName)
    {
      if (string.IsNullOrEmpty(switchName))
        throw new ArgumentException("cannot be null or empty.", nameof (switchName));
      if (string.IsNullOrEmpty(switchShortName))
        this.host.DiagMessage(DiagnosisKind.Error, string.Format("Duplicate command-line switch \"{0}\".", (object) switchName.ToSwitchNameDescription()), (object) null);
      else
        this.host.DiagMessage(DiagnosisKind.Error, string.Format("Duplicate command-line switch \"{0}\" or \"{1}\".", (object) switchName.ToSwitchNameDescription(), (object) switchShortName.ToSwitchNameDescription()), (object) null);
    }

    public IList<string> ScriptPathList => (IList<string>) this.scriptPathList;

    public IList<string> AssemblyPathList => (IList<string>) this.assemblyPathList;

    public IList<string> MachineNameRegexList => (IList<string>) this.machineNameRegexList;

    public IList<string> LibDirectoryList => (IList<string>) this.libDirList;

    public IList<string> UserTasks => (IList<string>) this.userTasks;

    private static IEnumerable<string> GetSwitchValues(string s, int valueOffset)
    {
      if (s.Length == valueOffset)
        return Enumerable.Empty<string>();
      return (IEnumerable<string>) s.Substring(valueOffset).Split(',');
    }

    private static bool IsCommandLineSwitch(
      string arg,
      string switchLongName,
      string switchShortName,
      out int cutOffset)
    {
      if (!string.IsNullOrEmpty(switchLongName) && arg.StartsWith(switchLongName, StringComparison.CurrentCultureIgnoreCase))
      {
        cutOffset = switchLongName.Length;
        return true;
      }
      if (!string.IsNullOrEmpty(switchShortName) && arg.StartsWith(switchShortName, StringComparison.CurrentCultureIgnoreCase))
      {
        cutOffset = switchShortName.Length;
        return true;
      }
      cutOffset = 0;
      return false;
    }
  }
}
