// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.IExplorer
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.Threading;

namespace Microsoft.SpecExplorer
{
  public interface IExplorer : IDisposable
  {
    ISession Session { get; }

    bool Sandboxed { get; set; }

    ExplorationState State { get; }

    WaitHandle StartBuilding();

    WaitHandle StartExploration();

    ExplorationResult ExplorationResult { get; }

    event EventHandler<ExplorationResultEventArgs> ExplorationResultUpdated;

    event EventHandler<ExplorationStatisticsEventArgs> ExplorationStatisticsProgress;

    event EventHandler<TestingStatisticsEventArgs> TestingStatisticsProgress;

    event EventHandler<TestCaseFinishedEventArgs> TestCaseFinishedProgress;

    event EventHandler<ExplorationStateChangedEventArgs> ExplorationStateChanged;

    void Abort();
  }
}
