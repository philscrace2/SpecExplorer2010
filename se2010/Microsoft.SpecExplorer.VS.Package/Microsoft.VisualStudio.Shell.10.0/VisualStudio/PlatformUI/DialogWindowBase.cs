// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.DialogWindowBase
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public abstract class DialogWindowBase : Window
  {
    public static readonly DependencyProperty HasMaximizeButtonProperty = DependencyProperty.Register(nameof (HasMaximizeButton), typeof (bool), typeof (DialogWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DialogWindowBase.OnWindowStyleChanged)));
    public static readonly DependencyProperty HasMinimizeButtonProperty = DependencyProperty.Register(nameof (HasMinimizeButton), typeof (bool), typeof (DialogWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DialogWindowBase.OnWindowStyleChanged)));
    public static readonly DependencyProperty HasDialogFrameProperty = DependencyProperty.Register(nameof (HasDialogFrame), typeof (bool), typeof (DialogWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(DialogWindowBase.OnWindowStyleChanged)));
    public static readonly DependencyProperty HasHelpButtonProperty = DependencyProperty.Register(nameof (HasHelpButton), typeof (bool), typeof (DialogWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DialogWindowBase.OnWindowStyleChanged)));
    private HwndSource hwndSource;

    static DialogWindowBase()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DialogWindowBase), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DialogWindowBase)));
    }

    public bool HasMaximizeButton
    {
      get
      {
        return (bool) this.GetValue(DialogWindowBase.HasMaximizeButtonProperty);
      }
      set
      {
        this.SetValue(DialogWindowBase.HasMaximizeButtonProperty, (object) value);
      }
    }

    public bool HasMinimizeButton
    {
      get
      {
        return (bool) this.GetValue(DialogWindowBase.HasMinimizeButtonProperty);
      }
      set
      {
        this.SetValue(DialogWindowBase.HasMinimizeButtonProperty, (object) value);
      }
    }

    public bool HasDialogFrame
    {
      get
      {
        return (bool) this.GetValue(DialogWindowBase.HasDialogFrameProperty);
      }
      set
      {
        this.SetValue(DialogWindowBase.HasDialogFrameProperty, (object) value);
      }
    }

    public bool HasHelpButton
    {
      get
      {
        return (bool) this.GetValue(DialogWindowBase.HasHelpButtonProperty);
      }
      set
      {
        this.SetValue(DialogWindowBase.HasHelpButtonProperty, (object) value);
      }
    }

    protected override void OnClosed(EventArgs e)
    {
      if (this.hwndSource != null)
      {
        this.hwndSource.Dispose();
        this.hwndSource = (HwndSource) null;
      }
      base.OnClosed(e);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
      this.hwndSource = (HwndSource) PresentationSource.FromVisual((Visual) this);
      this.hwndSource.AddHook(new HwndSourceHook(this.WndProcHook));
      this.UpdateWindowStyle(this.hwndSource.Handle);
      base.OnSourceInitialized(e);
    }

    private static void OnWindowStyleChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs args)
    {
      DialogWindow dialogWindow = (DialogWindow) obj;
      HwndSource hwndSource = dialogWindow.hwndSource;
      if (hwndSource == null)
        return;
      dialogWindow.UpdateWindowStyle(hwndSource.Handle);
    }

    private void UpdateWindowStyle(IntPtr hwnd)
    {
      int windowLong1 = NativeMethods.GetWindowLong(hwnd, -16);
      int num1 = !this.HasMaximizeButton ? windowLong1 & -65537 : windowLong1 | 65536;
      int num2 = !this.HasMinimizeButton ? num1 & -131073 : num1 | 131072;
      NativeMethods.SetWindowLong(hwnd, (short) -16, num2);
      int windowLong2 = NativeMethods.GetWindowLong(hwnd, -20);
      int num3 = !this.HasDialogFrame ? windowLong2 & -2 : windowLong2 | 1;
      int num4 = !this.HasHelpButton ? num3 & -1025 : num3 | 1024;
      NativeMethods.SetWindowLong(hwnd, (short) -20, num4);
      NativeMethods.SendMessage(hwnd, 128, new IntPtr(1), IntPtr.Zero);
      NativeMethods.SendMessage(hwnd, 128, new IntPtr(0), IntPtr.Zero);
    }

    private IntPtr WndProcHook(
      IntPtr hwnd,
      int msg,
      IntPtr wParam,
      IntPtr lParam,
      ref bool handled)
    {
      if (msg == 274 && wParam.ToInt32() == 61824)
      {
        this.InvokeDialogHelp();
        handled = true;
      }
      if (msg == 256 && wParam.ToInt32() == 112)
      {
        this.InvokeDialogHelp();
        handled = true;
      }
      if (msg == 26 && wParam.ToInt32() == 67 || msg == 21)
      {
        this.OnDialogThemeChanged();
        handled = true;
      }
      return IntPtr.Zero;
    }

    protected virtual void OnDialogThemeChanged()
    {
    }

    protected abstract void InvokeDialogHelp();
  }
}
