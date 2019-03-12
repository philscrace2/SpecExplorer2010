// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.SafeNativeMethods
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.VisualStudio
{
  [SuppressUnmanagedCodeSecurity]
  internal class SafeNativeMethods
  {
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool InvalidateRect(
      IntPtr hWnd,
      ref NativeMethods.RECT rect,
      bool erase);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool InvalidateRect(IntPtr hWnd, [MarshalAs(UnmanagedType.Interface)] object rect, bool erase);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsChild(IntPtr parent, IntPtr child);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    internal static extern int GetCurrentThreadId();

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern int MapWindowPoints(
      IntPtr hWndFrom,
      IntPtr hWndTo,
      [In, Out] ref NativeMethods.RECT rect,
      int cPoints);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern int MapWindowPoints(
      IntPtr hWndFrom,
      IntPtr hWndTo,
      [In, Out] NativeMethods.POINT pt,
      int cPoints);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern int RegisterWindowMessage(string msg);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetWindowRect(IntPtr hWnd, [In, Out] ref NativeMethods.RECT rect);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int DrawText(
      IntPtr hDC,
      string lpszString,
      int nCount,
      ref NativeMethods.RECT lpRect,
      int nFormat);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool OffsetRect([In, Out] ref NativeMethods.RECT lpRect, int dx, int dy);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetTextExtentPoint32(
      IntPtr hDC,
      string str,
      int len,
      [In, Out] NativeMethods.POINT ptSize);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr gdiObj);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern void DeleteObject(IntPtr gdiObj);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CreateSolidBrush(int crColor);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CreateFontIndirect([MarshalAs(UnmanagedType.AsAny), In, Out] object lf);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int SetTextColor(IntPtr hdc, int crColor);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int SetBkMode(IntPtr hdc, int nBkMode);

    [DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
    public static extern int GetCurrentThemeName(
      StringBuilder pszThemeFileName,
      int dwMaxNameChars,
      StringBuilder pszColorBuff,
      int dwMaxColorChars,
      StringBuilder pszSizeBuff,
      int cchMaxSizeChars);
  }
}
