// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterResizePreviewWindow
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class SplitterResizePreviewWindow : Control
  {
    private HwndSource hwndSource;

    static SplitterResizePreviewWindow()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (SplitterResizePreviewWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (SplitterResizePreviewWindow)));
    }

    public void Move(double deviceLeft, double deviceTop)
    {
      if (this.hwndSource == null)
        return;
      NativeMethods.SetWindowPos(this.hwndSource.Handle, IntPtr.Zero, (int) deviceLeft, (int) deviceTop, 0, 0, 85);
    }

    public void Show(UIElement parentElement)
    {
      HwndSource hwndSource = PresentationSource.FromVisual((Visual) parentElement) as HwndSource;
      this.EnsureWindow(hwndSource == null ? IntPtr.Zero : hwndSource.Handle);
      Point screen = parentElement.PointToScreen(new Point(0.0, 0.0));
      Size deviceUnits = parentElement.RenderSize.LogicalToDeviceUnits();
      NativeMethods.SetWindowPos(this.hwndSource.Handle, IntPtr.Zero, (int) screen.X, (int) screen.Y, (int) deviceUnits.Width, (int) deviceUnits.Height, 84);
    }

    public void Hide()
    {
      using (this.hwndSource)
        this.hwndSource = (HwndSource) null;
    }

    private void EnsureWindow(IntPtr owner)
    {
      if (this.hwndSource != null)
        return;
      HwndSourceParameters parameters = new HwndSourceParameters(nameof (SplitterResizePreviewWindow));
      int num = -2013265880;
      parameters.Width = 0;
      parameters.Height = 0;
      parameters.PositionX = 0;
      parameters.PositionY = 0;
      parameters.WindowStyle = num;
      parameters.UsesPerPixelOpacity = true;
      parameters.ParentWindow = owner;
      this.hwndSource = new HwndSource(parameters);
      this.hwndSource.SizeToContent = SizeToContent.Manual;
      this.hwndSource.RootVisual = (Visual) this;
    }
  }
}
