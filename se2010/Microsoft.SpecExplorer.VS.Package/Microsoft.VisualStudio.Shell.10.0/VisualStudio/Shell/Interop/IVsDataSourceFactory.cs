// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsDataSourceFactory
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [CompilerGenerated]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [TypeIdentifier]
  [Guid("24034437-CB2E-47DD-AE2B-14D56481A2F0")]
  [ComImport]
  public interface IVsDataSourceFactory
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetDataSource([In] ref Guid guid, [In] uint dw, [MarshalAs(UnmanagedType.Interface)] out IVsUIDataSource ppUIDataSource);
  }
}
