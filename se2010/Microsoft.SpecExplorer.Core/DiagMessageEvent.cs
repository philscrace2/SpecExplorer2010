// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.DiagMessageEvent
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;

namespace Microsoft.SpecExplorer
{
  [Serializable]
  internal class DiagMessageEvent : ExplorerEvent
  {
    public DiagMessageEvent(DiagnosisKind kind, string message, object location)
      : base(ExplorerEventType.DiagMessage)
    {
      this.Kind = kind;
      this.Message = message;
      this.Location = location;
    }

    public DiagnosisKind Kind { get; private set; }

    public string Message { get; private set; }

    public object Location { get; private set; }
  }
}
