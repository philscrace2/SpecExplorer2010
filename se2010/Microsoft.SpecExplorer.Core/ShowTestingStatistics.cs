// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ShowTestingStatistics
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using System;

namespace Microsoft.SpecExplorer
{
  [Serializable]
  internal class ShowTestingStatistics : ExplorerEvent
  {
    public TestingStatistics Statistics { get; private set; }

    public ShowTestingStatistics(TestingStatistics statistics)
      : base(ExplorerEventType.ShowTestingStatistics)
    {
      this.Statistics = statistics != null ? statistics : throw new ArgumentNullException(nameof (statistics));
    }
  }
}
