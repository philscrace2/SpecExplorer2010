// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.WorkflowToolWindow
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.SpecExplorer.ModelingGuidance;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Microsoft.SpecExplorer.VS
{
  [Guid("5AE58719-7142-4f61-A4C7-588FCF0B3C74")]
  public class WorkflowToolWindow : ToolWindowPane
  {
    public const string ActivityCompletionStatusKey = "SpecExplorer.ActivityCompletionStatus";
    public const string LastUsedGuidanceKey = "SpecExplorerLastUsedGuidance";
    public const string WasWorkflowLoadedKey = "SpecExplorerWasWorkflowLoaded";
    private ElementHost guidanceViewerHost;
    private SpecExplorerPackage sePackage;

    public WorkflowToolWindow()
      : base((System.IServiceProvider) null)
    {
      this.Caption = Microsoft.SpecExplorer.Resources.WorkflowToolWindowTitle;
      this.BitmapResourceID = 602;
      this.BitmapIndex = 2;
      this.guidanceViewerHost = new ElementHost();
      this.guidanceViewerHost.Dock = DockStyle.Fill;
      this.guidanceViewerHost.Child = (UIElement) null;
    }

    public override IWin32Window Window
    {
      get
      {
        return (IWin32Window) this.guidanceViewerHost;
      }
    }

    public override void OnToolWindowCreated()
    {
      base.OnToolWindowCreated();
      this.sePackage = this.Package as SpecExplorerPackage;
      this.sePackage.SessionInitialized += new EventHandler(this.OnSessionInitialized);
      this.sePackage.SessionDisposed += new EventHandler(this.OnSessionDisposed);
      this.SetUserContext();
    }

    private void SetUserContext()
    {
      object pvar;
      ErrorHandler.ThrowOnFailure(((IVsWindowFrame) this.Frame).GetProperty(-3010, out pvar));
      ErrorHandler.ThrowOnFailure(((IVsUserContext) pvar).AddAttribute(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1, "keyword", "microsoft.specexplorer.modelingguidance"));
    }

    private void OnSessionInitialized(object sender, EventArgs e)
    {
    }

    private void OnSessionDisposed(object sender, EventArgs e)
    {
    }

    internal void LoadWindowContent()
    {
      this.LoadGuidanceFiles();
      this.guidanceViewerHost.Child = (UIElement) new GuidanceUserControl();
      this.LoadGuidanceControl();
    }

    internal void UnloadWindowContent()
    {
      this.GuidanceLoader.UnloadGuidanceList();
      this.guidanceViewerHost.Child = (UIElement) null;
    }

    internal bool IsWindowContentLoaded
    {
      get
      {
        return this.guidanceViewerHost.Child != null;
      }
    }

    internal bool IsWindowVisibleAndLoaded
    {
      get
      {
        if (this.IsWindowVisible)
          return this.IsWindowContentLoaded;
        return false;
      }
    }

    internal bool IsWindowVisible
    {
      get
      {
        return (this.Frame as IVsWindowFrame).IsVisible() == 0;
      }
    }

    internal string CurrentGuidance
    {
      get
      {
        return this.UserControl.get_ControlModel().get_SelectedGuidance().get_Id();
      }
    }

    internal string GuidanceActivityCompletionStatus
    {
      get
      {
        if (this.GuidanceLoader == null)
          return string.Empty;
        return GuidanceUsageInfo.PackMultipleGuidanceUsageToString(this.GuidanceLoader.get_LoadedGuidanceList());
      }
    }

    private void AssistedProcedureRequestCallBack(
      object sender,
      AssistedProcedureRequestEventArgs e)
    {
      this.sePackage.ExecuteSEVSCommand(e.get_ProcedureId());
    }

    private void LoadGuidanceControl()
    {
      IEnumerable<IGuidance> loadedGuidanceList = ((IGuidanceLoader) this.sePackage.CoreServices.GetRequiredService<IGuidanceLoader>()).get_LoadedGuidanceList();
      using (IEnumerator<IGuidance> enumerator = loadedGuidanceList.GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
          this.UserControl.get_ControlModel().get_GuidanceList().Add(enumerator.Current);
      }
      IGuidance iguidance = this.sePackage.LastUsedGuidance != null ? loadedGuidanceList.FirstOrDefault<IGuidance>((Func<IGuidance, bool>) (gd => gd.get_Id() == this.sePackage.LastUsedGuidance)) : (IGuidance) null;
      if (iguidance != null)
        this.UserControl.get_ControlModel().set_SelectedGuidance(iguidance);
      this.UserControl.get_ControlModel().add_AssistedProcedureRequested(new EventHandler<AssistedProcedureRequestEventArgs>(this.AssistedProcedureRequestCallBack));
    }

    private void LoadGuidanceFiles()
    {
      try
      {
        foreach (string file in Directory.GetFiles(Path.Combine(this.sePackage.Session.get_InstallDir(), Microsoft.SpecExplorer.Resources.WorkflowsDirectory), Microsoft.SpecExplorer.Resources.WorkflowFileExtension, SearchOption.AllDirectories))
        {
          try
          {
            using (FileStream fileStream = File.OpenRead(file))
            {
              try
              {
                this.GuidanceLoader.LoadGuidance((Stream) fileStream);
              }
              catch (GuidanceException ex)
              {
              }
            }
          }
          catch (UnauthorizedAccessException ex)
          {
          }
        }
      }
      catch (UnauthorizedAccessException ex)
      {
      }
      catch (DirectoryNotFoundException ex)
      {
      }
      catch (IOException ex)
      {
      }
      try
      {
        this.GuidanceLoader.LoadGuidanceUsage((this.Package as SpecExplorerPackage).ActivityCompletionStatus);
      }
      catch (GuidanceException ex)
      {
      }
    }

    private GuidanceUserControl UserControl
    {
      get
      {
        return this.guidanceViewerHost.Child as GuidanceUserControl;
      }
    }

    private IGuidanceLoader GuidanceLoader
    {
      get
      {
        if (this.sePackage.CoreServices == null)
          return (IGuidanceLoader) null;
        return (IGuidanceLoader) this.sePackage.CoreServices.GetRequiredService<IGuidanceLoader>();
      }
    }
  }
}
