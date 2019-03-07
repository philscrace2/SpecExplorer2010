// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ViewDocument
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.Viewer;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Xrt.UI;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.SpecExplorer.VS
{
  internal class ViewDocument : WindowPane, IVsPersistDocData, IPersistFileFormat, Microsoft.VisualStudio.OLE.Interop.IPersist, IVsFileChangeEvents
  {
    private const uint cmdidFullScreen = 233;
    private SpecExplorerPackage package;
    private ViewDocumentControl viewDocumentControl;
    private string fileName;
    private TransitionSystem transitionSystem;
    private IVsFileChangeEx vsFileChangeEx;
    private uint vsFileChangeCookie;

    public ViewDocument(
      SpecExplorerPackage package,
      Microsoft.VisualStudio.OLE.Interop.IServiceProvider oleServiceProvider,
      string fileName)
      : base((System.IServiceProvider) new ServiceProvider(oleServiceProvider))
    {
      this.package = package;
      this.fileName = fileName;
      this.viewDocumentControl = new ViewDocumentControl((IViewDefinitionManager) this.package.CoreServices.GetRequiredService<IViewDefinitionManager>(), (IHost) this.package);
      this.AddEventHandlers();
    }

    private void AddEventHandlers()
    {
      this.viewDocumentControl.add_BrowseStates(new EventHandler<StatesBrowserEventArgs>(this.OnBrowseStates));
      this.viewDocumentControl.add_BrowseStep(new EventHandler<StepBrowserEventArgs>(this.OnBrowseStep));
      this.viewDocumentControl.add_CompareStates(new EventHandler<CompareStateEventArgs>(this.OnCompareStates));
      this.viewDocumentControl.add_InvokeViewDefinitionManager(new EventHandler(this.OnInvokeViewDefinitionManager));
      ViewDocumentControl viewDocumentControl = this.viewDocumentControl;
      viewDocumentControl.FullScreen = (__Null) Delegate.Combine((Delegate) viewDocumentControl.FullScreen, (Delegate) ((sender, args) =>
      {
        Guid standardCommandSet97 = VSConstants.GUID_VSStandardCommandSet97;
        object pvaIn = (object) null;
        ErrorHandler.ThrowOnFailure(((IVsUIShell) this.GetService(typeof (SVsUIShell))).PostExecCommand(ref standardCommandSet97, 233U, 233U, ref pvaIn));
      }));
    }

    private void OnInvokeViewDefinitionManager(object sender, EventArgs e)
    {
      this.package.ShowViewDefinitionDialog(sender, e);
    }

    private void OnBrowseStep(object sender, StepBrowserEventArgs e)
    {
      StepBrowserToolWindow toolWindow = this.package.FindToolWindow(typeof (StepBrowserToolWindow), 0, true) as StepBrowserToolWindow;
      toolWindow.LoadSteps(e.BrowserEdges, e.StepLabel);
      IVsWindowFrame frame = toolWindow.Frame as IVsWindowFrame;
      if (frame == null)
        return;
      this.package.AssertOk(frame.Show());
    }

    private void OnBrowseStates(object sender, StatesBrowserEventArgs e)
    {
      StateBrowserToolWindow toolWindow = this.package.FindToolWindow(typeof (StateBrowserToolWindow), 0, true) as StateBrowserToolWindow;
      toolWindow.SetHost((IHost) this.package);
      toolWindow.LoadStates(this.fileName, e.States, e.ShouldDisplayLeftTree, e.StateLabel);
      IVsWindowFrame frame = toolWindow.Frame as IVsWindowFrame;
      if (frame == null)
        return;
      this.package.AssertOk(frame.Show());
    }

    private void OnCompareStates(object sender, CompareStateEventArgs e)
    {
      ExplorationResultLoader explorationResultLoader = new ExplorationResultLoader(this.fileName);
      try
      {
        StateEntity state1 = explorationResultLoader.LoadState(e.Left.Label);
        StateEntity state2 = explorationResultLoader.LoadState(e.Right.Label);
        SharedEntitySet sharedEntities = explorationResultLoader.LoadSharedEntities();
        StateComparisonView toolWindow = this.package.FindToolWindow(typeof (StateComparisonView), 0, true) as StateComparisonView;
        toolWindow.Show();
        toolWindow.ShowDiff(e.Left(.Label, this.BuildStateString(sharedEntities, state1), e.Right.Label, this.BuildStateString(sharedEntities, state2), e.CompareLabel);
      }
      catch (ExplorationResultLoadingException ex)
      {
        this.package.NotificationDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, string.Format("Failed to load file {0}:\n{1}", (object) this.fileName, (object) ex.Message));
      }
    }

    private string BuildStateString(SharedEntitySet sharedEntities, StateEntity state)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("Kind : {0}\r\n", (object) state.Flags);
      stringBuilder.AppendFormat("Description : {0}\r\n", (object) state.Description);
      if (state.Content != null)
      {
        StateBrowserUtils stateBrowserUtils = new StateBrowserUtils();
        stringBuilder.Append(stateBrowserUtils.StateNodeToString(sharedEntities, state.Content));
      }
      return stringBuilder.ToString();
    }

    public override IWin32Window Window
    {
      get
      {
        return (IWin32Window) this.viewDocumentControl;
      }
    }

    int IVsPersistDocData.Close()
    {
      return 0;
    }

    int IVsPersistDocData.GetGuidEditorType(out Guid pClassID)
    {
      pClassID = GuidList.guidViewDocumentFactory;
      return 0;
    }

    int IVsPersistDocData.IsDocDataDirty(out int pfDirty)
    {
      pfDirty = 0;
      return 0;
    }

    int IVsPersistDocData.IsDocDataReloadable(out int pfReloadable)
    {
      pfReloadable = 1;
      return 0;
    }

    int IVsPersistDocData.LoadDocData(string pszMkDocument)
    {
      try
      {
        ExplorationResultLoader explorationResultLoader = new ExplorationResultLoader(pszMkDocument);
        TransitionSystem transitionSystem;
        this.viewDocumentControl.set_TransitionSystem(transitionSystem = explorationResultLoader.LoadTransitionSystem());
        this.transitionSystem = transitionSystem;
        int num1;
        int num2;
        int num3;
        int num4;
        int num5;
        int num6;
        int num7;
        this.viewDocumentControl.GetTransitionSystemStatus(ref num1, ref num2, ref num3, ref num4, ref num5, ref num6, ref num7);
        this.package.ProgressMessage((VerbosityLevel) 0, string.Format("Loaded exploration result '{0}' from '{1}'.", (object) this.transitionSystem.Name, (object) this.fileName));
        this.package.ProgressMessage((VerbosityLevel) 0, string.Format("'{0}' includes {1} states, {2} steps, {3} requirements, {4} errors, {5} non-accepting end states, {6} bounds hit.", (object) this.transitionSystem.Name, (object) num2, (object) num6, (object) num7, (object) num3, (object) num4, (object) num5));
        this.fileName = pszMkDocument;
        this.SetFileChangeNotification(this.fileName, true);
        this.UpdateHelpContext();
        return 0;
      }
      catch (ExplorationResultLoadingException ex)
      {
        this.package.NotificationDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, string.Format("Failed to load file {0}:\n{1}", (object) pszMkDocument, (object) ex.Message));
        return -2147467259;
      }
    }

    int IVsPersistDocData.OnRegisterDocData(
      uint docCookie,
      IVsHierarchy pHierNew,
      uint itemidNew)
    {
      return 0;
    }

    int IVsPersistDocData.ReloadDocData(uint grfFlags)
    {
      return ((IVsPersistDocData) this).LoadDocData(this.fileName);
    }

    int IVsPersistDocData.RenameDocData(
      uint grfAttribs,
      IVsHierarchy pHierNew,
      uint itemidNew,
      string pszMkDocumentNew)
    {
      this.SetFileChangeNotification(this.fileName, false);
      this.fileName = pszMkDocumentNew;
      this.SetFileChangeNotification(this.fileName, true);
      return 0;
    }

    int IVsPersistDocData.SaveDocData(
      VSSAVEFLAGS dwSave,
      out string pbstrMkDocumentNew,
      out int pfSaveCanceled)
    {
      pbstrMkDocumentNew = (string) null;
      pfSaveCanceled = 0;
      return 0;
    }

    int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath)
    {
      return 0;
    }

    int IPersistFileFormat.GetClassID(out Guid pClassID)
    {
      ErrorHandler.ThrowOnFailure(((Microsoft.VisualStudio.OLE.Interop.IPersist) this).GetClassID(out pClassID));
      return 0;
    }

    int IPersistFileFormat.GetCurFile(
      out string ppszFilename,
      out uint pnFormatIndex)
    {
      pnFormatIndex = 0U;
      ppszFilename = this.fileName;
      return 0;
    }

    int IPersistFileFormat.GetFormatList(out string ppszFormatList)
    {
      ppszFormatList = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Spec Explorer Exploration Result (*{0}){1}*{0}{1}{1}", (object) ".seexpl", (object) '\n');
      return 0;
    }

    int IPersistFileFormat.InitNew(uint nFormatIndex)
    {
      return 0;
    }

    int IPersistFileFormat.IsDirty(out int pfIsDirty)
    {
      return ((IVsPersistDocData) this).IsDocDataDirty(out pfIsDirty);
    }

    int IPersistFileFormat.Load(string pszFilename, uint grfMode, int fReadOnly)
    {
      return ((IVsPersistDocData) this).LoadDocData(pszFilename);
    }

    int IPersistFileFormat.Save(
      string pszFilename,
      int fRemember,
      uint nFormatIndex)
    {
      if (pszFilename == this.fileName)
        return 0;
      Action<Exception> action = (Action<Exception>) (ex => this.package.NotificationDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, string.Format("Failed to save file {0}:\n{1}", (object) pszFilename, (object) ex.Message)));
      try
      {
        File.Copy(this.fileName, pszFilename, true);
      }
      catch (UnauthorizedAccessException ex)
      {
        action((Exception) ex);
      }
      catch (PathTooLongException ex)
      {
        action((Exception) ex);
      }
      catch (DirectoryNotFoundException ex)
      {
        action((Exception) ex);
      }
      catch (FileNotFoundException ex)
      {
        action((Exception) ex);
      }
      catch (IOException ex)
      {
        action((Exception) ex);
      }
      catch (NotSupportedException ex)
      {
        action((Exception) ex);
      }
      return 0;
    }

    int IPersistFileFormat.SaveCompleted(string pszFilename)
    {
      return 0;
    }

    internal void UpdateHelpContext()
    {
      IVsUIHierarchy hierarchy;
      uint itemID;
      IVsWindowFrame windowFrame;
      if (!VsShellUtilities.IsDocumentOpen((System.IServiceProvider) this.package, this.fileName, Guid.Empty, out hierarchy, out itemID, out windowFrame))
        return;
      object pvar;
      ErrorHandler.ThrowOnFailure(windowFrame.GetProperty(-3010, out pvar));
      ErrorHandler.ThrowOnFailure(((IVsUserContext) pvar).AddAttribute(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1, "keyword", "microsoft.specexplorer.explorationgraphviewer"));
    }

    private int SetFileChangeNotification(string fileName, bool startTracking)
    {
      int num = -2147467259;
      if (this.vsFileChangeEx == null)
        this.vsFileChangeEx = (IVsFileChangeEx) this.GetService(typeof (SVsFileChangeEx));
      if (this.vsFileChangeEx == null)
        return -2147418113;
      if (startTracking)
      {
        if (this.vsFileChangeCookie == 0U)
        {
          num = this.vsFileChangeEx.AdviseFileChange(fileName, 7U, (IVsFileChangeEvents) this, out this.vsFileChangeCookie);
          if (this.vsFileChangeCookie == 0U)
            return -2147467259;
        }
      }
      else if (this.vsFileChangeCookie != 0U)
      {
        num = this.vsFileChangeEx.UnadviseFileChange(this.vsFileChangeCookie);
        this.vsFileChangeCookie = 0U;
      }
      return num;
    }

    int IVsFileChangeEvents.DirectoryChanged(string pszDirectory)
    {
      return 0;
    }

    int IVsFileChangeEvents.FilesChanged(
      uint cChanges,
      string[] rgpszFile,
      uint[] rggrfChange)
    {
      if (cChanges == 0U || rgpszFile == null || rggrfChange == null)
        return -2147024809;
      for (uint index = 0; index < cChanges; ++index)
      {
        if (!string.IsNullOrEmpty(rgpszFile[(IntPtr) index]) && string.Compare(rgpszFile[(IntPtr) index], this.fileName, true) == 0 && (((int) rggrfChange[(IntPtr) index] & 6) != 0 && this.IsFileReadyToRead(rgpszFile[(IntPtr) index])))
          ErrorHandler.ThrowOnFailure(((IVsPersistDocData) this).ReloadDocData(0U));
      }
      return 0;
    }

    private bool IsFileReadyToRead(string filePath)
    {
      try
      {
        using (File.Open(filePath, FileMode.Open, FileAccess.Read))
          ;
      }
      catch (IOException ex)
      {
        return false;
      }
      return true;
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        this.SetFileChangeNotification(this.fileName, false);
        if (this.viewDocumentControl == null)
          return;
        ((Component) this.viewDocumentControl).Dispose();
        this.viewDocumentControl = (ViewDocumentControl) null;
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    int Microsoft.VisualStudio.OLE.Interop.IPersist.GetClassID(out Guid pClassID)
    {
      ErrorHandler.ThrowOnFailure(((Microsoft.VisualStudio.OLE.Interop.IPersist) this).GetClassID(out pClassID));
      return 0;
    }
  }
}
