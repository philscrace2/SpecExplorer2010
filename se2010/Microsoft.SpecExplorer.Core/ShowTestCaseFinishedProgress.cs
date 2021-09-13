// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ShowTestCaseFinishedProgress
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using System;

namespace Microsoft.SpecExplorer
{
  [Serializable]
  internal class ShowTestCaseFinishedProgress : ExplorerEvent
  {
    public TestCaseFinishedEventArgs Progress { get; private set; }

    public ShowTestCaseFinishedProgress(TestCaseFinishedEventArgs args)
      : base(ExplorerEventType.ShowTestCaseFinishedProgress)
    {
      this.Progress = args != null ? args : throw new ArgumentNullException(nameof (args));
    }
  }
}
