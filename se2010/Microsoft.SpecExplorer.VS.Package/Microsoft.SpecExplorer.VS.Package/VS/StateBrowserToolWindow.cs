// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.StateBrowserToolWindow
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.SpecExplorer.Viewer;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.SpecExplorer.VS
{
  [Guid("1079EAE0-5880-4dc0-88FF-139EDF582BCA")]
  public class StateBrowserToolWindow : ToolWindowPane
  {
    private ExplorationStateView control;

    public StateBrowserToolWindow()
      : base((System.IServiceProvider) null)
    {
      this.control = new ExplorationStateView();
    }

    protected override void Initialize()
    {
      base.Initialize();
    }

    public void SetHost(IHost host)
    {
       this.control.Host = host;
    }

    public void LoadStates(
      string fileName,
      IEnumerable<Microsoft.SpecExplorer.ObjectModel.State> states,
      bool shouldDisplayLeftTree,
      string label)
    {
      this.Caption = string.Format("{0} - {1}", (object) Microsoft.SpecExplorer.Resources.StatesBrowserToolWindowTitle, (object) label);
      ((Control) this.control).SuspendLayout();
      this.control.LoadStates(fileName, states, shouldDisplayLeftTree);
      ((Control) this.control).ResumeLayout();
    }

    public override IWin32Window Window
    {
      get
      {
        return (IWin32Window) this.control;
      }
    }
  }
}
