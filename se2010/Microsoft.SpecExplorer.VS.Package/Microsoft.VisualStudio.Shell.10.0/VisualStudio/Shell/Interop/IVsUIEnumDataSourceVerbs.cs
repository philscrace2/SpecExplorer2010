// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIEnumDataSourceVerbs
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [TypeIdentifier]
  [Guid("51C2FFFB-35FA-4AD2-81B1-11816C482AAA")]
  [ComImport]
  public interface IVsUIEnumDataSourceVerbs
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Next([In] uint celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.BStr), Out] string[] rgelt, out uint pceltFetched);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Reset();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Clone([MarshalAs(UnmanagedType.Interface)] out IVsUIEnumDataSourceVerbs ppEnum);
  }
}
