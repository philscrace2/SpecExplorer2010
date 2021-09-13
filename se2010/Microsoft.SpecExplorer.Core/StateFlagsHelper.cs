// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.StateFlagsHelper
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
  public static class StateFlagsHelper
  {
    public static StateFlags SetStateKindFlag(
      this StateFlags flags,
      ControlStateKind kind)
    {
      switch (kind)
      {
        case ControlStateKind.Accepting:
          flags = (StateFlags) (flags & -5);
          flags = (StateFlags) (flags | 2);
          break;
        case ControlStateKind.Error:
          flags = (StateFlags) (flags & -3);
          flags = (StateFlags) (flags | 4);
          break;
      }
      return flags;
    }

    public static StateFlags SetStoppedReasonFlags(
      this StateFlags flags,
      ExplorationStateFlags explorationFlags)
    {
      flags = (StateFlags) (flags & -1017);
      if ((explorationFlags & ExplorationStateFlags.IsStepBoundStopped) != ExplorationStateFlags.None)
        flags = (StateFlags) (flags | 8);
      if ((explorationFlags & ExplorationStateFlags.IsStateBoundStopped) != ExplorationStateFlags.None)
        flags = (StateFlags) (flags | 16);
      if ((explorationFlags & ExplorationStateFlags.IsPathDepthBoundStopped) != ExplorationStateFlags.None)
        flags = (StateFlags) (flags | 32);
      if ((explorationFlags & ExplorationStateFlags.IsStepsPerStateBoundStopped) != ExplorationStateFlags.None)
        flags = (StateFlags) (flags | 64);
      if ((explorationFlags & ExplorationStateFlags.IsExplorationErrorBoundStopped) != ExplorationStateFlags.None)
        flags = (StateFlags) (flags | 512);
      if ((explorationFlags & ExplorationStateFlags.IsUserStopped) != ExplorationStateFlags.None)
        flags = (StateFlags) (flags | 128);
      if ((explorationFlags & ExplorationStateFlags.IsNonAcceptingEnd) != ExplorationStateFlags.None)
      {
        flags = (StateFlags) (flags & -3);
        flags = (StateFlags) (flags | 256);
      }
      return flags;
    }
  }
}
