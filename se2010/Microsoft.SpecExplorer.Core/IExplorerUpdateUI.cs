// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.IExplorerUpdateUI
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
  public interface IExplorerUpdateUI
  {
    void ShowStatistics(ExplorationStatistics statistics);

    void ShowStatistics(TestingStatistics statistics);

    void SwitchState(ExplorationState state);

    void UpdateExplorationResult(ExplorationResult explorationResult);

    void ShowTestCaseFinishedProgress(TestCaseFinishedEventArgs args);
  }
}
