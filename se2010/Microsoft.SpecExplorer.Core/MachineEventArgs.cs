// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.MachineEventArgs
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer
{
  [Serializable]
  public sealed class MachineEventArgs : EventArgs
  {
    public IList<Machine> Machines { get; private set; }

    public bool ReExplore { get; set; }

    public IEnumerable<string> PostProcessors { get; private set; }

    public bool IsOnTheFlyReplay { get; private set; }

    public MachineEventArgs(
      IList<Machine> machines,
      bool reExplore = false,
      IEnumerable<string> postProcessors = null,
      bool isOnTheFlyReplay = false)
    {
      if (machines == null)
        throw new ArgumentNullException(nameof (machines));
      this.Machines = machines.Count != 0 ? machines : throw new ArgumentException("The number of machines should not be zero.");
      this.ReExplore = reExplore;
      this.PostProcessors = postProcessors;
      this.IsOnTheFlyReplay = isOnTheFlyReplay;
    }
  }
}
