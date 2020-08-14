// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.AssistedProcedureReportWindow
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using Microsoft.SpecExplorer.VS;

namespace Microsoft.SpecExplorer.assistedprocedures
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public partial class AssistedProcedureReportWindow : Window
  {
    //internal AssistedProcedureReportWindow reportWindow;
    //private bool _contentLoaded;

    public AssistedProcedureReportWindow(ScriptManipulationReport report, string title)
    {
      this.GenerateMessages(report);
      this.InitializeComponent();
      this.Title = title;
    }

    public string ReportMessage { get; set; }

    private void GenerateMessages(ScriptManipulationReport report)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string insertedClause in (IEnumerable<string>) report.InsertedClauses)
        stringBuilder.AppendLine("+ " + insertedClause);
      this.ReportMessage = stringBuilder.ToString();
    }

    private void OKButton_Clicked(object sender, RoutedEventArgs evtArgs)
    {
      this.DialogResult = new bool?(true);
    }

    //[DebuggerNonUserCode]
    //public void InitializeComponent()
    //{
    //    if (this._contentLoaded)
    //        return;
    //    this._contentLoaded = true;
    //    Application.LoadComponent((object)this, new Uri("/Microsoft.SpecExplorer.VS.Package;V2.2.0.0;component/assistedprocedures/assistedprocedurereportwindow.xaml", UriKind.Relative));
    //}

    //[EditorBrowsable(EditorBrowsableState.Never)]
    //[DebuggerNonUserCode]
    //void IComponentConnector.Connect(int connectionId, object target)
    //{
    //    switch (connectionId)
    //    {
    //        case 1:
    //            this.reportWindow = (AssistedProcedureReportWindow)target;
    //            break;
    //        case 2:
    //            ((ButtonBase)target).Click += new RoutedEventHandler(this.OKButton_Clicked);
    //            break;
    //        default:
    //            this._contentLoaded = true;
    //            break;
    //    }
    //}
  }
}
