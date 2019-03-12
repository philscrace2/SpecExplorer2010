// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUICollection
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [Guid("F9362B93-C6FD-4C51-8AF9-B4BC13953E6C")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [TypeIdentifier]
  [ComImport]
  public interface IVsUICollection : IVsUISimpleDataSource, IVsUIDispatch
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int Invoke([MarshalAs(UnmanagedType.LPWStr), In] string verb, [MarshalAs(UnmanagedType.Struct), In] object pvaIn, [MarshalAs(UnmanagedType.Struct)] out object pvaOut);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int EnumVerbs([MarshalAs(UnmanagedType.Interface)] out IVsUIEnumDataSourceVerbs ppEnum);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int Close();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int get_Count(out uint pnCount);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetItem([In] uint nItem, [MarshalAs(UnmanagedType.Interface)] out IVsUIDataSource pVsUIDataSource);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int AdviseCollectionChangeEvents([MarshalAs(UnmanagedType.Interface), In] IVsUICollectionChangeEvents pAdvise, out uint pCookie);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int UnadviseCollectionChangeEvents([In] uint cookie);
  }
}
