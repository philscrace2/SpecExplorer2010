// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Win32Methods
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio
{
  [CLSCompliant(false)]
  public class Win32Methods
  {
    [DllImport("User32", CharSet = CharSet.Auto)]
    public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

    [DllImport("user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern bool IsDialogMessageA(IntPtr hDlg, ref MSG msg);
  }
}
