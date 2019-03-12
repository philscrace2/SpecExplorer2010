// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.UnsafeNativeMethods
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.VisualStudio
{
  [SuppressUnmanagedCodeSecurity]
  internal class UnsafeNativeMethods
  {
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetFileAttributes(string name);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern void GetTempFileName(
      string tempDirName,
      string prefixName,
      int unique,
      StringBuilder sb);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern void DebugBreak();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(HandleRef handle);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool LoadString(
      HandleRef hInstance,
      int uID,
      StringBuilder lpBuffer,
      int nBufferMax);

    public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
    {
      if (IntPtr.Size == 4)
        return UnsafeNativeMethods.GetWindowLong32(hWnd, nIndex);
      return UnsafeNativeMethods.GetWindowLongPtr64(hWnd, nIndex);
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr SendMessage(
      IntPtr hWnd,
      int msg,
      IntPtr wParam,
      IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hwnd, int msg, bool wparam, int lparam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

    public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
      if (IntPtr.Size == 4)
        return UnsafeNativeMethods.SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
      return UnsafeNativeMethods.SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public static IntPtr SetWindowLong(IntPtr hWnd, short nIndex, IntPtr dwNewLong)
    {
      if (IntPtr.Size == 4)
        return UnsafeNativeMethods.SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
      return UnsafeNativeMethods.SetWindowLongPtr64(hWnd, (int) nIndex, dwNewLong);
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLongPtr32(
      IntPtr hWnd,
      short nIndex,
      IntPtr dwNewLong);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPos(
      IntPtr hWnd,
      IntPtr hWndInsertAfter,
      int x,
      int y,
      int cx,
      int cy,
      int flags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr GlobalReAlloc(HandleRef handle, int bytes, int flags);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr GlobalLock(HandleRef handle);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GlobalUnlock(HandleRef handle);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr GlobalFree(HandleRef handle);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    internal static extern int GlobalSize(HandleRef handle);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    internal static extern IntPtr GlobalLock(IntPtr h);

    [DllImport("kernel32.dll", EntryPoint = "GlobalUnlock", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GlobalUnLock(IntPtr h);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    internal static extern int GlobalSize(IntPtr h);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
    internal static extern void CopyMemoryW(IntPtr pdst, string psrc, int cb);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
    internal static extern void CopyMemoryW(IntPtr pdst, char[] psrc, int cb);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
    internal static extern void CopyMemoryW(StringBuilder pdst, HandleRef psrc, int cb);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
    internal static extern void CopyMemoryW(char[] pdst, HandleRef psrc, int cb);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
    internal static extern void CopyMemory(IntPtr pdst, byte[] psrc, int cb);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
    internal static extern void CopyMemory(byte[] pdst, HandleRef psrc, int cb);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
    internal static extern void CopyMemory(IntPtr pdst, HandleRef psrc, int cb);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
    internal static extern void CopyMemory(IntPtr pdst, string psrc, int cb);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    internal static extern int WideCharToMultiByte(
      int codePage,
      int flags,
      [MarshalAs(UnmanagedType.LPWStr)] string wideStr,
      int chars,
      [In, Out] byte[] pOutBytes,
      int bufferBytes,
      IntPtr defaultChar,
      IntPtr pDefaultUsed);

    [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
    internal static extern int OleSetClipboard(IDataObject dataObject);

    [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
    internal static extern int OleGetClipboard(out IDataObject dataObject);

    [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
    internal static extern int OleFlushClipboard();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern int OpenClipboard(IntPtr newOwner);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern int EmptyClipboard();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern int CloseClipboard();

    [DllImport("user32.dll", EntryPoint = "RegisterClipboardFormatW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    internal static extern ushort RegisterClipboardFormat(string format);

    [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr ImageList_Duplicate(HandleRef himl);

    [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
    public static extern int ImageList_GetImageCount(HandleRef himl);

    [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ImageList_Draw(
      HandleRef himl,
      int i,
      HandleRef hdcDst,
      int x,
      int y,
      int fStyle);

    [DllImport("shell32.dll", EntryPoint = "DragQueryFileW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern uint DragQueryFile(IntPtr hDrop, uint iFile, char[] lpszFile, uint cch);
  }
}
