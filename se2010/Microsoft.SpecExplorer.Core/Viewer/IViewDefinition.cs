// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.IViewDefinition
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System.Drawing;

namespace Microsoft.SpecExplorer.Viewer
{
  public interface IViewDefinition
  {
    string Name { get; }

    Color NodeFillColor { get; set; }

    Color EdgeColor { get; set; }

    bool ViewCollapseLabels { get; set; }

    bool ViewCollapseSteps { get; set; }

    bool IsDefault { get; }

    int RenderingTimeOut { get; set; }
  }
}
