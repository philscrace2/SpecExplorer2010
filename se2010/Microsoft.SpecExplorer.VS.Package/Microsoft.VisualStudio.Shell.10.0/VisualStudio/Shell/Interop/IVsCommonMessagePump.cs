// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsCommonMessagePump
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [CompilerGenerated]
  [Guid("FB3B20F4-9C8E-454A-984B-B1334F790541")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [TypeIdentifier]
  [ComImport]
  public interface IVsCommonMessagePump
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int ModalWaitForObjects([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), In] IntPtr[] rgHandles, [In] uint cHandles, out uint pdwWaitResult);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int ModalWaitForObjectsWithClient(
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), In] IntPtr[] rgHandles,
      [In] uint cHandles,
      [MarshalAs(UnmanagedType.Interface), In] IVsCommonMessagePumpClientEvents pClient);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetTimeout([In] uint dwTimeoutInMilliseconds);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetAllowCancel([In] bool fAllowCancel);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetWaitText([MarshalAs(UnmanagedType.LPWStr), In] string pszWaitText);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetWaitTitle([MarshalAs(UnmanagedType.LPWStr), In] string pszWaitTitle);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetStatusBarText([MarshalAs(UnmanagedType.LPWStr), In] string pszStatusBarText);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int EnableRealProgress([In] bool fEnableRealProgress);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetProgressInfo([In] int iTotalSteps, [In] int iCurrentStep, [MarshalAs(UnmanagedType.LPWStr), In] string pszProgressText);
  }
}
