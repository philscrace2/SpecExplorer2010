// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.UIWin32ElementWrapper
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Microsoft.VisualStudio.Shell
{
  internal class UIWin32ElementWrapper : IVsUIWin32Element, IVsBroadcastMessageEvents, IDisposable
  {
    private IVsShell _vsShell;
    private uint _broadcastEventCookie;
    private WindowPane _pane;

    internal UIWin32ElementWrapper(WindowPane pane)
    {
      this._pane = pane;
    }

    public int Create(IntPtr parent, out IntPtr pHandle)
    {
      IntPtr handle = this._pane.Window.Handle;
      int num1 = ((int) UnsafeNativeMethods.GetWindowLong(handle, -16) | 1409286144) & 1588658175;
      UnsafeNativeMethods.SetWindowLong(handle, -16, (IntPtr) num1);
      int num2 = (int) UnsafeNativeMethods.GetWindowLong(handle, -20) & -263374;
      UnsafeNativeMethods.SetWindowLong(handle, -20, (IntPtr) num2);
      UnsafeNativeMethods.SetParent(handle, parent);
      UnsafeNativeMethods.ShowWindow(handle, 1);
      if (this._vsShell == null)
      {
        this._vsShell = ServiceProvider.GlobalProvider.GetService(typeof (SVsShell)) as IVsShell;
        if (this._vsShell != null)
          NativeMethods.ThrowOnFailure(this._vsShell.AdviseBroadcastMessages((IVsBroadcastMessageEvents) this, out this._broadcastEventCookie));
      }
      pHandle = handle;
      return 0;
    }

    public int Destroy()
    {
      throw new NotImplementedException();
    }

    public int GetHandle(out IntPtr pHandle)
    {
      pHandle = this._pane.Window.Handle;
      return 0;
    }

    public int ShowModal(IntPtr parent, out int pDlgResult)
    {
      throw new NotImplementedException();
    }

    int IVsBroadcastMessageEvents.OnBroadcastMessage(
      uint msg,
      IntPtr wParam,
      IntPtr lParam)
    {
      int num = 0;
      if (!UnsafeNativeMethods.PostMessage(this._pane.Window.Handle, (int) msg, wParam, wParam))
        num = -2147467259;
      return num;
    }

    public void Dispose()
    {
      if (this._vsShell == null)
        return;
      try
      {
        this._vsShell.UnadviseBroadcastMessages(this._broadcastEventCookie);
      }
      catch (Exception ex)
      {
      }
      this._vsShell = (IVsShell) null;
      this._broadcastEventCookie = 0U;
    }
  }
}
