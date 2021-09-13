// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ISession
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer
{
  public interface ISession
  {
    IHost Host { get; }

    ApplicationBase Application { get; }

    IExplorer CreateExplorer(
      ICollection<string> assemblies,
      ICollection<string> scripts,
      ExplorationMode explorationMode,
      string machine,
      string outputDir,
      string replayPath,
      int? onTheFlyMaximumExperimentCount,
      IDictionary<string, string> machineSwitches,
      bool allowEndingAtEventStates);

    ITestCodeGenerator CreateTestCodeGenerator(TransitionSystem transitionSystem);

    string InstallDir { get; }

    string ConfigurationDir { get; }
  }
}
