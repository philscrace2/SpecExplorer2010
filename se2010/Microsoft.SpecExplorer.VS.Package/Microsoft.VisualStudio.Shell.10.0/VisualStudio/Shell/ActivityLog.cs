// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ActivityLog
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Shell
{
  public static class ActivityLog
  {
    public static string LogFilePath
    {
      get
      {
        object pvar;
        ErrorHandler.ThrowOnFailure(ActivityLog.Shell.GetProperty(-9064, out pvar));
        return (string) pvar;
      }
    }

    public static void LogError(string source, string message)
    {
      ActivityLog.LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, source, message);
    }

    public static void LogWarning(string source, string message)
    {
      ActivityLog.LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_WARNING, source, message);
    }

    public static void LogInformation(string source, string message)
    {
      ActivityLog.LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, source, message);
    }

    private static void LogEntry(__ACTIVITYLOG_ENTRYTYPE type, string source, string message)
    {
      ErrorHandler.ThrowOnFailure(ActivityLog.Log.LogEntry((uint) type, source, message));
    }

    private static IVsActivityLog Log
    {
      get
      {
        return ServiceProvider.GlobalProvider.GetService<IVsActivityLog>(typeof (SVsActivityLog));
      }
    }

    private static IVsShell Shell
    {
      get
      {
        return ServiceProvider.GlobalProvider.GetService<IVsShell>(typeof (SVsShell));
      }
    }
  }
}
