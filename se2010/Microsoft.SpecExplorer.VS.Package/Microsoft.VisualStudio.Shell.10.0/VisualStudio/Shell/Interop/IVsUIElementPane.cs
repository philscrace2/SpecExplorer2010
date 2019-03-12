// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIElementPane
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [TypeIdentifier]
  [Guid("D5083078-66C7-4047-B101-62A5F7997EC5")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [ComImport]
  public interface IVsUIElementPane
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetUIElementSite([MarshalAs(UnmanagedType.Interface), In] IServiceProvider pSP);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int CreateUIElementPane([MarshalAs(UnmanagedType.IUnknown)] out object punkUIElement);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetDefaultUIElementSize([MarshalAs(UnmanagedType.LPArray), Out] SIZE[] psize);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int CloseUIElementPane();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int LoadUIElementState([MarshalAs(UnmanagedType.Interface), In] IStream pstream);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SaveUIElementState([MarshalAs(UnmanagedType.Interface), In] IStream pstream);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int TranslateUIElementAccelerator([MarshalAs(UnmanagedType.LPArray)] MSG[] lpmsg);
  }
}
