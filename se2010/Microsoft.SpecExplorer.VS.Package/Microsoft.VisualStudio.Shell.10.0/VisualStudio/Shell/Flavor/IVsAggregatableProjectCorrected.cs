// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Flavor.IVsAggregatableProjectCorrected
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Flavor
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("ffb2e715-7312-4b93-83d7-d37bcc561c90")]
  [CLSCompliant(false)]
  [ComImport]
  public interface IVsAggregatableProjectCorrected
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetInnerProject(IntPtr punkInnerIUnknown);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int InitializeForOuter(
      [MarshalAs(UnmanagedType.LPWStr)] string pszFilename,
      [MarshalAs(UnmanagedType.LPWStr)] string pszLocation,
      [MarshalAs(UnmanagedType.LPWStr)] string pszName,
      uint grfCreateFlags,
      ref Guid iidProject,
      out IntPtr ppvProject,
      out int pfCanceled);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int OnAggregationComplete();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetAggregateProjectTypeGuids([MarshalAs(UnmanagedType.BStr)] out string pbstrProjTypeGuids);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetAggregateProjectTypeGuids([MarshalAs(UnmanagedType.LPWStr)] string lpstrProjTypeGuids);
  }
}
