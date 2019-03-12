// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterGrip
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class SplitterGrip : Thumb
  {
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof (Orientation), typeof (Orientation), typeof (SplitterGrip), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Vertical));
    public static readonly DependencyProperty ResizeBehaviorProperty = DependencyProperty.Register(nameof (ResizeBehavior), typeof (GridResizeBehavior), typeof (SplitterGrip), (PropertyMetadata) new FrameworkPropertyMetadata((object) GridResizeBehavior.CurrentAndNext));

    static SplitterGrip()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (SplitterGrip), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (SplitterGrip)));
    }

    public SplitterGrip()
    {
      AutomationProperties.SetAutomationId((DependencyObject) this, nameof (SplitterGrip));
    }

    public Orientation Orientation
    {
      get
      {
        return (Orientation) this.GetValue(SplitterGrip.OrientationProperty);
      }
      set
      {
        this.SetValue(SplitterGrip.OrientationProperty, (object) value);
      }
    }

    public GridResizeBehavior ResizeBehavior
    {
      get
      {
        return (GridResizeBehavior) this.GetValue(SplitterGrip.ResizeBehaviorProperty);
      }
      set
      {
        this.SetValue(SplitterGrip.ResizeBehaviorProperty, (object) value);
      }
    }
  }
}
