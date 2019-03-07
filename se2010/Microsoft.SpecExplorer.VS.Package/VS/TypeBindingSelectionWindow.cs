// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.TypeBindingSelectionWindow
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Microsoft.SpecExplorer.VS
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class TypeBindingSelectionWindow : Window, IComponentConnector
  {
    internal TypeBindingSelectionControlModel controlModel;
    internal RadioButton classCreationButton;
    internal RadioButton classSelectionButton;
    internal CodeElementViewer viewerControl;
    private bool _contentLoaded;

    public TypeBindingSelectionWindow()
    {
      this.InitializeComponent();
      this.controlModel.ViewerModel = this.viewerControl.ViewerModel;
    }

    public TypeBindingSelectionControlModel ControlModel
    {
      get
      {
        return this.controlModel;
      }
    }

    private void OKButtonClick(object sender, RoutedEventArgs evtArgs)
    {
      this.DialogResult = new bool?(true);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.SpecExplorer.VS.Package;V2.2.0.0;component/assistedprocedures/typebindingselectionwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.controlModel = (TypeBindingSelectionControlModel) target;
          break;
        case 2:
          this.classCreationButton = (RadioButton) target;
          break;
        case 3:
          this.classSelectionButton = (RadioButton) target;
          break;
        case 4:
          this.viewerControl = (CodeElementViewer) target;
          break;
        case 5:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.OKButtonClick);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
