// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ModelingGuidance.IActivityReference
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System.ComponentModel;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
  public interface IActivityReference : INotifyPropertyChanged
  {
    IActivity Activity { get; }

    bool IsCompleted { get; set; }

    bool IsSelected { get; set; }

    int Index { get; }

    bool IsOptional { get; }
  }
}
