// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.CodeElementViewer
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
  
  public partial class CodeElementViewer : UserControl
  {
    //internal CodeElementViewerModel viewerModel;
    //private bool _contentLoaded;

    public CodeElementViewer()
    {
      this.InitializeComponent();
    }

    public CodeElementViewerModel ViewerModel
    {
      get
      {
        return this.viewerModel;
      }
    }

    //[DebuggerNonUserCode]
    //public void InitializeComponent()
    //{
    //  if (this._contentLoaded)
    //    return;
    //  this._contentLoaded = true;
    //  Application.LoadComponent((object) this, new Uri("/Microsoft.SpecExplorer.VS.Package;V2.2.0.0;component/assistedprocedures/codeelementviewer.xaml", UriKind.Relative));
    //}

    //[DebuggerNonUserCode]
    //[EditorBrowsable(EditorBrowsableState.Never)]
    //void IComponentConnector.Connect(int connectionId, object target)
    //{
    //  if (connectionId == 1)
    //    this.viewerModel = (CodeElementViewerModel) target;
    //  else
    //    this._contentLoaded = true;
    //}
  }
}
