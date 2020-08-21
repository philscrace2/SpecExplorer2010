// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ErrorReporting.WER_REPORT_INFORMATION
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System;
using System.Runtime.InteropServices;

namespace Company.VSPackage5
{
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  public struct WER_REPORT_INFORMATION
  {
    public uint dwSize;
    public IntPtr hProcess;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string wzConsentKey;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string wzFriendlyEventName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string wzApplicationName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string wzApplicationPath;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
    public string wzDescription;
    public IntPtr hwndParent;
  }
}
