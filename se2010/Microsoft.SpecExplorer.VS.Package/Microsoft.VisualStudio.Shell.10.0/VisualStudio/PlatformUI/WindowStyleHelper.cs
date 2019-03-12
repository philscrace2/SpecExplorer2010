// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.WindowStyleHelper
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public static class WindowStyleHelper
  {
    public static readonly DependencyProperty HasMaximizeButtonProperty = DependencyProperty.RegisterAttached("HasMaximizeButton", typeof (bool), typeof (WindowStyleHelper), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(WindowStyleHelper.OnWindowStyleChanged)));
    public static readonly DependencyProperty HasMinimizeButtonProperty = DependencyProperty.RegisterAttached("HasMinimizeButton", typeof (bool), typeof (WindowStyleHelper), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(WindowStyleHelper.OnWindowStyleChanged)));

    public static bool GetHasMaximizeButton(Window window)
    {
      if (window == null)
        throw new ArgumentNullException(nameof (window));
      return (bool) window.GetValue(WindowStyleHelper.HasMaximizeButtonProperty);
    }

    public static void SetHasMaximizeButton(Window window, bool value)
    {
      if (window == null)
        throw new ArgumentNullException(nameof (window));
      window.SetValue(WindowStyleHelper.HasMaximizeButtonProperty, (object) value);
    }

    public static bool GetHasMinimizeButton(Window window)
    {
      if (window == null)
        throw new ArgumentNullException(nameof (window));
      return (bool) window.GetValue(WindowStyleHelper.HasMinimizeButtonProperty);
    }

    public static void SetHasMinimizeButton(Window window, bool value)
    {
      if (window == null)
        throw new ArgumentNullException(nameof (window));
      window.SetValue(WindowStyleHelper.HasMinimizeButtonProperty, (object) value);
    }

    private static void OnWindowStyleChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs args)
    {
      Window window = (Window) obj;
      HwndSource hwndSource = PresentationSource.FromVisual((Visual) window) as HwndSource;
      if (hwndSource == null)
        return;
      WindowStyleHelper.UpdateWindowStyle(window, hwndSource.Handle);
    }

    private static void UpdateWindowStyle(Window window, IntPtr hwnd)
    {
      int windowLong = NativeMethods.GetWindowLong(hwnd, -16);
      int num1 = !WindowStyleHelper.GetHasMaximizeButton(window) ? windowLong & -65537 : windowLong | 65536;
      int num2 = !WindowStyleHelper.GetHasMinimizeButton(window) ? num1 & -131073 : num1 | 131072;
      NativeMethods.SetWindowLong(hwnd, (short) -16, num2);
    }
  }
}
