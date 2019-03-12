// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIDataSourcePropertyChangeEvents
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [Guid("EC495559-B090-435E-8D7E-3D95286A9BE8")]
  [ComImport]
  public interface IVsUIDataSourcePropertyChangeEvents : IVsUIEventSink
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int Disconnect([MarshalAs(UnmanagedType.Interface), In] IVsUISimpleDataSource pSource);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int OnPropertyChanged(
      [MarshalAs(UnmanagedType.Interface), In] IVsUIDataSource pDataSource,
      [MarshalAs(UnmanagedType.LPWStr), In] string prop,
      [MarshalAs(UnmanagedType.Interface), In] IVsUIObject pVarOld,
      [MarshalAs(UnmanagedType.Interface), In] IVsUIObject pVarNew);
  }
}
