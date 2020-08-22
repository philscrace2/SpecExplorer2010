// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ActionSelectionControl
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.SpecExplorer.VS
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public partial class ActionSelectionControl : UserControl
  {
    //internal ActionSelectionControl actionSelectionControl;
    //internal ActionSelectionControlModel controlModel;
    //private bool _contentLoaded;

    public ActionSelectionControl()
    {
      this.InitializeComponent();
    }

    public ActionSelectionControlModel ControlModel
    {
      get
      {
        return this.controlModel;
      }
    }

    //[DebuggerNonUserCode]
    //public void InitializeComponent()
    //{
    //  if (this._contentLoaded)
    //    return;
    //  this._contentLoaded = true;
    //  Application.LoadComponent((object) this, new Uri("/Microsoft.SpecExplorer.VS.Package;V2.2.0.0;component/assistedprocedures/actionselectioncontrol.xaml", UriKind.Relative));
    //}

    //[EditorBrowsable(EditorBrowsableState.Never)]
    //[DebuggerNonUserCode]
    //void IComponentConnector.Connect(int connectionId, object target)
    //{
    //  switch (connectionId)
    //  {
    //    case 1:
    //      this.actionSelectionControl = (ActionSelectionControl) target;
    //      break;
    //    case 2:
    //      this.controlModel = (ActionSelectionControlModel) target;
    //      break;
    //    default:
    //      this._contentLoaded = true;
    //      break;
    //  }
    //}
  }
}
