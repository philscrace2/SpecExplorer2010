// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterItemsControl
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class SplitterItemsControl : LayoutSynchronizedItemsControl
  {
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof (Orientation), typeof (Orientation), typeof (SplitterItemsControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty SplitterGripSizeProperty = DependencyProperty.RegisterAttached("SplitterGripSize", typeof (double), typeof (SplitterItemsControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) 5.0, FrameworkPropertyMetadataOptions.Inherits));

    static SplitterItemsControl()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (SplitterItemsControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (SplitterItemsControl)));
    }

    public static double GetSplitterGripSize(DependencyObject element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      return (double) element.GetValue(SplitterItemsControl.SplitterGripSizeProperty);
    }

    public static void SetSplitterGripSize(DependencyObject element, double value)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      element.SetValue(SplitterItemsControl.SplitterGripSizeProperty, (object) value);
    }

    public Orientation Orientation
    {
      get
      {
        return (Orientation) this.GetValue(SplitterItemsControl.OrientationProperty);
      }
      set
      {
        this.SetValue(SplitterItemsControl.OrientationProperty, (object) value);
      }
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is SplitterItem;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
      return (DependencyObject) new SplitterItem();
    }
  }
}
