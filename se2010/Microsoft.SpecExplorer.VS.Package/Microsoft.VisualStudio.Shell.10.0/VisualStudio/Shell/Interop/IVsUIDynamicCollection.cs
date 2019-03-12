// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIDynamicCollection
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [TypeIdentifier]
  [Guid("6D155041-B4B8-4121-8D74-841E5DA4373E")]
  [CompilerGenerated]
  [ComImport]
  public interface IVsUIDynamicCollection : IVsUICollection, IVsUISimpleDataSource, IVsUIDispatch
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int Invoke([MarshalAs(UnmanagedType.LPWStr), In] string verb, [MarshalAs(UnmanagedType.Struct), In] object pvaIn, [MarshalAs(UnmanagedType.Struct)] out object pvaOut);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int EnumVerbs([MarshalAs(UnmanagedType.Interface)] out IVsUIEnumDataSourceVerbs ppEnum);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int Close();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int get_Count(out uint pnCount);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int GetItem([In] uint nItem, [MarshalAs(UnmanagedType.Interface)] out IVsUIDataSource pVsUIDataSource);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int AdviseCollectionChangeEvents([MarshalAs(UnmanagedType.Interface), In] IVsUICollectionChangeEvents pAdvise, out uint pCookie);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int UnadviseCollectionChangeEvents([In] uint cookie);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int AddItem([MarshalAs(UnmanagedType.Interface), In] IVsUIDataSource pItem, out uint pIndex);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int InsertItem([In] uint nIndex, [MarshalAs(UnmanagedType.Interface), In] IVsUIDataSource pItem);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int RemoveItem([In] uint nIndex);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int ReplaceItem([In] uint nIndex, [MarshalAs(UnmanagedType.Interface), In] IVsUIDataSource pItem);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int ClearItems();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int InsertCollection([In] uint nIndex, [MarshalAs(UnmanagedType.Interface), In] IVsUICollection pCollection);
  }
}
