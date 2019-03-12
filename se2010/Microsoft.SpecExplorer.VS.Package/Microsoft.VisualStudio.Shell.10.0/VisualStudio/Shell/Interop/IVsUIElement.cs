// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIElement
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("62C0A03E-4979-4B4E-90F0-56DF90521F79")]
  [CompilerGenerated]
  [ComImport]
  public interface IVsUIElement
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int get_DataSource([MarshalAs(UnmanagedType.Interface)] out IVsUISimpleDataSource ppDataSource);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int put_DataSource([MarshalAs(UnmanagedType.Interface), In] IVsUISimpleDataSource pDataSource);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int TranslateAccelerator([MarshalAs(UnmanagedType.Interface), In] IVsUIAccelerator pAccel);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetUIObject([MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);
  }
}
