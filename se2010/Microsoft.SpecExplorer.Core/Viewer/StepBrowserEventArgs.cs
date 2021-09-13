// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.StepBrowserEventArgs
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer.Viewer
{
  public sealed class StepBrowserEventArgs : EventArgs
  {
    public IEnumerable<BrowserEdge> BrowserEdges { get; private set; }

    public string StepLabel { get; private set; }

    public StepBrowserEventArgs(IEnumerable<BrowserEdge> browserEdges, string stepLabel)
    {
      this.BrowserEdges = browserEdges;
      this.StepLabel = stepLabel;
    }
  }
}
