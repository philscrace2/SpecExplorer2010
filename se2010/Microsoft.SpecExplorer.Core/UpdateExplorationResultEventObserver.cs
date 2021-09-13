// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.UpdateExplorationResultEventObserver
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

namespace Microsoft.SpecExplorer
{
  internal class UpdateExplorationResultEventObserver : EventObserver
  {
    private IExplorerUpdateUI explorer;

    internal UpdateExplorationResultEventObserver(IExplorerUpdateUI explorerImpl) => this.explorer = explorerImpl;

    internal override bool HandleEvent(ExplorerEvent e)
    {
      if (ExplorerEventType.UpdateExplorationResult != e.Type)
        return false;
      this.explorer.UpdateExplorationResult(((UpdateExplorationResultEvent) e).ExplorationResult);
      return true;
    }
  }
}
