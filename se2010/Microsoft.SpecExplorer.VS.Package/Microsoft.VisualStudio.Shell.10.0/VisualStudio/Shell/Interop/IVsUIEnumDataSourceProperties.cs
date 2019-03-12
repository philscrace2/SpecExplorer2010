// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIEnumDataSourceProperties
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [CompilerGenerated]
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("327922B7-0B7F-4123-8446-0E614B337673")]
  [ComImport]
  public interface IVsUIEnumDataSourceProperties
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Next([In] uint celt, [MarshalAs(UnmanagedType.LPArray), Out] VsUIPropertyDescriptor[] rgelt, out uint pceltFetched);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Reset();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Clone([MarshalAs(UnmanagedType.Interface)] out IVsUIEnumDataSourceProperties ppEnum);
  }
}
