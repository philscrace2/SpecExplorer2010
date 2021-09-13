// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.DefaultErrorReportProvider
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
  internal class DefaultErrorReportProvider : ComponentBase, IErrorReportProvider
  {
    private EventAdapter eventAdapter;

    internal DefaultErrorReportProvider(EventAdapter eventAdapter) => this.eventAdapter = eventAdapter;

    public void ReportError(string message) => this.eventAdapter.DiagMessage(DiagnosisKind.Error, message, (object) null);

    public void ReportWarning(string message) => this.eventAdapter.DiagMessage(DiagnosisKind.Warning, message, (object) null);

    public void ReportHint(string message) => this.eventAdapter.DiagMessage(DiagnosisKind.Hint, message, (object) null);
  }
}
