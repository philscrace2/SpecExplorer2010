// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ErrorReporting.NativeMethods
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.SpecExplorer.ErrorReporting
{
  public class NativeMethods
  {
    public const int WER_FILE_DELETE_WHEN_DONE = 1;
    public const int WER_FILE_ANONYMOUS_DATA = 2;
    public const int WER_SUBMIT_HONOR_RECOVERY = 1;
    public const int WER_SUBMIT_HONOR_RESTART = 2;
    public const int WER_SUBMIT_QUEUE = 4;
    public const int WER_SUBMIT_SHOW_DEBUG = 8;
    public const int WER_SUBMIT_ADD_REGISTERED_DATA = 16;
    public const int WER_SUBMIT_OUTOFPROCESS = 32;
    public const int WER_SUBMIT_NO_CLOSE_UI = 64;
    public const int WER_SUBMIT_NO_QUEUE = 128;
    public const int WER_SUBMIT_NO_ARCHIVE = 256;
    public const int WER_SUBMIT_START_MINIMIZED = 512;
    public const int WER_SUBMIT_OUTOFPROCESS_ASYNC = 1024;

    [DllImport("Wer.dll")]
    public static extern int WerReportCreate(
      [MarshalAs(UnmanagedType.LPWStr), In] string pwzEventType,
      WER_REPORT_TYPE repType,
      [In] IntPtr pReportInformation,
      ref IntPtr phReportHandle);

    [DllImport("Wer.dll")]
    public static extern int WerReportSetParameter(
      [In] IntPtr hReportHandle,
      uint dwparamID,
      [MarshalAs(UnmanagedType.LPWStr), In] string pwzName,
      [MarshalAs(UnmanagedType.LPWStr), In] string pwzValue);

    [DllImport("Wer.dll")]
    public static extern int WerReportSubmit(
      [In] IntPtr hReportHandle,
      WER_CONSENT consent,
      uint dwFlags,
      IntPtr pSubmitResult);

    [DllImport("Wer.dll")]
    public static extern int WerReportCloseHandle([In] IntPtr hReportHandle);

    [DllImport("kernel32.dll")]
    public static extern int WerRegisterFile(
      [MarshalAs(UnmanagedType.LPWStr), In] string pwzFile,
      WER_REGISTER_FILE_TYPE regFileType,
      uint dwFlags);

    [DllImport("Wer.dll")]
    public static extern int WerReportSetUIOption(
      [In] IntPtr hReportHandle,
      WER_REPORT_UI repUITypeID,
      [MarshalAs(UnmanagedType.LPWStr), In] string pwzValue);
  }
}
