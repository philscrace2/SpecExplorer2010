// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.StepBrowserToolWindow
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
  [Guid("7E4F0150-06DA-4084-8F5C-A3A76A70E7D7")]
  public class StepBrowserToolWindow : ToolWindowPane
  {
    private StepBrowserControl control;

    public StepBrowserToolWindow()
      : base((System.IServiceProvider) null)
    {
      this.Caption = Microsoft.SpecExplorer.Resources.StepBrowserToolWindow;
      this.control = new StepBrowserControl();
    }

    protected override void Initialize()
    {
      base.Initialize();
    }

    public void LoadSteps(IEnumerable<BrowserEdge> browserEdges, string label)
    {
      this.Caption = string.Format("{0} - {1}", (object) Microsoft.SpecExplorer.Resources.StepBrowserToolWindow, (object) label);
      this.control.LoadSteps(browserEdges);
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
