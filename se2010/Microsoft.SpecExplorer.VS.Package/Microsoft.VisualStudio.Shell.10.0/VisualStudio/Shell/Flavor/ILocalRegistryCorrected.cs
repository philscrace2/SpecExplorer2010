// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Flavor.ILocalRegistryCorrected
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Flavor
{
  [CLSCompliant(false)]
  [Guid("6d5140d3-7436-11ce-8034-00aa006009fa")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface ILocalRegistryCorrected
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int CreateInstance(
      Guid clsid,
      IntPtr punkOuterIUnknown,
      ref Guid riid,
      uint dwFlags,
      out IntPtr ppvObj);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetClassObjectOfClsid(
      ref Guid clsid,
      uint dwFlags,
      IntPtr lpReserved,
      ref Guid riid,
      out IntPtr ppvClassObject);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetTypeLibOfClsid(Guid clsid, out ITypeLib pptLib);
  }
}
