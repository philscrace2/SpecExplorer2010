// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.CompareStateEventArgs
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using System;

namespace Microsoft.SpecExplorer.Viewer
{
  public sealed class CompareStateEventArgs : EventArgs
  {
    public State Left { get; private set; }

    public State Right { get; private set; }

    public string CompareLabel { get; private set; }

    public CompareStateEventArgs(State left, State right)
    {
      this.Left = left;
      this.Right = right;
      this.CompareLabel = string.Format("{0} : {1}", (object) left.Label, (object) right.Label);
    }
  }
}
