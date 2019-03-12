// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.Shell.Interop.IOleComponent2Private
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.Shell.Interop
{
  [Guid("ED0751FC-D772-4D1D-88FC-0C1AA275391B")]
  [CLSCompliant(false)]
  [InterfaceType(1)]
  [ComImport]
  public interface IOleComponent2Private : IOleComponent
  {
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")]
    new int FReserved1([ComAliasName("Microsoft.VisualStudio.OLE.Interop.DWORD"), In] uint dwReserved, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.UINT"), In] uint message, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.UINT_PTR"), In] IntPtr wParam, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.LONG_PTR"), In] IntPtr lParam);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")]
    new int FPreTranslateMessage([ComAliasName("Microsoft.VisualStudio.OLE.Interop.MSG"), MarshalAs(UnmanagedType.LPArray), In, Out] MSG[] pMsg);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void OnEnterState([ComAliasName("Microsoft.VisualStudio.OLE.Interop.OLECSTATE"), In] uint uStateID, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL"), In] int fEnter);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void OnAppActivate([ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL"), In] int fActive, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.DWORD"), In] uint dwOtherThreadID);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void OnLoseActivation();

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void OnActivationChange(
      [MarshalAs(UnmanagedType.Interface), In] IOleComponent pic,
      [ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL"), In] int fSameComponent,
      [ComAliasName("Microsoft.VisualStudio.OLE.Interop.OLECRINFO"), MarshalAs(UnmanagedType.LPArray), In] OLECRINFO[] pcrinfo,
      [ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL"), In] int fHostIsActivating,
      [ComAliasName("Microsoft.VisualStudio.OLE.Interop.OLECHOSTINFO"), MarshalAs(UnmanagedType.LPArray), In] OLECHOSTINFO[] pchostinfo,
      [ComAliasName("Microsoft.VisualStudio.OLE.Interop.DWORD"), In] uint dwReserved);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")]
    new int FDoIdle([ComAliasName("Microsoft.VisualStudio.OLE.Interop.OLEIDLEF"), In] uint grfidlef);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")]
    new int FContinueMessageLoop([ComAliasName("Microsoft.VisualStudio.OLE.Interop.OLELOOP"), In] uint uReason, [In] IntPtr pvLoopData, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.MSG"), MarshalAs(UnmanagedType.LPArray), In] MSG[] pMsgPeeked);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")]
    new int FQueryTerminate([ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL"), In] int fPromptUser);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void Terminate();

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new IntPtr HwndGetWindow([ComAliasName("Microsoft.VisualStudio.OLE.Interop.OLECWINDOW"), In] uint dwWhich, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.DWORD"), In] uint dwReserved);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetWaitHandlesAndTimeout(
      [MarshalAs(UnmanagedType.LPArray, SizeConst = 64), Out] IntPtr[] handleArray,
      [ComAliasName("Microsoft.VisualStudio.OLE.Interop.UINT")] out uint handleCount,
      [ComAliasName("Microsoft.VisualStudio.OLE.Interop.DWORD")] out uint timeout,
      [In] IntPtr pvLoopData);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int OnHandleSignaled([ComAliasName("Microsoft.VisualStudio.OLE.Interop.UINT"), In] uint handleCount, [In] IntPtr pvLoopData, out bool pfContinue);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int OnTimeout([In] IntPtr pvLoopData, out bool pfContinue);
  }
}
