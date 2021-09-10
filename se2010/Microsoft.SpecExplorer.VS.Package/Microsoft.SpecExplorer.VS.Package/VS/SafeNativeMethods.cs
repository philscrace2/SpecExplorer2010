// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.SafeNativeMethods
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.SpecExplorer.VS
{
  public static class SafeNativeMethods
  {
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsChild(IntPtr hwndParent, IntPtr hwndChildTest);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr GetFocus();

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPos(
      IntPtr hwnd,
      IntPtr hwndBefore,
      int x,
      int y,
      int cx,
      int cy,
      int flags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
  }
}
