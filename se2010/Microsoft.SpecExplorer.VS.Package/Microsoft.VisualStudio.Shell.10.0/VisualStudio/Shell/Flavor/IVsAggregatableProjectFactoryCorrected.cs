// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Flavor.IVsAggregatableProjectFactoryCorrected
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Flavor
{
  [Guid("44569501-2ad0-4966-9bac-12b799a1ced6")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IVsAggregatableProjectFactoryCorrected
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetAggregateProjectType([MarshalAs(UnmanagedType.LPWStr)] string fileName, [MarshalAs(UnmanagedType.BStr)] out string projectTypeGuid);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int PreCreateForOuter(IntPtr outerProjectIUnknown, out IntPtr projectIUnknown);
  }
}
