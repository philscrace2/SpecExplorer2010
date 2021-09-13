// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.StatesBrowserEventArgs
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer.Viewer
{
  public sealed class StatesBrowserEventArgs : EventArgs
  {
    public IEnumerable<State> States { get; private set; }

    public bool ShouldDisplayLeftTree { get; private set; }

    public string StateLabel { get; private set; }

    public StatesBrowserEventArgs(DisplayNode node)
    {
      List<State> stateList = new List<State>();
      this.States = (IEnumerable<State>) stateList;
      switch (node.DisplayNodeKind)
      {
        case DisplayNodeKind.Normal:
          stateList.Add(node.Label);
          break;
        case DisplayNodeKind.Hyper:
          stateList.AddRange((IEnumerable<State>) node.LeafNodeStates);
          break;
      }
      this.ShouldDisplayLeftTree = node.DisplayNodeKind == DisplayNodeKind.Hyper;
      this.StateLabel = node.Text;
    }
  }
}
