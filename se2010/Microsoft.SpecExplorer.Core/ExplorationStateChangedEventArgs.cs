// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ExplorationStateChangedEventArgs
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;

namespace Microsoft.SpecExplorer
{
  public class ExplorationStateChangedEventArgs : EventArgs
  {
    internal ExplorationStateChangedEventArgs(ExplorationState oldState, ExplorationState newState)
    {
      this.OldState = oldState;
      this.NewState = newState;
    }

    public ExplorationState OldState { get; private set; }

    public ExplorationState NewState { get; private set; }
  }
}
