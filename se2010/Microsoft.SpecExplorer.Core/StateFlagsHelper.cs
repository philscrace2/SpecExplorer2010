// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.StateFlagsHelper
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\Users\pls2\OneDrive\source code\SourceCode\spec_explorer\original_files\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

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
          flags &= ~StateFlags.Error;
          flags |= StateFlags.Accepting;
          break;
        case ControlStateKind.Error:
          flags &= ~StateFlags.Accepting;
          flags |= StateFlags.Error;
          break;
      }
      return flags;
    }

    public static StateFlags SetStoppedReasonFlags(
      this StateFlags flags,
      ExplorationStateFlags explorationFlags)
    {
      flags &= ~StateFlags.StoppedReasonFlags;
      if ((explorationFlags & ExplorationStateFlags.IsStepBoundStopped) != ExplorationStateFlags.None)
        flags |= StateFlags.StepBoundStopped;
      if ((explorationFlags & ExplorationStateFlags.IsStateBoundStopped) != ExplorationStateFlags.None)
        flags |= StateFlags.StateBoundStopped;
      if ((explorationFlags & ExplorationStateFlags.IsPathDepthBoundStopped) != ExplorationStateFlags.None)
        flags |= StateFlags.PathDepthBoundStopped;
      if ((explorationFlags & ExplorationStateFlags.IsStepsPerStateBoundStopped) != ExplorationStateFlags.None)
        flags |= StateFlags.StepsPerStateBoundStopped;
      if ((explorationFlags & ExplorationStateFlags.IsExplorationErrorBoundStopped) != ExplorationStateFlags.None)
        flags |= StateFlags.ExplorationErrorBoundStopped;
      if ((explorationFlags & ExplorationStateFlags.IsUserStopped) != ExplorationStateFlags.None)
        flags |= StateFlags.UserStopped;
      if ((explorationFlags & ExplorationStateFlags.IsNonAcceptingEnd) != ExplorationStateFlags.None)
      {
        flags &= ~StateFlags.Accepting;
        flags |= StateFlags.NonAcceptingEnd;
      }
      return flags;
    }
  }
}
