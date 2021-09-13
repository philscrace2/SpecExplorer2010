// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.DiagMessageEventObserver
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

namespace Microsoft.SpecExplorer
{
  internal class DiagMessageEventObserver : EventObserver
  {
    private IHost host;

    internal DiagMessageEventObserver(IHost host) => this.host = host;

    internal override bool HandleEvent(ExplorerEvent e)
    {
      if (ExplorerEventType.DiagMessage != e.Type)
        return false;
      DiagMessageEvent diagMessageEvent = (DiagMessageEvent) e;
      this.host.DiagMessage(diagMessageEvent.Kind, diagMessageEvent.Message, diagMessageEvent.Location);
      return true;
    }
  }
}
