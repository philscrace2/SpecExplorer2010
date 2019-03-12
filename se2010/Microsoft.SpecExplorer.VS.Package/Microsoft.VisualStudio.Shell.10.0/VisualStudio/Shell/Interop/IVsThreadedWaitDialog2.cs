// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsThreadedWaitDialog2
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [Guid("88194D8B-88DA-4C33-A2C6-15140626E222")]
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [ComImport]
  public interface IVsThreadedWaitDialog2
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int StartWaitDialog(
      [MarshalAs(UnmanagedType.LPWStr), In] string szWaitCaption,
      [MarshalAs(UnmanagedType.LPWStr), In] string szWaitMessage,
      [MarshalAs(UnmanagedType.LPWStr), In] string szProgressText,
      [MarshalAs(UnmanagedType.Struct), In] object varStatusBmpAnim,
      [MarshalAs(UnmanagedType.LPWStr), In] string szStatusBarText,
      [In] int iDelayToShowDialog,
      [In] bool fIsCancelable,
      [In] bool fShowMarqueeProgress);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int StartWaitDialogWithPercentageProgress(
      [MarshalAs(UnmanagedType.LPWStr), In] string szWaitCaption,
      [MarshalAs(UnmanagedType.LPWStr), In] string szWaitMessage,
      [MarshalAs(UnmanagedType.LPWStr), In] string szProgressText,
      [MarshalAs(UnmanagedType.Struct), In] object varStatusBmpAnim,
      [MarshalAs(UnmanagedType.LPWStr), In] string szStatusBarText,
      [In] bool fIsCancelable,
      [In] int iDelayToShowDialog,
      [In] int iTotalSteps,
      [In] int iCurrentStep);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int EndWaitDialog(out int pfCanceled);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int UpdateProgress(
      [MarshalAs(UnmanagedType.LPWStr), In] string szUpdatedWaitMessage,
      [MarshalAs(UnmanagedType.LPWStr), In] string szProgressText,
      [MarshalAs(UnmanagedType.LPWStr), In] string szStatusBarText,
      [In] int iCurrentStep,
      [In] int iTotalSteps,
      [In] bool fDisableCancel,
      out bool pfCanceled);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int HasCanceled(out bool pfCanceled);
  }
}
