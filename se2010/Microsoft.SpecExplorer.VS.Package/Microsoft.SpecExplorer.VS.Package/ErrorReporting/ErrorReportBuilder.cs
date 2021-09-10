// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ErrorReporting.ErrorReportBuilder
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.VisualStudio;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.SpecExplorer.ErrorReporting
{
  public class ErrorReportBuilder
  {
    private const int MaxLength = 30;
    private IHost host;

    public ErrorReportBuilder(IHost host)
    {
      this.host = host;
    }

    private void GenerateGenericEventParameters(
      IntPtr phReportHandle,
      string stackTraceMessage,
      Exception exception)
    {
      string localPath = new Uri(typeof (ErrorReportBuilder).Assembly.CodeBase).LocalPath;
      string pwzValue1;
      string pwzValue2;
      if (!string.IsNullOrEmpty(localPath) && File.Exists(localPath))
      {
        pwzValue1 = FileVersionInfo.GetVersionInfo(localPath).FileVersion;
        pwzValue2 = File.GetCreationTime(localPath).Ticks.ToString("x");
      }
      else
      {
        pwzValue1 = "Can not get app version.";
        pwzValue2 = "Can not get app time stamp.";
      }
      int num1 = NativeMethods.WerReportSetParameter(phReportHandle, 0U, "AppName", Resources.ApplicationName);
      if (num1 != 0)
      {
        this.host.ProgressMessage((VerbosityLevel) 0, string.Format("Set Parameter AppName failed with Error Code {0}", (object) num1));
      }
      else
      {
        int num2 = NativeMethods.WerReportSetParameter(phReportHandle, 1U, "AppVer", pwzValue1);
        if (num2 != 0)
        {
          this.host.ProgressMessage((VerbosityLevel) 0, string.Format("Set Parameter AppVer failed with Error Code {0}", (object) num2));
        }
        else
        {
          int num3 = NativeMethods.WerReportSetParameter(phReportHandle, 2U, "AppStamp", pwzValue2);
          if (num3 != 0)
          {
            this.host.ProgressMessage((VerbosityLevel) 0, string.Format("Set Parameter AppStamp failed with Error Code {0}", (object) num3));
          }
          else
          {
            int num4 = NativeMethods.WerReportSetParameter(phReportHandle, 3U, "AsmAndModName", Resources.SEMainAssemblyName);
            if (num4 != 0)
            {
              this.host.ProgressMessage((VerbosityLevel) 0, string.Format("Set Parameter AsmAndModName failed with Error Code {0}", (object) num4));
            }
            else
            {
              int num5 = NativeMethods.WerReportSetParameter(phReportHandle, 4U, "ExceptionStackHash", this.GenerateHashCode(stackTraceMessage));
              if (num5 != 0)
              {
                this.host.ProgressMessage((VerbosityLevel) 0, string.Format("Set Parameter ExceptionStackHash failed with Error Code {0}", (object) num5));
              }
              else
              {
                if (exception == null)
                  return;
                string pwzValue3 = this.EnsureShorterThanMaxLength(exception.InnerException == null ? exception.GetType().ToString() : exception.InnerException.GetType().ToString());
                int num6 = NativeMethods.WerReportSetParameter(phReportHandle, 5U, "ExceptionType", pwzValue3);
                if (num6 != 0)
                {
                  this.host.ProgressMessage((VerbosityLevel) 0, string.Format("Set Parameter ExceptionType failed with Error Code {0}", (object) num6));
                }
                else
                {
                  string pwzValue4 = this.EnsureShorterThanMaxLength(exception.Message);
                  int num7 = NativeMethods.WerReportSetParameter(phReportHandle, 6U, "ExceptionErrorcode", pwzValue4);
                  if (num7 == 0)
                    return;
                  this.host.ProgressMessage((VerbosityLevel) 0, string.Format("Set Parameter ExceptionErrorcode failed with Error Code {0}", (object) num7));
                }
              }
            }
          }
        }
      }
    }

    private string EnsureShorterThanMaxLength(string value)
    {
      if (value.Length > 30)
        return value.Substring(0, 30);
      return value;
    }

    private string GenerateHashCode(string stackTraceMessage)
    {
      byte[] hash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(stackTraceMessage));
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < hash.Length; ++index)
        stringBuilder.Append(hash[index].ToString("x2"));
      return stringBuilder.ToString();
    }

    public void GenerateErrorReport(string stackTrace, Exception exception)
    {
      IntPtr num1 = new IntPtr();
      IntPtr num2 = new IntPtr();
      try
      {
        bool flag = false;
        OperatingSystem osVersion = Environment.OSVersion;
        if (osVersion.Platform == PlatformID.Win32NT && osVersion.Version.Major == 6)
          flag = true;
        if (!flag)
          return;
        WER_REPORT_INFORMATION reportInformation = new WER_REPORT_INFORMATION()
        {
          wzDescription = "Error report for Spec Explorer 2010",
          wzConsentKey = "PT3TestKey",
          wzFriendlyEventName = "",
          wzApplicationName = "Spec Explorer",
          wzApplicationPath = ""
        };
        reportInformation.hwndParent = reportInformation.hProcess;
        reportInformation.dwSize = (uint) Marshal.SizeOf((object) reportInformation);
        num1 = Marshal.AllocCoTaskMem(Marshal.SizeOf((object) reportInformation));
        Marshal.StructureToPtr((object) reportInformation, num1, true);
        IntPtr phReportHandle = new IntPtr();
        int num3 = NativeMethods.WerReportCreate("SEVS", WER_REPORT_TYPE.WerReportCritical, num1, ref phReportHandle);
        if (num3 != 0)
        {
          this.host.ProgressMessage((VerbosityLevel) 0, string.Format("Create Report failed with Error Code {0}", (object) num3));
        }
        else
        {
          this.GenerateGenericEventParameters(phReportHandle, stackTrace, exception);
          string tempFileName = Path.GetTempFileName();
          using (FileStream fileStream = new FileStream(tempFileName, FileMode.OpenOrCreate))
          {
            StreamWriter streamWriter = new StreamWriter((Stream) fileStream);
            streamWriter.WriteLine(stackTrace);
            streamWriter.Close();
            fileStream.Close();
          }
          int num4 = NativeMethods.WerRegisterFile(tempFileName, WER_REGISTER_FILE_TYPE.WerRegFileTypeOther, 3U);
          if (num4 != 0)
          {
            this.host.ProgressMessage((VerbosityLevel) 0, string.Format("Register File failed with Error Code {0}", (object) num4));
          }
          else
          {
            num2 = Marshal.AllocHGlobal(4);
            if (!ErrorHandler.Succeeded(NativeMethods.WerReportSetUIOption(phReportHandle, WER_REPORT_UI.WerUIConsentDlgHeader, "Spec Explorer has encountered an unexpected error")))
              this.host.ProgressMessage((VerbosityLevel) 0, "Failed to set UI option WerUIConsentDlgHeader.");
            else if (!ErrorHandler.Succeeded(NativeMethods.WerReportSetUIOption(phReportHandle, WER_REPORT_UI.WerUICloseDlgHeader, "Spec Explorer has encountered an unexpected error")))
              this.host.ProgressMessage((VerbosityLevel) 0, "Failed to set UI option WerUICloseDlgHeader.");
            else if (!ErrorHandler.Succeeded(NativeMethods.WerReportSetUIOption(phReportHandle, WER_REPORT_UI.WerUICloseDlgBody, "A problem caused the program to stop working correctly, Spec Explorer will recover from unexpected error.")))
              this.host.ProgressMessage((VerbosityLevel) 0, "Failed to set UI option WerUICloseDlgBody.");
            else if (!ErrorHandler.Succeeded(NativeMethods.WerReportSetUIOption(phReportHandle, WER_REPORT_UI.WerUICloseDlgButtonText, "Close")))
            {
              this.host.ProgressMessage((VerbosityLevel) 0, "Failed to set UI option WerUICloseDlgButtonText.");
            }
            else
            {
              int num5 = NativeMethods.WerReportSubmit(phReportHandle, WER_CONSENT.WerConsentApproved, 16U, num2);
              switch (Marshal.ReadInt32(num2))
              {
                case 1:
                  this.host.ProgressMessage((VerbosityLevel) 0, "The report was queued.");
                  break;
                case 2:
                  this.host.ProgressMessage((VerbosityLevel) 0, "The report was uploaded.");
                  break;
                case 3:
                  this.host.ProgressMessage((VerbosityLevel) 0, "The Debug button was clicked.");
                  break;
                case 4:
                  this.host.ProgressMessage((VerbosityLevel) 0, "The report submission failed.");
                  break;
                case 5:
                  this.host.ProgressMessage((VerbosityLevel) 0, "Error reporting was disabled.");
                  break;
                case 6:
                  this.host.ProgressMessage((VerbosityLevel) 0, "The report was canceled.");
                  break;
                case 7:
                  this.host.ProgressMessage((VerbosityLevel) 0, "Queuing was disabled.");
                  break;
                case 8:
                  this.host.ProgressMessage((VerbosityLevel) 0, "The report was asynchronous.");
                  break;
              }
              if (num5 != 0)
              {
                this.host.ProgressMessage((VerbosityLevel) 0, string.Format("Error Report submission failed with error code {0}", (object) num5));
              }
              else
              {
                int num6 = NativeMethods.WerReportCloseHandle(phReportHandle);
                if (num6 == 0)
                  return;
                this.host.ProgressMessage((VerbosityLevel) 0, string.Format("Close Handle failed with Error Code {0}", (object) num6));
              }
            }
          }
        }
      }
      finally
      {
        if (num1 != IntPtr.Zero)
          Marshal.FreeCoTaskMem(num1);
        if (num2 != IntPtr.Zero)
          Marshal.FreeHGlobal(num2);
      }
    }
  }
}
