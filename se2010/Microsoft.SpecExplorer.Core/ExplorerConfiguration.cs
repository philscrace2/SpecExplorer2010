// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ExplorerConfiguration
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.Xrt;
using System;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer
{
  [Serializable]
  public class ExplorerConfiguration
  {
    public ExplorerConfiguration(
      ICollection<string> assemblies,
      ICollection<string> scripts,
      ExplorationMode explorationMode,
      string machine,
      IDictionary<string, string> machineSwitches,
      string outputDir,
      string replayResultPath,
      int? onTheFlyMaximumExperimentCount,
      bool allowEndingAtEventStates)
    {
      this.Assemblies = assemblies;
      this.Scripts = scripts;
      this.ExplorationMode = explorationMode;
      this.Machine = machine;
      this.MachineSwitches = machineSwitches;
      this.OutputDirectory = outputDir;
      this.ReplayResultPath = replayResultPath;
      this.OnTheFlyMaximumExperimentCount = onTheFlyMaximumExperimentCount;
      this.AllowEndingAtEventStates = allowEndingAtEventStates;
    }

    public string Machine { get; private set; }

    public ICollection<string> Assemblies { get; private set; }

    public ICollection<string> Scripts { get; private set; }

    public IDictionary<string, string> MachineSwitches { get; private set; }

    public ExplorationMode ExplorationMode { get; private set; }

    public string OutputDirectory { get; private set; }

    public int? OnTheFlyMaximumExperimentCount { get; private set; }

    public string ReplayResultPath { get; private set; }

    public bool AllowEndingAtEventStates { get; private set; }
  }
}
