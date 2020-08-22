// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.StateComparisonView
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.SpecExplorer.Viewer;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Microsoft.SpecExplorer.VS.Package55;

namespace Microsoft.SpecExplorer.VS
{
  [Guid("C84398D1-E949-42f9-9BBB-C794D39E6361")]
  public class StateComparisonView : ToolWindowPane
  {
    private ElementHost elementHost;
    private StateComparisonControl control;

    public StateComparisonView()
      : base((System.IServiceProvider) null)
    {
      this.control = new StateComparisonControl();
      this.elementHost = new ElementHost();
      this.elementHost.Child = (UIElement) this.control;
      this.elementHost.Dock = DockStyle.Fill;
    }

    internal void Show()
    {
      IVsWindowFrame frame = this.Frame as IVsWindowFrame;
      if (frame == null)
        return;
      ErrorHandler.ThrowOnFailure(frame.Show());
    }

    internal void Hide()
    {
      IVsWindowFrame frame = this.Frame as IVsWindowFrame;
      if (frame == null)
        return;
      ErrorHandler.ThrowOnFailure(frame.Hide());
    }

    public override IWin32Window Window
    {
      get
      {
        return (IWin32Window) this.elementHost;
      }
    }

    internal void ShowDiff(
      string leftTitle,
      string left,
      string rightTitle,
      string right,
      string label)
    {
      this.Caption = string.Format("{0} - {1}", (object) Resources.StateComparisonWindowTitle, (object) label);
      this.control.ShowDiff(leftTitle, left, rightTitle, right);
    }
  }
}
