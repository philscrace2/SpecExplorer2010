// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUICollectionChangeEvents
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [Guid("D47ABBE0-4E31-424D-8DC9-31DE024E75E7")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [TypeIdentifier]
  [ComImport]
  public interface IVsUICollectionChangeEvents : IVsUIEventSink
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int Disconnect([MarshalAs(UnmanagedType.Interface), In] IVsUISimpleDataSource pSource);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int OnAfterItemAdded([MarshalAs(UnmanagedType.Interface), In] IVsUICollection pCollection, [In] uint nItem);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int OnAfterItemRemoved([MarshalAs(UnmanagedType.Interface), In] IVsUICollection pCollection, [MarshalAs(UnmanagedType.Interface), In] IVsUIDataSource pRemovedItem, [In] uint nItem);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int OnAfterItemReplaced(
      [MarshalAs(UnmanagedType.Interface), In] IVsUICollection pCollection,
      [MarshalAs(UnmanagedType.Interface), In] IVsUIDataSource pNewItem,
      [MarshalAs(UnmanagedType.Interface), In] IVsUIDataSource pOldItem,
      [In] uint nItem);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int OnInvalidateAllItems([MarshalAs(UnmanagedType.Interface), In] IVsUICollection pCollection);
  }
}
