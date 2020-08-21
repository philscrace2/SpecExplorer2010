// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ErrorReporting.WER_REPORT_TYPE
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

namespace Company.VSPackage5
{
  public enum WER_REPORT_TYPE
  {
    WerReportNonCritical,
    WerReportCritical,
    WerReportApplicationCrash,
    WerReportApplicationHang,
    WerReportKernel,
    WerReportInvalid,
  }
}
