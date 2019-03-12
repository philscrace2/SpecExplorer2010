// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterMeasureData
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class SplitterMeasureData
  {
    public SplitterMeasureData(UIElement element)
    {
      this.Element = element;
      this.AttachedLength = SplitterPanel.GetSplitterLength(element);
    }

    public static IList<SplitterMeasureData> FromElements(IList elements)
    {
      List<SplitterMeasureData> splitterMeasureDataList = new List<SplitterMeasureData>(elements.Count);
      foreach (UIElement element in (IEnumerable) elements)
        splitterMeasureDataList.Add(new SplitterMeasureData(element));
      return (IList<SplitterMeasureData>) splitterMeasureDataList;
    }

    public UIElement Element { get; private set; }

    public SplitterLength AttachedLength { get; set; }

    public bool IsMinimumReached { get; set; }

    public bool IsMaximumReached { get; set; }

    public Rect MeasuredBounds { get; set; }
  }
}
