// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.Shell.Interop.IVsUIWpfHostPrivate
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.Shell.Interop
{
  [Guid("5B074157-F1C7-4A1B-BD32-66666F4E4B3B")]
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [ComImport]
  public interface IVsUIWpfHostPrivate
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int RaiseThreadMessage([MarshalAs(UnmanagedType.LPArray), In] MSG[] pMsg, out bool pResult);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int RaiseIdle();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int VerifySynchronizationContext();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int InvokeShutdown();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int EnableUnhandledExceptionDisplay();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int RegisterComponentForModalTracking(
      [MarshalAs(UnmanagedType.Interface), In] IOleComponentManager pComponentManager,
      [In] uint dwComponentId);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int CreateWPFUIElementContainer(
      [MarshalAs(UnmanagedType.LPWStr), In] string szWindowType,
      [In] IntPtr hWndParent,
      [MarshalAs(UnmanagedType.Interface), In] IVsUIElement pElement,
      [MarshalAs(UnmanagedType.Interface), In] IVsUIDataSource pDataSource,
      [MarshalAs(UnmanagedType.IUnknown), In] object pSite,
      [MarshalAs(UnmanagedType.Interface)] out IVsUIWPFElementContainerPrivate ppElementContainer);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int InvokeRender();
  }
}
