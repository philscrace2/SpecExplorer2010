// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.SpecExplorerPackage
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using EnvDTE80;
using Microsoft.ActionMachines.Cord;
using Microsoft.Modeling;
using Microsoft.SpecExplorer.ErrorReporting;
using Microsoft.SpecExplorer.Runtime.Testing;
using Microsoft.SpecExplorer.Viewer;
using Microsoft.SpecExplorer.VS.Common;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.VSHelp;
using Microsoft.VisualStudio.VSHelp80;
using Microsoft.Xrt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using VSLangProj80;
using Microsoft.VisualStudio;


namespace Microsoft.SpecExplorer.VS
{
  [ProvideToolWindow(typeof (StateComparisonView), Height = 480, MultiInstances = false, Orientation = ToolWindowOrientation.Right, Style = VsDockStyle.Tabbed, Transient = true, Width = 640, Window = "{4a9b7e51-aa16-11d0-a8c5-00a0c921a4d2}")]
  [ProvideKeyBindingTable("04C7681D-A337-4705-8AD9-2206D31A9F7B", 508)]
  [ProvideEditorFactory(typeof (EditorFactory), 503, TrustLevel = __VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
  [ProvideToolWindow(typeof (StepBrowserToolWindow), Height = 480, MultiInstances = false, Orientation = ToolWindowOrientation.Right, Style = VsDockStyle.Tabbed, Transient = true, Width = 640, Window = "{4a9b7e51-aa16-11d0-a8c5-00a0c921a4d2}")]
  [ProvideToolWindow(typeof (WorkflowToolWindow), Orientation = ToolWindowOrientation.none, Style = VsDockStyle.Tabbed, Width = 250, Window = "{B1E99781-AB81-11D0-B683-00AA00A3EE26}")]
  [ProvideToolWindow(typeof (ExplorationManagerToolWindow), Style = VsDockStyle.Tabbed, Window = "{3AE79031-E1BC-11D0-8F78-00A0C9110057}")]
  [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\10.0")]
  //[InstalledProductRegistration(false, "Spec Explorer for Visual Studio 2010 (version 3.5.3146.0)", "Spec Explorer Modeling and Testing Environment, (c) 2009 Microsoft Corporation.", "3.5.3146.0", IconResourceID = 600)]
  [ProvideLoadKey("Standard", "2.0", "Spec Explorer for VS", "Microsoft", 400)]
  [ProvideMenuResource(1000, 32)]
  [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")]
  [ProvideToolWindow(typeof (StateBrowserToolWindow), Height = 480, MultiInstances = false, Orientation = ToolWindowOrientation.Right, Style = VsDockStyle.Tabbed, Transient = true, Width = 640, Window = "{4a9b7e51-aa16-11d0-a8c5-00a0c921a4d2}")]
  [ProvideEditorExtension(typeof (EditorFactory), ".cord", 32)]
  [ProvideEditorLogicalView(typeof (EditorFactory), "{7651A703-06E5-11D1-8EBD-00A0C90F26EA}", IsTrusted = true)]
  [ProvideEditorFactory(typeof (ViewDocumentFactory), 509, TrustLevel = __VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
  [ProvideEditorExtension(typeof (ViewDocumentFactory), ".seexpl", 32)]
  [ProvideEditorLogicalView(typeof (ViewDocumentFactory), "{00000000-0000-0000-0000-000000000000}", IsTrusted = true)]
  [ProvideEditorFactory(typeof (SummaryDocumentFactory), 511, TrustLevel = __VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
  [ProvideEditorExtension(typeof (SummaryDocumentFactory), ".sesum", 32)]
  [ProvideEditorLogicalView(typeof (SummaryDocumentFactory), "{00000000-0000-0000-0000-000000000000}", IsTrusted = true)]
  [ProvideService(typeof (SGlobalService))]  
  [Guid("f9b9b97b-5213-4c39-b0df-9b44a2b97c58")]
  [ProvideSolutionProps("SpecExplorer.ActivityCompletionStatus")]
  public sealed class SpecExplorerPackage : Package, IHost, IDisposable, IVsSolutionEvents, IVsTrackProjectDocumentsEvents2, IVsUpdateSolutionEvents2, IVsPersistSolutionProps, IVsPersistSolutionOpts
  {
    private bool loggingEnabled = true;
    private VerbosityLevel verbosity = (VerbosityLevel) 2;
    private TaskCategory currentTaskCategory = TaskCategory.BuildCompile;
    private Dictionary<Tuple<string, string>, string> projectRenameQueries = new Dictionary<Tuple<string, string>, string>();
    private string lastUsedGuidance = string.Empty;
    internal EditorFactory editorFactory;
    private ViewDocumentFactory viewFactory;
    private ISession session;
    private IExtensionManager extensionManager;
    private int errorsSuppressed;
    private DTE dte;
    private IVsUIShell uiShell;
    private IVsTextManager textManager;
    private IVsMonitorSelection monitorSelection;
    private IVsStatusbar statusBar;
    private IVsSolutionBuildManager2 buildManager;
    private bool disposed;
    private ErrorListProvider errorList;
    private OutputWindowPane specExplorerPane;
    private OutputWindowPane debugPane;
    private CommandWindow commandWindow;
    private bool wasWorkflowLoaded;

    private void RegisterAddAction()
    {
      OleMenuCommandService service = this.GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
      if (service == null)
        return;
      MenuCommand command = new MenuCommand(new EventHandler(this.AddActionCallBack), new CommandID(GuidList.guidSpecExplorerCmdSet, 4386));
      service.AddCommand(command);
    }

    private void AddActionCallBack(object sender, EventArgs evtArgs)
    {
      new AddActionAssistedProcedure(this).Invoke();
    }

    private void RegisterDeclareRule()
    {
      OleMenuCommandService service = this.GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
      if (service == null)
        return;
      MenuCommand command = new MenuCommand(new EventHandler(this.DeclareRuleCallBack), new CommandID(GuidList.guidSpecExplorerCmdSet, 4387));
      service.AddCommand(command);
    }

    private void DeclareRuleCallBack(object sender, EventArgs evtArgs)
    {
      new DeclareRuleAssistedProcedure(this).Invoke();
    }

    internal string CreateScriptAndAddToProject(string projectUniqueName, string scriptName)
    {
      Project projectByUniqueName = this.GetProjectByUniqueName(projectUniqueName);
      if (projectByUniqueName == null)
      {
        this.NotificationDialog("Failed Adding New Script", string.Format("Unable to locate Container Project : {0}", (object) projectUniqueName));
        return (string) null;
      }
      string str = Path.Combine(Path.GetDirectoryName(projectByUniqueName.FullName), FileNames.HasScriptExtension(scriptName) ? scriptName : scriptName + ".cord");
      if (File.Exists(str))
      {
        this.NotificationDialog("Failed Adding New Script", string.Format("{0}\nUnable to create script : {1}", (object) "Script already Exists", (object) str));
        return (string) null;
      }
      try
      {
        File.CreateText(str);
        projectByUniqueName.ProjectItems.AddFromFile(str);
        return str;
      }
      catch (DirectoryNotFoundException ex)
      {
        this.NotificationDialog("Failed Adding New Script", string.Format("{0}\nUnable to create script : {1}", (object) ex.Message, (object) str));
        return (string) null;
      }
      catch (UnauthorizedAccessException ex)
      {
        this.NotificationDialog("Failed Adding New Script", string.Format("{0}\nUnable to create script : {1}", (object) ex.Message, (object) str));
        return (string) null;
      }
      catch (ArgumentException ex)
      {
        this.NotificationDialog("Failed Adding New Script", string.Format("{0}\nUnable to create script : {1}", (object) ex.Message, (object) str));
        return (string) null;
      }
    }

    private void RegisterHelps()
    {
      OleMenuCommandService service = this.GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
      if (service == null)
        return;
      OleMenuCommand oleMenuCommand1 = new OleMenuCommand(new EventHandler(this.OpenSpecExplorerHomePage), new CommandID(GuidList.guidSpecExplorerCmdSet, 513));
      service.AddCommand((MenuCommand) oleMenuCommand1);
      OleMenuCommand oleMenuCommand2 = new OleMenuCommand(new EventHandler(this.OpenSpecExplorerDocumentation), new CommandID(GuidList.guidSpecExplorerCmdSet, 514));
      service.AddCommand((MenuCommand) oleMenuCommand2);
      OleMenuCommand oleMenuCommand3 = new OleMenuCommand(new EventHandler(this.OpenSpecExplorerForum), new CommandID(GuidList.guidSpecExplorerCmdSet, 516));
      service.AddCommand((MenuCommand) oleMenuCommand3);
    }

    private void OpenSpecExplorerHomePage(object sender, EventArgs e)
    {
        string s = Microsoft.SpecExplorer.Resources.ExplorationManagerToolWindowTitle;
        System.Diagnostics.Process.Start("http://go.microsoft.com/fwlink/?LinkID=166911");
    }

    private void OpenSpecExplorerDocumentation(object sender, EventArgs e)
    {
      System.Diagnostics.Process.Start("ms-xhelp:///?method=page&id=05db001d-3ea1-48c8-b031-7fb75b6eeaae&product=vs&productversion=100&locale=en-us");
    }

    private void OpenSpecExplorerForum(object sender, EventArgs e)
    {
      System.Diagnostics.Process.Start("http://social.msdn.microsoft.com/Forums/en-US/specexplorer/threads");
    }

    private void CheckAndConvertProject(Project project)
    {
      if (project == null || project.ProjectItems == null)
        return;
      bool flag = false;
      foreach (ProjectItem projectItem in project.ProjectItems)
      {
        if (this.CheckAndConvertProjectItem(projectItem))
          flag = true;
      }
      if (!flag)
        return;
      this.NotificationDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, string.Format("Project {0} was created by previous version of {1}. \r\nThe project file has been converted. Please save it and reopen your solution. \r\nFor more information, please check release notes", (object) project, (object) Microsoft.SpecExplorer.Resources.SpecExplorer));
    }

    private bool CheckAndConvertProjectItem(ProjectItem projectItem)
    {
      string str = "SubType";
      bool flag = false;
      if (projectItem != null)
      {
        Guid guid = new Guid(projectItem.Kind);
        if (guid == VSConstants.GUID_ItemType_PhysicalFile)
        {
          if (FileNames.HasScriptExtension(projectItem.Name) && projectItem.Properties != null)
          {
            Property property = projectItem.Properties.Item((object) str);
            if (property != null && property != null && !string.IsNullOrEmpty(property.ToString()))
            {
              flag = true;
            }
          }
        }
        else if (guid == VSConstants.GUID_ItemType_PhysicalFolder && projectItem.ProjectItems != null)
        {
          foreach (ProjectItem projectItem1 in projectItem.ProjectItems)
          {
            if (this.CheckAndConvertProjectItem(projectItem1))
              flag = true;
          }
        }
      }
      return flag;
    }

    internal ICordDesignTimeScopeManager CordScopeManager
    {
      get
      {
        return (ICordDesignTimeScopeManager) this.CoreServices.GetRequiredService<ICordDesignTimeScopeManager>();
      }
    }

    private void InitializeScriptDesignTime()
    {
      this.CordScopeManager.BeforeParseScript += new EventHandler<ScriptParseEventArgs>(this.OnBeforeParseScript);
      this.CordScopeManager.ScopeChanged += new EventHandler<ScopeChangeEventArgs>(this.OnScopeChanged);
    }

    private void OnBeforeParseScript(object sender, ScriptParseEventArgs e)
    {
      this.ClearParsingErrorList();
    }

    private bool FilterProjectKind(Project project)
    {
      if (project.Kind != "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}" && project.Kind != "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}")
        return project.Kind != "{67294A52-A4F0-11D2-AA88-00C04F688DDE}";
      return false;
    }

    internal ICordDesignTimeManager GetDesignTimeForCordDocument(
      CordDocument doc)
    {
      Project dteProject = this.ToDteProject(doc.Hierarchy);
      if (dteProject != null)
        return this.GetDesignTimeForProject(dteProject);
      return (ICordDesignTimeManager) null;
    }

    internal ICordDesignTimeManager GetDesignTimeForProject(Project project)
    {
      return ((ICordDesignTimeScopeManager) this.CoreServices.GetRequiredService<ICordDesignTimeScopeManager>()).GetCordDesignTimeManager(project.UniqueName);
    }

    private void RegisterProjectToCordScopeManager(Project project)
    {
      if (!this.FilterProjectKind(project))
        return;
      bool flag = !this.CordScopeManager.AllScopes.Contains(project.UniqueName);
      ICordDesignTimeManager cdt = this.CordScopeManager.RegisterCordDesignTimeManager(project.UniqueName);
      if (flag)
      {
        cdt.ReportError += new EventHandler<ErrorReport>(this.OnReportError);        
        cdt.ScriptChanged += new EventHandler<ScriptChangeEventArgs>(this.OnScriptChanged);
      }
      foreach (string scriptPath in (IEnumerable<string>) ProjectUtils.GetDocumentsInProject(project, ".cord"))
        this.RegisterScriptToCordDesignTimeManager(cdt, scriptPath);
    }

    private void OnScriptChanged(object sender, ScriptChangeEventArgs e)
    {
      this.FindToolWindow<ExplorationManagerToolWindow>().UpdateMachineList();
    }

    private void OnReportError(object sender, ErrorReport error)
    {
      DiagnosisKind kind = (DiagnosisKind) 0;
      if (error.Kind.Equals(3))
        kind = (DiagnosisKind) 2;
      else if (error.Kind.Equals(2))
        kind = (DiagnosisKind) 1;
      this.DiagMessage(kind, (string) error.Description, (object) new TextLocation((string) error.FileName, (short) ((Location) error.Location).StartLine, (short) ((Location) error.Location).StartColumn), (bool) error.IsParsingError);
    }

    internal void RegisterCordDocumentToDesignTimeManager(
      CordDocument doc,
      bool getContentFromBuffer)
    {
      ICordDesignTimeManager timeForCordDocument = this.GetDesignTimeForCordDocument(doc);
      if (timeForCordDocument == null)
        return;
      if (getContentFromBuffer)
      {
        if (timeForCordDocument.ManagedScripts.Contains(doc.FileName))
          return;
        timeForCordDocument.RegisterScript(doc.FileName, (Func<string>) (() => doc.GetBufferContent()));
      }
      else
        this.RegisterScriptToCordDesignTimeManager(timeForCordDocument, doc.FileName);
    }

    internal void RegisterScriptToCordDesignTimeManager(
      ICordDesignTimeManager cdt,
      string scriptPath)
    {
      if (cdt == null)
        return;
      string content = this.GetScriptContent(scriptPath);
      if (content == null || cdt.ManagedScripts.Contains(scriptPath))
        return;
      cdt.RegisterScript(scriptPath, (Func<string>) (() => content));
    }

    private string GetScriptContent(string path)
    {
      try
      {
        return File.ReadAllText(path);
      }
      catch
      {
      }
      return (string) null;
    }

    private void RegisterScriptToCordDesignTimeManager(Project project, string path)
    {
      if (!FileNames.HasScriptExtension(path) || !this.FilterProjectKind(project))
        return;
      this.RegisterScriptToCordDesignTimeManager(this.CordScopeManager.GetCordDesignTimeManager(project.UniqueName), path);
    }

    private void OnScopeChanged(object sender, ScopeChangeEventArgs e)
    {
      this.FindToolWindow<ExplorationManagerToolWindow>().UpdateMachineList();
    }

    internal void UnregisterCordDocumentFromDesignTimeManager(CordDocument doc)
    {
      this.GetDesignTimeForCordDocument(doc).UnregisterScript(doc.FileName);
    }

    private void UnregisterScriptFromCordDesignTimeManager(string scope, string path)
    {
      this.CordScopeManager.GetCordDesignTimeManager(scope).UnregisterScript(path);
    }

    private void UnregisterProjectFromCordScopeManager(Project project)
    {
      ICordDesignTimeManager designTimeManager = this.CordScopeManager.GetCordDesignTimeManager(project.UniqueName);
      if (designTimeManager == null)
        return;
      designTimeManager.ReportError -= new EventHandler<ErrorReport>(this.OnReportError);
      designTimeManager.ScriptChanged -= new EventHandler<ScriptChangeEventArgs>(this.OnScriptChanged);
      this.CordScopeManager.UnregisterCordDesignTimeManager(project.UniqueName);
    }

    private void UnregisterAllProjectsFromCordScopeManager()
    {
      foreach (string str in this.CordScopeManager.AllScopes.ToArray<string>())
      {
        ICordDesignTimeManager designTimeManager = this.CordScopeManager.GetCordDesignTimeManager(str);
        if (designTimeManager != null)
        {
          designTimeManager.ReportError -= new EventHandler<ErrorReport>(this.OnReportError);
          designTimeManager.ScriptChanged -= new EventHandler<ScriptChangeEventArgs>(this.OnScriptChanged);
          this.CordScopeManager.UnregisterCordDesignTimeManager(str);
        }
      }
    }

    internal bool ValidateAllScripts()
    {
      bool flag1 = true;
      ICordDesignTimeScopeManager requiredService = (ICordDesignTimeScopeManager) this.CoreServices.GetRequiredService<ICordDesignTimeScopeManager>();
      IEnumerable<Project> containingCordScript = ProjectUtils.GetProjectsContainingCordScript(this.DTE, requiredService);
      if (containingCordScript.Count<Project>() > 0)
      {
        CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        CultureInfo cultureInfo = new CultureInfo("en-US");
        using (IEnumerator<string> enumerator = requiredService.AllScopes.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            string scope = enumerator.Current;
            Project project = containingCordScript.FirstOrDefault<Project>((Func<Project, bool>) (p => p.UniqueName == scope));
            if (project != null)
            {
              ICordDesignTimeManager designTimeManager = requiredService.GetCordDesignTimeManager(scope);
              if (designTimeManager != null)
              {
                if (designTimeManager.ManagedScripts.Count > 0)
                {
                  try
                  {
                    System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                    bool flag2 = designTimeManager.EnsureValidation((IEnumerable<string>) this.CollectReferences(project));
                    flag1 = flag1 && flag2;
                  }
                  catch (Exception ex)
                  {
                    this.DiagMessage((DiagnosisKind) 0, ex.Message, (object) null);
                    flag1 = false;
                  }
                  finally
                  {
                    System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
                  }
                }
              }
            }
          }
        }
      }
      return flag1;
    }

    private void RegisterExplorationManagerCommands()
    {
      OleMenuCommandService service = this.GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
      if (service == null)
        return;
      OleMenuCommand oleMenuCommand = new OleMenuCommand(new EventHandler(this.ShowExplorationManagerToolWindow), new CommandID(GuidList.guidSpecExplorerCmdSet, 262));
      service.AddCommand((MenuCommand) oleMenuCommand);
    }

    private void ShowExplorationManagerToolWindow(object sender, EventArgs e)
    {
        ErrorHandler.ThrowOnFailure(((IVsWindowFrame) this.FindToolWindow<ExplorationManagerToolWindow>().Frame).Show());            
    }

    public Exception FatalError(string message, params Exception[] exceptions)
    {
      message = this.LogFatalError(message, exceptions);
      throw new InvalidOperationException(message);
    }

    private string LogFatalError(string message, params Exception[] exceptions)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("===========");
      stringBuilder.AppendLine("FATAL ERROR: " + message);
      stringBuilder.AppendLine("stacktrace: ");
      stringBuilder.AppendLine(new StackTrace(1, true).ToString());
      foreach (Exception exception in exceptions)
        stringBuilder.AppendLine("involved exception: " + (object) exception);
      stringBuilder.AppendLine("===========");
      string str = stringBuilder.ToString();
      this.Log(str);
      new ErrorReportBuilder((IHost) this).GenerateErrorReport(str, ((IEnumerable<Exception>) exceptions).FirstOrDefault<Exception>());
      return "Spec Explorer VS integration encounters fatal error: " + message + ".\r\n\r\nPlease report content of Debug->Windows->Output.";
    }

    public void RunProtected(ProtectedAction action)
    {
      try
      {
        action.Invoke();
      }
      catch (Exception ex)
      {
        this.RecoverFromFatalError(ex);
      }
    }

    public void RecoverFromFatalError(Exception exception)
    {
      if (exception == null)
        return;
      string pszText;
      if (!(exception is InvalidOperationException) || string.IsNullOrEmpty(exception.Message) || !exception.Message.StartsWith("Spec Explorer VS integration"))
        pszText = this.LogFatalError(exception.Message, exception);
      else
        pszText = exception.Message;
      IVsUIShell service = (IVsUIShell) this.GetService(typeof (SVsUIShell));
      Guid empty = Guid.Empty;
      int pnResult;
      this.AssertOk(service.ShowMessageBox(0U, ref empty, "Spec Explorer", pszText, string.Empty, 0U, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_CRITICAL, 0, out pnResult));
    }

    public EventHandler Protect(EventHandler handler)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: method pointer
      //return (EventHandler) ((sender, args) => this.RunProtected(new ProtectedAction((object) new SpecExplorerPackage())
      //{
      //  CS\u0024\u003C\u003E8__localsf = this,
      //  sender = sender,
      //  args = args
      //}, __methodptr(\u003CProtect\u003Eb__d))));

        return handler;
    }

    public void Assert(bool condition, string message)
    {
      if (condition)
        return;
      this.FatalError(message);
    }

    public void Assert(bool condition)
    {
      if (condition)
        return;
      this.FatalError("assertion failed");
    }

    public void AssertOk(int hr)
    {
      if (ErrorHandler.Succeeded(hr))
        return;
      this.FatalError(string.Format("assertion on COM call failed (hr={0})", (object) hr));
    }

    public void Log(string line)
    {
      if (string.IsNullOrEmpty(line))
        return;
      string pszDescription = line.EndsWith("\n", StringComparison.CurrentCultureIgnoreCase) ? line : line + "\r\n";
      IVsActivityLog service = this.GetService(typeof (SVsActivityLog)) as IVsActivityLog;
      if (service != null)
        this.AssertOk(service.LogEntry(3U, "SpecExplorer", pszDescription));
      if (this.DebugPaneIsAvailable)
      {
        this.DebugPane.OutputString(string.Format("{0}", (object) pszDescription));
        this.DebugPane.ForceItemsToTaskList();
      }
      else
        Console.WriteLine("{0}", (object) line);
    }

    public bool Logging
    {
      get
      {
        return this.loggingEnabled;
      }
    }

    public void ProgressMessage(VerbosityLevel verbosity, string message)
    {
      if (verbosity > this.verbosity)
        return;
      this.SpecExplorerPane.OutputString(message + "\r\n");
      if (verbosity > 0)
        return;
      this.AssertOk(this.Statusbar.SetText(message));
    }

    public VerbosityLevel Verbosity
    {
      get
      {
        return this.verbosity;
      }
    }

    public void DiagMessage(DiagnosisKind kind, string message, object location)
    {
      this.DiagMessage(kind, message, location, false);
    }

    public void DiagMessage(
      DiagnosisKind kind,
      string message,
      object location,
      bool isParsingError)
    {
      if (this.errorsSuppressed > 0)
        return;
      string str = kind == null ? "error: " : (kind.Equals(1) ? "warning: " : "hint: ");
      int line = 0;
      int column = 0;
      string fileName;
      if (location is TextLocation)
      {
        TextLocation textLocation = (TextLocation) location;
        fileName = ((TextLocation) textLocation).FileName ?? Microsoft.SpecExplorer.Resources.SpecExplorer;
        line = (int) ((TextLocation) textLocation).FirstLine;
        column = (int) ((TextLocation) textLocation).FirstColumn;
      }
      else
        fileName = location == null ? Microsoft.SpecExplorer.Resources.SpecExplorer : location.ToString() ?? Microsoft.SpecExplorer.Resources.SpecExplorer;
      this.SpecExplorerPane.OutputString(str + message + "\r\n");
      this.ErrorList.Tasks.Add((Task) new SpecExplorerPackage.SpecExplorerError(this, this.currentTaskCategory, kind, fileName, isParsingError, line, column, message));
    }

    public void NotificationDialog(string title, string message)
    {
      this.NotificationDialog(title, message, OLEMSGICON.OLEMSGICON_INFO);
    }

    private void NotificationDialog(string title, string message, OLEMSGICON icon)
    {
      IVsUIShell service = (IVsUIShell) this.GetService(typeof (SVsUIShell));
      Guid empty = Guid.Empty;
      int pnResult;
      this.AssertOk(service.ShowMessageBox(0U, ref empty, title, message, string.Empty, 0U, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, icon, 0, out pnResult));
    }

    public MessageResult DecisionDialog(
      string title,
      string message,
      MessageButton messageButton)
    {
      IVsUIShell service = (IVsUIShell) this.GetService(typeof (SVsUIShell));
      Guid empty = Guid.Empty;
      int pnResult;
      this.AssertOk(service.ShowMessageBox(0U, ref empty, title, message, string.Empty, 0U, messageButton.ToOleMessageButton(), OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_NOICON, 0, out pnResult));
      return (MessageResult) pnResult;
    }

    public DialogResult ModalDialog(Form form)
    {
      throw new NotImplementedException("The method or operation is not implemented.");
    }

    object IHost.GetService(System.Type type)
    {
      return this.GetService(type);
    }

    public IWin32Window DialogOwner
    {
      get
      {
        IntPtr phwnd;
        if (this.UIShell.GetDialogOwnerHwnd(out phwnd) == 0)
          return (IWin32Window) new SpecExplorerPackage.Win32Window(phwnd);
        return (IWin32Window) null;
      }
    }

    public bool TryFindLocation(MemberInfo member, out TextLocation location)
    {
        location = new TextLocation();
        return false;
    }

        //public bool TryFindLocation(MemberInfo member, out TextLocation location)
        //{
        //  try
        //  {
        //    location = new TextLocation();
        //    Project containingProject = this.GetContainingProject(member);
        //    if (containingProject == null)
        //      return false;
        //    CodeModel codeModel = containingProject.CodeModel;
        //    if (codeModel == null)
        //      return false;
        //    CodeType codeType = codeModel.CodeTypeFromFullName(member.TypeName);
        //    if (member.Kind == null)
        //    {
        //      if (codeType == null)
        //        return false;
        //      location = this.MakeLocation(codeType as CodeElement);
        //      return true;
        //    }
        //    if (member.Kind.Equals(2))
        //    {
        //      foreach (CodeElement allMember in codeType.GetAllMembers())
        //      {
        //        if (allMember.Kind == vsCMElement.vsCMElementVariable && allMember == member)
        //        {
        //          location = this.MakeLocation(allMember);
        //          return true;
        //        }
        //      }
        //      return false;
        //    }
        //    if (member.Kind.Equals(1))
        //    {
        //      string[] array = member.ParameterTypes.ToArray<string>();
        //      foreach (CodeElement allMember in codeType.GetAllMembers())
        //      {
        //        if (allMember.Kind == vsCMElement.vsCMElementFunction)
        //        {
        //          CodeFunction codeFunction = allMember as CodeFunction;
        //          if (allMember == member && codeFunction != null)
        //          {
        //            int length = array.Length;
        //            int num = 0;
        //            if (length == codeFunction.Parameters.Count)
        //            {
        //              bool flag = false;
        //              foreach (CodeParameter parameter in codeFunction.Parameters)
        //              {
        //                string str = array[num++];
        //                if (parameter.Type.TypeKind != vsCMTypeRef.vsCMTypeRefArray && parameter.Type.AsFullName != str || parameter.Type.TypeKind == vsCMTypeRef.vsCMTypeRefArray && !str.Contains(parameter.Type.ElementType.AsFullName))
        //                {
        //                  flag = true;
        //                  break;
        //                }
        //              }
        //              if (!flag)
        //              {
        //                location = this.MakeLocation(allMember);
        //                return true;
        //              }
        //            }
        //          }
        //        }
        //      }
        //      return false;
        //    }
        //    if (member.Kind.Equals(3))
        //      return false;
        //    foreach (CodeElement allMember in codeType.GetAllMembers())
        //    {
        //      if (allMember.Kind == vsCMElement.vsCMElementProperty && allMember == member)
        //      {
        //        location = this.MakeLocation(allMember);
        //        return true;
        //      }
        //    }
        //    return false;
        //  }
        //  catch (Exception ex)
        //  {
        //    this.DiagMessage((DiagnosisKind) 1, string.Format("A Spec Explorer/Visual Studio integration error occurred: \r\n{0}.\r\n Please report this error to support team.", (object) ex.ToString()), (object) null);
        //    location = new TextLocation();
        //    return false;
        //  }
        //}

        public void NavigateTo(string fileName, int line, int column)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentException("File name cannot be null or empty.", fileName);
      if (line < 0)
        throw new ArgumentException("line number cannot be negative integer.", line.ToString());
      if (column < 0)
        throw new ArgumentException("column number cannot be negative integer.", column.ToString());
      Guid logicalView = VSConstants.LOGVIEWID_TextView;
      IVsUIHierarchy hierarchy;
      uint itemID;
      IVsWindowFrame windowFrame;
      IVsTextView view;
      VsShellUtilities.OpenDocument((System.IServiceProvider) this, fileName, logicalView, out hierarchy, out itemID, out windowFrame, out view);
      object pvar;
      ErrorHandler.ThrowOnFailure(windowFrame.GetProperty(-4004, out pvar));
      IVsTextBuffer buffer = pvar as IVsTextBuffer;
      this.Assert(buffer != null, "Unable to get document buffer");
      IVsTextManager mgr = this.GetService(typeof (VsTextManagerClass)) as IVsTextManager;
      new System.Threading.Thread((ThreadStart) (() =>
      {
        System.Threading.Thread.Sleep(200);
        this.AssertOk(mgr.NavigateToLineAndColumn(buffer, ref logicalView, line, column, line, column));
      })).Start();
    }

    private Project GetContainingProject(MemberInfo member)
    {
      Project containingProject = (Project) null;
      foreach (Project allRealProject in ProjectUtils.GetAllRealProjects(this.DTE))
      {
        if (allRealProject != null && this.IsContainingProject(allRealProject, member, out containingProject))
          return containingProject;
      }
      return (Project) null;
    }

    private bool IsContainingProject(
      Project project,
      MemberInfo member,
      out Project containingProject)
    {
      CodeModel codeModel = project.CodeModel;
      if (codeModel != null)
      {
        try
        {
          CodeType codeType = codeModel.CodeTypeFromFullName(member.TypeName);
          if (codeType != null)
          {
            if (codeType.ProjectItem != null)
            {
              if (codeType.ProjectItem.ContainingProject != null)
              {
                if (string.Equals(codeType.ProjectItem.ContainingProject.FileName, project.FileName, StringComparison.CurrentCultureIgnoreCase))
                {
                  containingProject = project;
                  return true;
                }
              }
            }
          }
        }
        catch (COMException ex)
        {
        }
      }
      containingProject = (Project) null;
      return false;
    }

    private TextLocation MakeLocation(CodeElement elem)
    {
      return new TextLocation(elem.ProjectItem.get_FileNames((short) 0), (short) elem.StartPoint.Line, (short) elem.StartPoint.DisplayColumn);
    }

    public bool TryGetExtensionData(string key, object inputValue, out object outputValue)
    {
      return this.extensionManager.TyrGetExtensionData(key, inputValue, out outputValue);
    }

    private void InitializeSession()
    {
      if (this.session == null)
      {
        Microsoft.SpecExplorer.Session session = new Microsoft.SpecExplorer.Session((IHost) this);
        session.Application.Setup.Add((IComponent) new CordCompletionProvider(this));
        this.session = (ISession) session;
        ((IServiceContainer) this).AddService(typeof (SGlobalService), (object) new GlobalService((ComponentBase) session), true);
        if (this.SessionInitialized != null)
          this.SessionInitialized((object) this, (EventArgs) null);
      }
      this.InitializeScriptDesignTime();
      this.InitializeViewDefinitionManager();
    }

    private void DisposeSession()
    {
      ((IServiceContainer) this).RemoveService(typeof (SGlobalService));
      (this.session as IDisposable).Dispose();
      this.session = (ISession) null;
      if (this.SessionDisposed == null)
        return;
      this.SessionDisposed((object) this, (EventArgs) null);
    }

    public event EventHandler SessionInitialized;

    public event EventHandler SessionDisposed;

    protected override void Initialize()
     {
      base.Initialize();
      this.RegisterVSService();
      this.InitializeSession();
      this.RegisterViewDefinitionManagerCommands();
      this.RegisterExplorationManagerCommands();
      this.RegisterWorkflowToolWindow();
      this.RegisterAddAction();
      this.RegisterDeclareRule();
      this.RegisterHelps();
      this.editorFactory = new EditorFactory(this);
      this.RegisterEditorFactory((IVsEditorFactory) this.editorFactory);
      foreach (Project allRealProject in ProjectUtils.GetAllRealProjects(this.DTE))
      {
        if (allRealProject != null)
          this.RegisterProjectToCordScopeManager(allRealProject);
      }
      this.viewFactory = new ViewDocumentFactory(this);
      this.RegisterEditorFactory((IVsEditorFactory) this.viewFactory);
      this.RegisterEditorFactory((IVsEditorFactory) new SummaryDocumentFactory(this));
      Guid guidSpecExplorerPkg = GuidList.guidSpecExplorerPkg;
      uint pdwCmdUICookie;
      this.AssertOk(this.MonitorSelection.GetCmdUIContextCookie(ref guidSpecExplorerPkg, out pdwCmdUICookie));
      this.AssertOk(this.MonitorSelection.SetCmdUIContext(pdwCmdUICookie, 1));
      IVsSolution service1 = this.GetService(typeof (IVsSolution)) as IVsSolution;
      this.Assert(service1 != null, "Failed to retrieve IVsSolution service.");
      uint pdwCookie1;
      this.AssertOk(service1.AdviseSolutionEvents((IVsSolutionEvents) this, out pdwCookie1));
      IVsTrackProjectDocuments2 service2 = (IVsTrackProjectDocuments2) this.GetService(typeof (SVsTrackProjectDocuments));
      this.Assert(service2 != null, "Failed to retrieve IVsTrackProjectDocumentsEvents2 service.");
      uint pdwCookie2;
      this.AssertOk(service2.AdviseTrackProjectDocumentsEvents((IVsTrackProjectDocumentsEvents2) this, out pdwCookie2));
      this.Assert(0U != pdwCookie2);
    }

    private void RegisterVSService()
    {
      IServiceContainer serviceContainer = (IServiceContainer) this;
      this.extensionManager = (IExtensionManager) new ExtensionManager();
      serviceContainer.AddService(typeof (IExtensionManager), (object) this.extensionManager, true);
      serviceContainer.AddService(typeof (IHost), (object) this, true);
    }

    protected override int QueryClose(out bool canClose)
    {
      this.FindToolWindow<ExplorationManagerToolWindow>().AbortOperation();
      return base.QueryClose(out canClose);
    }

    internal DTE DTE
    {
      get
      {
        if (this.dte == null)
          this.dte = this.GetRequiredService<DTE>(typeof (DTE));
        return this.dte;
      }
    }

    internal IVsSolutionBuildManager2 SolutionBuildManager
    {
      get
      {
        if (this.buildManager == null)
        {
          this.buildManager = this.GetService(typeof (SVsSolutionBuildManager)) as IVsSolutionBuildManager2;
          this.Assert(this.buildManager != null, "Failed to retrieve solution build manager service.");
        }
        return this.buildManager;
      }
    }

    internal IVsUIShell UIShell
    {
      get
      {
        if (this.uiShell == null)
          this.uiShell = this.GetRequiredService<IVsUIShell>(typeof (SVsUIShell));
        return this.uiShell;
      }
    }

    private IVsTextManager TextManager
    {
      get
      {
        if (this.textManager == null)
          this.textManager = this.GetRequiredService<IVsTextManager>(typeof (SVsTextManager));
        return this.textManager;
      }
    }

    private IVsMonitorSelection MonitorSelection
    {
      get
      {
        if (this.monitorSelection == null)
          this.monitorSelection = this.GetRequiredService<IVsMonitorSelection>(typeof (SVsShellMonitorSelection));
        return this.monitorSelection;
      }
    }

    private IVsStatusbar Statusbar
    {
      get
      {
        if (this.statusBar == null)
          this.statusBar = this.GetRequiredService<IVsStatusbar>(typeof (SVsStatusbar));
        return this.statusBar;
      }
    }

    internal S GetRequiredService<S>(System.Type type) where S : class
    {
      S s1 = default (S);
      if (this.CoreServices != null)
      {
        S service = this.CoreServices.GetService<S>();
        if ((object) service != null)
          return service;
      }
      object service1 = this.GetService(type);
      if (service1 == null)
      {
        this.FatalError(string.Format("cannot find required service {0}", (object) type.FullName));
        return default (S);
      }
      S s2 = service1 as S;
      if ((object) s2 != null)
        return s2;
      this.FatalError(string.Format("cannot get interface {0} of service {1}", (object) typeof (S).FullName, (object) type.FullName));
      return default (S);
    }

    private bool IsServiceAvailable<S>(System.Type t) where S : class
    {
      if (this.CoreServices != null && (object) this.CoreServices.GetService<S>() != null)
        return true;
      object service = this.GetService(t);
      if (service == null)
        return false;
      return service is S;
    }

    internal Project GetProjectByUniqueName(string uniqueName)
    {
      return ProjectUtils.GetAllRealProjects(this.DTE).FirstOrDefault<Project>((Func<Project, bool>) (project =>
      {
        if (project != null)
          return project.UniqueName == uniqueName;
        return false;
      }));
    }

    internal void ExecuteSEVSCommand(uint procedureId)
    {
      IVsUIShell service = (IVsUIShell) this.GetService(typeof (SVsUIShell));
      Guid specExplorerCmdSet = GuidList.guidSpecExplorerCmdSet;
      object pvaIn = (object) null;
      ErrorHandler.ThrowOnFailure(service.PostExecCommand(ref specExplorerCmdSet, procedureId, 0U, ref pvaIn));
    }

    private string GetActiveDocumentName()
    {
      Document activeDocument = this.DTE.ActiveDocument;
      if (activeDocument != null && activeDocument.FullName != null)
        return activeDocument.FullName;
      return (string) null;
    }

    private bool GetSelectedDocument(out IVsHierarchy hierarchy, out uint itemId)
    {
      hierarchy = (IVsHierarchy) null;
      itemId = 0U;
      object obj;
      if (this.MonitorSelection.GetCurrentElementValue(2U, out obj) == 0)
      {
        IVsWindowFrame vsWindowFrame = obj as IVsWindowFrame;
        if (vsWindowFrame != null)
        {
          this.AssertOk(vsWindowFrame.GetProperty(-4005, out obj));
          hierarchy = obj as IVsHierarchy;
          this.AssertOk(vsWindowFrame.GetProperty(-4006, out obj));
          if (obj is int)
            itemId = (uint) (int) obj;
          else
            hierarchy = (IVsHierarchy) null;
        }
      }
      return hierarchy != null;
    }

    private bool GetSelectedProjectItem(out IVsHierarchy hierarchy, out uint itemId)
    {
      IntPtr ppHier;
      IVsMultiItemSelect ppMIS;
      IntPtr ppSC;
      this.AssertOk(this.MonitorSelection.GetCurrentSelection(out ppHier, out itemId, out ppMIS, out ppSC));
      if (ppHier != IntPtr.Zero)
      {
        hierarchy = (IVsHierarchy) Marshal.GetTypedObjectForIUnknown(ppHier, typeof (IVsHierarchy));
        return true;
      }
      hierarchy = (IVsHierarchy) null;
      return false;
    }

    private Project GetProjectOfSelectedDocument()
    {
      IVsHierarchy hierarchy;
      uint itemId;
      if (this.GetSelectedDocument(out hierarchy, out itemId))
      {
        ProjectItem projectItem = this.GetProjectItem(hierarchy, itemId);
        if (projectItem != null)
          return projectItem.ContainingProject;
      }
      return (Project) null;
    }

    internal ProjectItem GetProjectItem(IVsHierarchy hierarchy, uint itemId)
    {
      object pvar;
      this.AssertOk(hierarchy.GetProperty(itemId, -2027, out pvar));
      return pvar as ProjectItem;
    }

    private bool GetHierarchy(
      ProjectItem item,
      out IVsHierarchy hierarchy,
      out uint itemId,
      out object docData)
    {
      if (item.Document == null)
        item.Open("{00000000-0000-0000-0000-000000000000}");
      RunningDocumentTable runningDocumentTable = new RunningDocumentTable((System.IServiceProvider) this);
      string str = item.get_FileNames((short) 0);
      foreach (RunningDocumentInfo runningDocumentInfo in runningDocumentTable)
      {
        if (runningDocumentInfo.Moniker == str && runningDocumentInfo.Hierarchy != null)
        {
          hierarchy = runningDocumentInfo.Hierarchy;
          itemId = runningDocumentInfo.ItemId;
          docData = runningDocumentInfo.DocData;
          return true;
        }
      }
      hierarchy = (IVsHierarchy) null;
      itemId = 0U;
      docData = (object) null;
      return false;
    }

    internal object GetDocData(ProjectItem item)
    {
      if (item.Document == null)
        item.Open("{00000000-0000-0000-0000-000000000000}");
      return new RunningDocumentTable((System.IServiceProvider) this).FindDocument(item.get_FileNames((short) 0));
    }

    internal T FindToolWindow<T>() where T : ToolWindowPane
    {
      T toolWindow = (T) this.FindToolWindow(typeof (T), 0, true);
      this.Assert((object) toolWindow != null && toolWindow.Frame != null, Microsoft.SpecExplorer.Resources.CanNotCreateWindow);
      return toolWindow;
    }

    protected override void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      IServiceContainer serviceContainer = (IServiceContainer) this;
      serviceContainer.RemoveService(typeof (IExtensionManager), true);
      serviceContainer.RemoveService(typeof (IHost), true);
      this.DisposeSession();
      if (this.errorList != null)
      {
        this.errorList.Dispose();
        this.errorList = (ErrorListProvider) null;
      }
      base.Dispose(disposing);
      this.disposed = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void InitializeViewDefinitionManager()
    {
      IViewDefinitionManager requiredService = (IViewDefinitionManager) this.CoreServices.GetRequiredService<IViewDefinitionManager>();
      if (!this.DTE.Solution.IsOpen || string.IsNullOrEmpty(this.DTE.Solution.FileName))
        return;
      Stream stream = this.OpenViewDefinitionStream(this.GetViewDefinitionFileName());
      if (stream != null)
      {
        using (stream)
        {
          try
          {
            requiredService.Load(stream);
          }
          catch (ViewDefinitionManagerException ex)
          {
            this.NotificationDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, string.Format("Error occured while loading view definitions:\n{0}", (object) ((Exception) ex).Message));
          }
        }
      }
      else
        requiredService.SetDeferredLoading((Func<Stream>) (() =>
        {
          string definitionFileName = this.GetViewDefinitionFileName();
          if (string.IsNullOrEmpty(definitionFileName))
            return (Stream) null;
          return this.OpenViewDefinitionStream(definitionFileName);
        }));
    }

    private void RegisterViewDefinitionManagerCommands()
    {
      OleMenuCommandService service = this.GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
      if (service == null)
        return;
      OleMenuCommand oleMenuCommand = new OleMenuCommand(new EventHandler(this.ShowViewDefinitionDialog), new CommandID(GuidList.guidSpecExplorerCmdSet, 264));
      service.AddCommand((MenuCommand) oleMenuCommand);
    }

    private Stream OpenViewDefinitionStream(string fileName)
    {
      if (!File.Exists(fileName))
        return (Stream) null;
      Stream stream = (Stream) null;
      try
      {
        stream = (Stream) File.OpenRead(fileName);
      }
      catch (UnauthorizedAccessException ex)
      {
        this.NotificationDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, string.Format("Error occured opening view definition file {0}:\n{1}", (object) fileName, (object) ex.Message));
      }
      return stream;
    }

    private string GetViewDefinitionFileName()
    {
      if (string.IsNullOrEmpty(this.DTE.Solution.FileName))
        return (string) null;
      return string.Format("{0}\\{1}{2}", (object) Path.GetDirectoryName(this.DTE.Solution.FullName), (object) Path.GetFileNameWithoutExtension(this.DTE.Solution.FullName), (object) ".sevu");
    }

    internal void ShowViewDefinitionDialog(object sender, EventArgs e)
    {
      using (ViewDefinitionManagerForm definitionManagerForm = new ViewDefinitionManagerForm((IHost) this, this.CoreServices == null ? (IViewDefinitionManager) new ViewDefinitionManager((IHost) this) : (IViewDefinitionManager) this.CoreServices.GetService<IViewDefinitionManager>(), this.GetViewDefinitionFileName()))
      {
        definitionManagerForm.ShowHelp += new EventHandler(this.ShowViewDefinitionHelp);
        int num = (int) ((Form) definitionManagerForm).ShowDialog();
      }
    }

    private void ShowViewDefinitionHelp(object sender, EventArgs e)
    {
      ((Help2) this.GetService(typeof (SVsHelp))).DisplayTopicFromF1Keyword("microsoft.specexplorer.viewerdefinitiondialog");
    }

    public int OnAfterCloseSolution(object pUnkReserved)
    {
      try
      {
        if (this.editorFactory != null)
          this.editorFactory.Clear();
        this.errorsSuppressed = 0;
        if (this.CoreServices != null)
          ((IViewDefinitionManager) this.CoreServices.GetRequiredService<IViewDefinitionManager>()).Reset();
        this.FindToolWindow<ExplorationManagerToolWindow>().SwitchView(false);
        this.FindToolWindow<WorkflowToolWindow>().UnloadWindowContent();
        this.ActivityCompletionStatus = string.Empty;
        this.UnregisterAllProjectsFromCordScopeManager();
        this.ClearErrorList();
        this.SpecExplorerPane.Clear();
      }
      finally
      {
        this.DisposeSession();
      }
      return 0;
    }

    public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
    {
      if (this.Session != null)
      {
        Project dteProject = this.ToDteProject(pRealHierarchy);
        if (dteProject != null)
        {
          this.CheckAndConvertProject(dteProject);
          this.RegisterProjectToCordScopeManager(dteProject);
        }
      }
      return 0;
    }

    public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
    {
      if (this.Session != null)
      {
        Project dteProject = this.ToDteProject(pHierarchy);
        if (dteProject != null)
        {
          this.CheckAndConvertProject(dteProject);
          this.RegisterProjectToCordScopeManager(dteProject);
        }
      }
      return 0;
    }

    public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
    {
      this.InitializeSession();
      SpecExplorerPackage.ReferenceUpgradeHelper referenceUpgradeHelper = new SpecExplorerPackage.ReferenceUpgradeHelper();
      foreach (Project allRealProject in ProjectUtils.GetAllRealProjects(this.DTE))
      {
        if (allRealProject != null)
        {
          this.CheckAndConvertProject(allRealProject);
          referenceUpgradeHelper.Handle(allRealProject);
        }
      }
      if (referenceUpgradeHelper.NeedUpgrade && MessageResult.YES == this.DecisionDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, string.Format("{0} finds the projects that you are opening contain references created by previous version of {0}, which might cause unexpected behaviors. Would you like to upgrade it to the latest version?", (object) Microsoft.SpecExplorer.Resources.SpecExplorer), MessageButton.YESNOCANCEL))
        this.NotificationDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, "Project references created by previous version of have been upgraded successfully");
      foreach (Project allRealProject in ProjectUtils.GetAllRealProjects(this.DTE))
      {
        if (allRealProject != null)
          this.RegisterProjectToCordScopeManager(allRealProject);
      }
      this.FindToolWindow<ExplorationManagerToolWindow>().SwitchView(true);
      if (this.WasWorkflowLoadedFlag || this.FindToolWindow<WorkflowToolWindow>().IsWindowVisible)
        this.LoadWorkflowToolWindow();
      return 0;
    }

    public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
    {
      Project dteProject = this.ToDteProject(pHierarchy);
      if (dteProject != null)
        this.UnregisterProjectFromCordScopeManager(dteProject);
      return 0;
    }

    public int OnBeforeCloseSolution(object pUnkReserved)
    {
      return 0;
    }

    public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
    {
      Project dteProject = this.ToDteProject(pRealHierarchy);
      if (dteProject != null)
        this.UnregisterProjectFromCordScopeManager(dteProject);
      return 0;
    }

    internal Project ToDteProject(IVsHierarchy hierarchy)
    {
      object pvar = (object) null;
      if (hierarchy != null && hierarchy.GetProperty(4294967294U, -2027, out pvar) >= 0)
        return (Project) pvar;
      return (Project) null;
    }

    private Project ToDteProject(IVsProject project)
    {
      if (project == null)
        throw new ArgumentNullException(project.ToString());
      return this.ToDteProject(project as IVsHierarchy);
    }

    internal IVsHierarchy ToHierarchy(Project project)
    {
      if (project == null)
        throw new ArgumentNullException(project.Name);
      string g = (string) null;
      using (XmlReader xmlReader = XmlReader.Create(project.FileName))
      {
        int content = (int) xmlReader.MoveToContent();
        object objB = (object) xmlReader.NameTable.Add("ProjectGuid");
        while (xmlReader.Read())
        {
          if (object.Equals((object) xmlReader.LocalName, objB))
          {
            g = xmlReader.ReadElementContentAsString();
            break;
          }
        }
      }
      return VsShellUtilities.GetHierarchy((System.IServiceProvider) new ServiceProvider(project.DTE as Microsoft.VisualStudio.OLE.Interop.IServiceProvider), new Guid(g));
    }

    public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
    {
      return 0;
    }

    public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
    {
       return 0;
    }

    public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
    {
      return 0;
    }

    public ISession Session
    {
      get
      {
        return this.session;
      }
    }

    public ComponentBase CoreServices
    {
      get
      {
        return this.session as ComponentBase;
      }
    }

    internal IEnumerable<ProjectItem> GetAuthoredCSharpDocuments(
      ProjectItems items)
    {
      if (items != null)
      {
        IEnumerator enumerator = items.GetEnumerator();
        try
        {
          while (enumerator.MoveNext())
          {
            ProjectItem item = (ProjectItem) enumerator.Current;
            string name = item.Name;
            if (FileNames.HasCSharpExtension(name))
              yield return item;
            foreach (ProjectItem authoredCsharpDocument in this.GetAuthoredCSharpDocuments(item.ProjectItems))
              yield return authoredCsharpDocument;
          }
        }
        finally
        {
          IDisposable disposable = enumerator as IDisposable;
          disposable.Dispose();
        }
      }
    }

    internal bool BuildProject(Project project)
    {
      this.currentTaskCategory = TaskCategory.BuildCompile;
      try
      {
        SolutionConfiguration2 activeConfiguration = this.DTE.Solution.SolutionBuild.ActiveConfiguration as SolutionConfiguration2;
        this.Assert(activeConfiguration != null);
                //this.DTE.Solution.SolutionBuild.BuildProject(activeConfiguration + "|" + activeConfiguration.PlatformName, project.UniqueName, true);
        this.DTE.Solution.SolutionBuild.BuildProject("Debug" + "|" + "Any CPU", project.UniqueName, true);
      }
      catch (COMException ex)
      {
        this.DiagMessage((DiagnosisKind) 0, Microsoft.SpecExplorer.Resources.FailedToBuildProject, (object) null);
        return false;
      }
      return this.DTE.Solution.SolutionBuild.LastBuildInfo <= 0;
    }

    internal ICollection<string> CollectReferences(Project project)
    {
      this.Assert(project != null);
      List<string> stringList = new List<string>();
      string str1 = project.Properties.Item("FullPath").ToString();
      string str2 = project.Properties.Item("OutputFileName").ToString();
      string str3 = project.ConfigurationManager.ActiveConfiguration.Properties.Item((object) "OutputPath").ToString();
      string str4 = string.Format("{0}\\{1}\\{2}", (object) str1, (object) str3, (object) str2);
      stringList.Add(str4);
      this.ProgressMessage((VerbosityLevel) 2, string.Format("referencing {0}", (object) str4));
      if (project.Kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}" || project.Kind == "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}")
      {
        foreach (Reference3 reference in (project.Object as VSProject2).References)
        {
          if (!reference.AutoReferenced)
          {
            if (string.IsNullOrEmpty(reference.Path))
            {
              this.DiagMessage((DiagnosisKind) 1, string.Format("Referenced assembly cannot be found: {0}", (object) reference.Name), (object) null);
            }
            else
            {
              this.ProgressMessage((VerbosityLevel) 2, string.Format("referencing {0}", (object) reference.Path));
              if(!reference.Path.Contains("System__COMObject"))
              {
                 stringList.Add(reference.Path);
              }
            }
          }
        }
      }
      stringList.RemoveAt(0);
      return (ICollection<string>) stringList;
    }

    internal void CollectScripts(List<string> scripts, ProjectItems items)
    {
      foreach (ProjectItem projectItem in items)
      {
        if (FileNames.HasScriptExtension(projectItem.Name))
        {
          string str = projectItem.get_FileNames((short) 0);
          scripts.Add(str);
          this.ProgressMessage((VerbosityLevel) 2, string.Format("including {0}", (object) str));
        }
        this.CollectScripts(scripts, projectItem.ProjectItems);
      }
    }

    internal bool SolutionHasCSharpClass()
    {
      return ProjectUtils.GetAllRealProjects(this.DTE).Any<Project>((Func<Project, bool>) (proj => this.ProjectHasCSharpClass(proj)));
    }

    internal bool ProjectHasCSharpClass(Project project)
    {
      if (project == null)
        return false;
      IEnumerable<ProjectItem> authoredCsharpDocuments = this.GetAuthoredCSharpDocuments(project.ProjectItems);
      if (authoredCsharpDocuments != null)
        return authoredCsharpDocuments.Any<ProjectItem>((Func<ProjectItem, bool>) (item =>
        {
          if (item != null && item.FileCodeModel != null && item.FileCodeModel.CodeElements != null)
            return item.FileCodeModel.CodeElements.Cast<CodeElement>().Any<CodeElement>((Func<CodeElement, bool>) (codeElem => this.CodeElementHasCodeClass(codeElem)));
          return false;
        }));
      return false;
    }

    private bool CodeElementHasCodeClass(CodeElement parent)
    {
      CodeElements codeElements = (CodeElements) null;
      CodeNamespace codeNamespace = parent as CodeNamespace;
      if (codeNamespace != null)
      {
        codeElements = codeNamespace.Members;
      }
      else
      {
        CodeType codeType = parent as CodeType;
        if (codeType != null)
          codeElements = codeType.Members;
      }
      if (codeElements != null)
      {
        foreach (CodeElement parent1 in codeElements)
        {
          if (parent1.Kind == vsCMElement.vsCMElementClass || this.CodeElementHasCodeClass(parent1))
            return true;
        }
      }
      return false;
    }

    internal string ComputePathRelativeToProject(Project project, ProjectItem keyItem)
    {
      string fromProjectItems = this.ComputePathFromProjectItems(project.ProjectItems, keyItem);
      if (!string.IsNullOrEmpty(fromProjectItems))
        return project + "\\" + fromProjectItems;
      return (string) null;
    }

    internal string ComputePathFromProjectItems(ProjectItems itemCollection, ProjectItem keyItem)
    {
      if (itemCollection != null)
      {
        foreach (ProjectItem projectItem in itemCollection)
        {
          if (projectItem == keyItem)
            return projectItem.Name;
          string fromProjectItems = this.ComputePathFromProjectItems(projectItem.ProjectItems, keyItem);
          if (!string.IsNullOrEmpty(fromProjectItems))
            return projectItem + "\\" + fromProjectItems;
        }
      }
      return (string) null;
    }

    public int OnAfterAddDirectoriesEx(
      int cProjects,
      int cDirectories,
      IVsProject[] rgpProjects,
      int[] rgFirstIndices,
      string[] rgpszMkDocuments,
      VSADDDIRECTORYFLAGS[] rgFlags)
    {
      return 0;
    }

    public int OnAfterAddFilesEx(
      int cProjects,
      int cFiles,
      IVsProject[] rgpProjects,
      int[] rgFirstIndices,
      string[] rgpszMkDocuments,
      VSADDFILEFLAGS[] rgFlags)
    {
      for (int index1 = 0; index1 < cProjects; ++index1)
      {
        Project dteProject = this.ToDteProject(rgpProjects[index1]);
        if (dteProject != null)
        {
          int rgFirstIndex = rgFirstIndices[index1];
          int num = index1 < cProjects - 1 ? rgFirstIndices[index1 + 1] : cFiles;
          for (int index2 = rgFirstIndex; index2 < num; ++index2)
          {
            string rgpszMkDocument = rgpszMkDocuments[index2];
            if (FileNames.HasScriptExtension(rgpszMkDocument))
              this.RegisterScriptToCordDesignTimeManager(dteProject, rgpszMkDocument);
          }
        }
      }
      return 0;
    }

    public int OnAfterRemoveDirectories(
      int cProjects,
      int cDirectories,
      IVsProject[] rgpProjects,
      int[] rgFirstIndices,
      string[] rgpszMkDocuments,
      VSREMOVEDIRECTORYFLAGS[] rgFlags)
    {
      return 0;
    }

    public int OnAfterRemoveFiles(
      int cProjects,
      int cFiles,
      IVsProject[] rgpProjects,
      int[] rgFirstIndices,
      string[] rgpszMkDocuments,
      VSREMOVEFILEFLAGS[] rgFlags)
    {
      for (int index1 = 0; index1 < cProjects; ++index1)
      {
        Project dteProject = this.ToDteProject(rgpProjects[index1]);
        if (dteProject != null)
        {
          int rgFirstIndex = rgFirstIndices[index1];
          int num = index1 < cProjects - 1 ? rgFirstIndices[index1 + 1] : cFiles;
          for (int index2 = rgFirstIndex; index2 < num; ++index2)
          {
            string rgpszMkDocument = rgpszMkDocuments[index2];
            if (FileNames.HasScriptExtension(rgpszMkDocument))
            {
              this.UnregisterScriptFromCordDesignTimeManager(dteProject.UniqueName, rgpszMkDocument);
              this.editorFactory.Remove(rgpszMkDocument);
            }
          }
        }
      }
      return 0;
    }

    public int OnAfterRenameDirectories(
      int cProjects,
      int cDirs,
      IVsProject[] rgpProjects,
      int[] rgFirstIndices,
      string[] rgszMkOldNames,
      string[] rgszMkNewNames,
      VSRENAMEDIRECTORYFLAGS[] rgFlags)
    {
      return 0;
    }

    public int OnAfterRenameFiles(
      int cProjects,
      int cFiles,
      IVsProject[] rgpProjects,
      int[] rgFirstIndices,
      string[] rgszMkOldNames,
      string[] rgszMkNewNames,
      VSRENAMEFILEFLAGS[] rgFlags)
    {
      for (int index1 = 0; index1 < cProjects; ++index1)
      {
        Project dteProject = this.ToDteProject(rgpProjects[index1]);
        if (dteProject != null)
        {
          int rgFirstIndex = rgFirstIndices[index1];
          int num = index1 < cProjects - 1 ? rgFirstIndices[index1 + 1] : cFiles;
          for (int index2 = rgFirstIndex; index2 < num; ++index2)
            this.OnFileRenamed(dteProject, rgszMkNewNames[index2], rgszMkOldNames[index2]);
        }
      }
      return 0;
    }

    private void OnFileRenamed(Project project, string newFileName, string oldFileName)
    {
      string uniqueName = project.UniqueName;
      if (string.Compare(project.FullName, newFileName, StringComparison.OrdinalIgnoreCase) == 0 && newFileName.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) && oldFileName.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
      {
        Tuple<string, string> key = new Tuple<string, string>(oldFileName.ToLower(), newFileName.ToLower());
        string str;
        if (!this.projectRenameQueries.TryGetValue(key, out str) || this.CordScopeManager == null || (this.CordScopeManager.AllScopes == null || !this.CordScopeManager.AllScopes.Contains(oldFileName)))
          return;
        this.CordScopeManager.ChangeRegistrationKey(str, uniqueName);
        this.projectRenameQueries.Remove(key);
      }
      else if (FileNames.HasScriptExtension(oldFileName) && FileNames.HasScriptExtension(newFileName))
      {
        this.UnregisterScriptFromCordDesignTimeManager(uniqueName, oldFileName);
        CordDocument cordDocumentByName = this.editorFactory.GetOpenedCordDocumentByName(oldFileName);
        if (cordDocumentByName != null)
        {
          cordDocumentByName.FileName = newFileName;
          this.editorFactory.Rename(oldFileName, cordDocumentByName);
          this.RegisterCordDocumentToDesignTimeManager(cordDocumentByName, true);
        }
        else
          this.RegisterScriptToCordDesignTimeManager(project, newFileName);
      }
      else if (FileNames.HasScriptExtension(oldFileName) && !FileNames.HasScriptExtension(newFileName))
      {
        this.UnregisterScriptFromCordDesignTimeManager(uniqueName, oldFileName);
        if (this.editorFactory.GetOpenedCordDocumentByName(oldFileName) == null)
          return;
        this.editorFactory.Remove(oldFileName);
      }
      else
      {
        if (FileNames.HasScriptExtension(oldFileName) || !FileNames.HasScriptExtension(newFileName))
          return;
        IVsUIHierarchy hierarchy;
        uint itemID;
        IVsWindowFrame windowFrame;
        if (VsShellUtilities.IsDocumentOpen((System.IServiceProvider) this, newFileName, Guid.Empty, out hierarchy, out itemID, out windowFrame))
        {
          this.editorFactory.OpenDocument(newFileName);
          CordDocument cordDocumentByName = this.editorFactory.GetOpenedCordDocumentByName(newFileName);
          this.Assert(cordDocumentByName != null);
          this.RegisterCordDocumentToDesignTimeManager(cordDocumentByName, true);
        }
        else
          this.RegisterScriptToCordDesignTimeManager(project, newFileName);
      }
    }

    public int OnAfterSccStatusChanged(
      int cProjects,
      int cFiles,
      IVsProject[] rgpProjects,
      int[] rgFirstIndices,
      string[] rgpszMkDocuments,
      uint[] rgdwSccStatus)
    {
      return 0;
    }

    public int OnQueryAddDirectories(
      IVsProject pProject,
      int cDirectories,
      string[] rgpszMkDocuments,
      VSQUERYADDDIRECTORYFLAGS[] rgFlags,
      VSQUERYADDDIRECTORYRESULTS[] pSummaryResult,
      VSQUERYADDDIRECTORYRESULTS[] rgResults)
    {
      return 0;
    }

    public int OnQueryAddFiles(
      IVsProject pProject,
      int cFiles,
      string[] rgpszMkDocuments,
      VSQUERYADDFILEFLAGS[] rgFlags,
      VSQUERYADDFILERESULTS[] pSummaryResult,
      VSQUERYADDFILERESULTS[] rgResults)
    {
      return 0;
    }

    public int OnQueryRemoveDirectories(
      IVsProject pProject,
      int cDirectories,
      string[] rgpszMkDocuments,
      VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags,
      VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult,
      VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
    {
      return 0;
    }

    public int OnQueryRemoveFiles(
      IVsProject pProject,
      int cFiles,
      string[] rgpszMkDocuments,
      VSQUERYREMOVEFILEFLAGS[] rgFlags,
      VSQUERYREMOVEFILERESULTS[] pSummaryResult,
      VSQUERYREMOVEFILERESULTS[] rgResults)
    {
      return 0;
    }

    public int OnQueryRenameDirectories(
      IVsProject pProject,
      int cDirs,
      string[] rgszMkOldNames,
      string[] rgszMkNewNames,
      VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags,
      VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult,
      VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
    {
      return 0;
    }

    public int OnQueryRenameFiles(
      IVsProject pProject,
      int cFiles,
      string[] rgszMkOldNames,
      string[] rgszMkNewNames,
      VSQUERYRENAMEFILEFLAGS[] rgFlags,
      VSQUERYRENAMEFILERESULTS[] pSummaryResult,
      VSQUERYRENAMEFILERESULTS[] rgResults)
    {
      for (int index = 0; index < cFiles; ++index)
      {
        Project dteProject = this.ToDteProject(pProject);
        string rgszMkOldName = rgszMkOldNames[index];
        string rgszMkNewName = rgszMkNewNames[index];
        if (dteProject != null && string.Compare(dteProject.FullName, rgszMkOldName, StringComparison.OrdinalIgnoreCase) == 0 && (rgszMkOldName.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) && rgszMkNewName.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)))
          this.projectRenameQueries[new Tuple<string, string>(rgszMkOldName.ToLower(), rgszMkNewName.ToLower())] = dteProject.UniqueName;
      }
      return 0;
    }

    public int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
    {
      return 0;
    }

    public int UpdateSolution_Begin(ref int pfCancelUpdate)
    {
      return 0;
    }

    public int UpdateSolution_Cancel()
    {
      if (this.SolutionBuildFinished != null)
        this.SolutionBuildFinished((object) this, new SolutionBuildEventArgs(true, false));
      return 0;
    }

    public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
    {
      if (this.SolutionBuildFinished != null)
        this.SolutionBuildFinished((object) this, new SolutionBuildEventArgs(fCancelCommand == 1, fSucceeded == 1));
      return 0;
    }

    public event EventHandler<SolutionBuildEventArgs> SolutionBuildFinished;

    public int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
    {
      return 0;
    }

    public IntPtr MainWindowHandle
    {
      get
      {
        IntPtr phwnd;
        this.AssertOk((this.GetService(typeof (SVsUIShell)) as IVsUIShell).GetDialogOwnerHwnd(out phwnd));
        return phwnd;
      }
    }

    internal ErrorListProvider ErrorList
    {
      get
      {
        if (this.errorList == null)
          this.errorList = new ErrorListProvider((System.IServiceProvider) this);
        return this.errorList;
      }
    }

    internal TextSpan[] MakeTextSpan(int line, int column)
    {
      TextSpan[] textSpanArray = new TextSpan[1];
      textSpanArray[0].iStartLine = line;
      textSpanArray[0].iStartIndex = column;
      textSpanArray[0].iEndLine = line;
      textSpanArray[0].iEndIndex = column;
      return textSpanArray;
    }

    internal TextSpan[] MakeTextSpan(object locator)
    {
      if (!(locator is TextLocation))
        return this.MakeTextSpan(0, 0);
      TextLocation textLocation = (TextLocation) locator;
      return this.MakeTextSpan((int) ((TextLocation) textLocation).FirstLine - 1, (int) ((TextLocation) textLocation).FirstColumn - 1);
    }

    internal void MakeErrorListVisible()
    {
      if (this.errorsSuppressed != 0)
        return;
      this.DTE.Windows.Item((object) "{D78612C7-9962-4B83-95D9-268046DAD23A}").Activate();
    }

    internal void ClearErrorList()
    {
      if (this.errorsSuppressed != 0)
        return;
      this.ErrorList.Tasks.Clear();
    }

    internal void ClearParsingErrorList()
    {
      if (this.errorsSuppressed != 0)
        return;
      this.ErrorList.SuspendRefresh();
      int index = 0;
      while (index < this.ErrorList.Tasks.Count)
      {
        SpecExplorerPackage.SpecExplorerError task = this.ErrorList.Tasks[index] as SpecExplorerPackage.SpecExplorerError;
        if (task != null && task.IsParsingError)
          this.ErrorList.Tasks.RemoveAt(index);
        else
          ++index;
      }
      this.ErrorList.ResumeRefresh();
    }

    internal void SupressErrorProcessing()
    {
      ++this.errorsSuppressed;
    }

    internal void ResumeErrorProcessing()
    {
      --this.errorsSuppressed;
    }

    private OutputWindowPane SpecExplorerPane
    {
      get
      {
        if (this.specExplorerPane == null)
        {
          OutputWindow outputWindow = this.DTE.Windows.Item((object) "{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}").Object as OutputWindow;
          foreach (OutputWindowPane outputWindowPane in outputWindow.OutputWindowPanes)
          {
            if (outputWindowPane.Name == Microsoft.SpecExplorer.Resources.SpecExplorer)
            {
              this.specExplorerPane = outputWindowPane;
              break;
            }
          }
          if (this.specExplorerPane == null)
            this.specExplorerPane = outputWindow.OutputWindowPanes.Add(Microsoft.SpecExplorer.Resources.SpecExplorer);
        }
        return this.specExplorerPane;
      }
    }

    private OutputWindowPane DebugPane
    {
      get
      {
        if (this.debugPane == null)
        {
          OutputWindow outputWindow = this.DTE.Windows.Item((object) "{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}").Object as OutputWindow;
          foreach (OutputWindowPane outputWindowPane in outputWindow.OutputWindowPanes)
          {
            if (outputWindowPane.Name == "Debug")
            {
              this.debugPane = outputWindowPane;
              break;
            }
          }
          if (this.debugPane == null)
            this.debugPane = outputWindow.OutputWindowPanes.Add("Debug");
          this.debugPane.Activate();
        }
        return this.debugPane;
      }
    }

    private bool DebugPaneIsAvailable
    {
      get
      {
        return this.IsServiceAvailable<DTE>(typeof (DTE));
      }
    }

    internal void ActivateDebugPane()
    {
      this.DebugPane.Activate();
    }

    private CommandWindow VsCommandWindow
    {
      get
      {
        if (this.commandWindow == null)
          this.commandWindow = this.DTE.Windows.Item((object) "{28836128-FC2C-11D2-A433-00C04F72D18A}").Object as CommandWindow;
        return this.commandWindow;
      }
    }

    internal string ActivityCompletionStatus { get; set; }

    internal string LastUsedGuidance
    {
      get
      {
        return this.lastUsedGuidance;
      }
    }

    private void ShowWorkflowToolWindow(object sender, EventArgs e)
    {
      this.LoadWorkflowToolWindow();
    }

    private void LoadWorkflowToolWindow()
    {
      WorkflowToolWindow toolWindow = this.FindToolWindow<WorkflowToolWindow>();
      ErrorHandler.ThrowOnFailure(((IVsWindowFrame) toolWindow.Frame).Show());
      if (!this.DTE.Solution.IsOpen || toolWindow.IsWindowContentLoaded)
        return;
      toolWindow.LoadWindowContent();
    }

    private void RegisterWorkflowToolWindow()
    {
      (this.GetService(typeof (IMenuCommandService)) as OleMenuCommandService).AddCommand((MenuCommand) new OleMenuCommand(new EventHandler(this.ShowWorkflowToolWindow), new CommandID(GuidList.guidSpecExplorerCmdSet, 4369)));
    }

    private bool WasWorkflowLoadedFlag
    {
      get
      {
        return this.wasWorkflowLoaded;
      }
    }

    private bool SolutionHasDirtyProps
    {
      get
      {
        WorkflowToolWindow toolWindow = this.FindToolWindow<WorkflowToolWindow>();
        if (toolWindow.IsWindowContentLoaded)
          return this.ActivityCompletionStatus != toolWindow.GuidanceActivityCompletionStatus;
        return false;
      }
    }

    private bool IsSolutionHierarchy(IVsHierarchy hierarchy)
    {
      return this.ToDteProject(hierarchy) == null;
    }

    public int OnProjectLoadFailure(
      [In] IVsHierarchy pStubHierarchy,
      [In] string pszProjectName,
      [In] string pszProjectMk,
      [In] string pszKey)
    {
      return 0;
    }

    public int QuerySaveSolutionProps([In] IVsHierarchy pHierarchy, [Out] VSQUERYSAVESLNPROPS[] pqsspSave)
    {
      pqsspSave[0] = !this.IsSolutionHierarchy(pHierarchy) ? VSQUERYSAVESLNPROPS.QSP_HasNoProps : (this.SolutionHasDirtyProps ? VSQUERYSAVESLNPROPS.QSP_HasDirtyProps : VSQUERYSAVESLNPROPS.QSP_HasNoDirtyProps);
      return 0;
    }

    public int SaveSolutionProps([In] IVsHierarchy pHierarchy, [In] IVsSolutionPersistence pPersistence)
    {
      if (!this.IsSolutionHierarchy(pHierarchy))
        return 0;
      this.ActivityCompletionStatus = this.FindToolWindow<WorkflowToolWindow>().GuidanceActivityCompletionStatus;
      return pPersistence.SavePackageSolutionProps(1, (IVsHierarchy) null, (IVsPersistSolutionProps) this, "SpecExplorer.ActivityCompletionStatus");
    }

    public int WriteSolutionProps([In] IVsHierarchy pHierarchy, [In] string pszKey, [In] Microsoft.VisualStudio.OLE.Interop.IPropertyBag pPropBag)
    {
      if (this.IsSolutionHierarchy(pHierarchy) && !string.IsNullOrEmpty(this.ActivityCompletionStatus))
      {
        object completionStatus = (object) this.ActivityCompletionStatus;
        pPropBag.Write("SpecExplorer.ActivityCompletionStatus", ref completionStatus);
      }
      return 0;
    }

    public int ReadSolutionProps(
      [In] IVsHierarchy pHierarchy,
      [In] string pszProjectName,
      [In] string pszProjectMk,
      [In] string pszKey,
      [In] int fPreLoad,
      [In] Microsoft.VisualStudio.OLE.Interop.IPropertyBag pPropBag)
    {
      if ("SpecExplorer.ActivityCompletionStatus".Equals(pszKey, StringComparison.CurrentCultureIgnoreCase))
      {
        object pVar;
        pPropBag.Read("SpecExplorer.ActivityCompletionStatus", out pVar, (Microsoft.VisualStudio.OLE.Interop.IErrorLog) null, 0U, (object) null);
        if (pVar != null)
          this.ActivityCompletionStatus = pVar.ToString();
      }
      return 0;
    }

    public int SaveUserOptions([In] IVsSolutionPersistence pPersistence)
    {
      WorkflowToolWindow toolWindow = this.FindToolWindow<WorkflowToolWindow>();
      if (toolWindow.IsWindowContentLoaded)
      {
        string currentGuidance = toolWindow.CurrentGuidance;
        if (this.lastUsedGuidance != currentGuidance && currentGuidance != null)
        {
          this.lastUsedGuidance = currentGuidance;
          int hr = pPersistence.SavePackageUserOpts((IVsPersistSolutionOpts) this, "SpecExplorerLastUsedGuidance");
          if (ErrorHandler.Failed(hr))
            return hr;
        }
      }
      this.wasWorkflowLoaded = toolWindow.IsWindowVisibleAndLoaded;
      return pPersistence.SavePackageUserOpts((IVsPersistSolutionOpts) this, "SpecExplorerWasWorkflowLoaded");
    }

    public int WriteUserOptions([In] Microsoft.VisualStudio.OLE.Interop.IStream pOptionsStream, [In] string pszKey)
    {
      string writeString = (string) null;
      switch (pszKey)
      {
        case "SpecExplorerLastUsedGuidance":
          writeString = this.lastUsedGuidance;
          break;
        case "SpecExplorerWasWorkflowLoaded":
          writeString = this.wasWorkflowLoaded.ToString();
          break;
      }
      SpecExplorerPackage.WriteStringToStream(pOptionsStream, writeString);
      return 0;
    }

    public int ReadUserOptions([In] Microsoft.VisualStudio.OLE.Interop.IStream pOptionsStream, [In] string pszKey)
    {
      string str = SpecExplorerPackage.ReadStringFromStream(pOptionsStream);
      switch (pszKey)
      {
        case "SpecExplorerLastUsedGuidance":
          this.lastUsedGuidance = str;
          break;
        case "SpecExplorerWasWorkflowLoaded":
          bool.TryParse(str, out this.wasWorkflowLoaded);
          break;
      }
      return 0;
    }

    public int LoadUserOptions(IVsSolutionPersistence pPersistence, uint grfLoadOpts)
    {
      this.lastUsedGuidance = string.Empty;
      pPersistence.LoadPackageUserOpts((IVsPersistSolutionOpts) this, "SpecExplorerLastUsedGuidance");
      this.wasWorkflowLoaded = false;
      pPersistence.LoadPackageUserOpts((IVsPersistSolutionOpts) this, "SpecExplorerWasWorkflowLoaded");
      return 0;
    }

    private static string ReadStringFromStream(Microsoft.VisualStudio.OLE.Interop.IStream pOptionsStream)
    {
      LARGE_INTEGER dlibMove = new LARGE_INTEGER()
      {
        QuadPart = 0
      };
      ULARGE_INTEGER[] plibNewPosition = new ULARGE_INTEGER[1]
      {
        new ULARGE_INTEGER()
      };
      pOptionsStream.Seek(dlibMove, 2U, plibNewPosition);
      uint quadPart = (uint) plibNewPosition[0].QuadPart;
      pOptionsStream.Seek(dlibMove, 0U, plibNewPosition);
      byte[] numArray = new byte[quadPart];
      uint pcbRead;
      pOptionsStream.Read(numArray, quadPart, out pcbRead);
      return new ASCIIEncoding().GetString(numArray, 0, (int) pcbRead);
    }

    private static void WriteStringToStream(Microsoft.VisualStudio.OLE.Interop.IStream pOptionsStream, string writeString)
    {
      if (writeString == null)
        return;
      byte[] bytes = new ASCIIEncoding().GetBytes(writeString);
      uint pcbWritten;
      pOptionsStream.Write(bytes, (uint) bytes.Length, out pcbWritten);
    }

    internal sealed class ReferenceUpgradeHelper
    {
      private static readonly string LatestestXrtRuntimeReferenceFullName = typeof (RuleAttribute).Assembly.FullName;
      private static readonly string LatestestSERuntimeReferenceFullName = typeof (TestAdapterAttribute).Assembly.FullName;
      private static readonly string LatestestSERuntimeVSReferenceFullName = typeof (VsTestClassBase).Assembly.FullName;
      private List<SpecExplorerPackage.ReferenceUpgradeItem> references = new List<SpecExplorerPackage.ReferenceUpgradeItem>();
      private const string LatestReferenceVersion = "2.2.0.0";

      private bool IsLatestReferenceVersion(string currentReferenceVersion)
      {
        return string.Equals(currentReferenceVersion, "2.2.0.0", StringComparison.OrdinalIgnoreCase);
      }

      private void AddReference(VSProject2 project, Reference3 r, string referenceFullPath)
      {
        this.references.Add(new SpecExplorerPackage.ReferenceUpgradeItem()
        {
          Project = project,
          Reference = r,
          LatestReferencePath = referenceFullPath
        });
      }

      private Reference3 GetReference(
        VSProject2 vsp,
        string assemblyname,
        ref string version)
      {
        foreach (Reference3 reference in vsp.References)
        {
          if (!reference.AutoReferenced)
          {
            string[] strArray = reference.Identity.Split(new char[1]
            {
              ','
            }, StringSplitOptions.RemoveEmptyEntries);
            if (string.Equals(strArray[0], assemblyname, StringComparison.OrdinalIgnoreCase))
            {
              version = strArray[1];
              return reference;
            }
          }
        }
        return (Reference3) null;
      }

      private Reference3 GetOldVersionReference(
        VSProject2 selectedProject,
        string assemblyname,
        ref string currentReferenceVersion)
      {
        Reference3 reference = selectedProject.References.Find(assemblyname) as Reference3;
        if (reference != null)
          currentReferenceVersion = reference.Version;
        else
          reference = this.GetReference(selectedProject, assemblyname, ref currentReferenceVersion);
        return reference;
      }

      public void Handle(Project project)
      {
        if (!(project.Kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") && !(project.Kind == "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"))
          return;
        VSProject2 project1 = project.Object as VSProject2;
        if (project1 == null)
          return;
        this.UpgradeReference(project1, "Microsoft.Xrt.Runtime", SpecExplorerPackage.ReferenceUpgradeHelper.LatestestXrtRuntimeReferenceFullName);
        this.UpgradeReference(project1, "Microsoft.SpecExplorer.Runtime", SpecExplorerPackage.ReferenceUpgradeHelper.LatestestSERuntimeReferenceFullName);
        this.UpgradeReference(project1, "Microsoft.SpecExplorer.Runtime.VisualStudio", SpecExplorerPackage.ReferenceUpgradeHelper.LatestestSERuntimeVSReferenceFullName);
      }

      private void UpgradeReference(
        VSProject2 project,
        string referenceName,
        string latestReferenceFullName)
      {
        string empty = string.Empty;
        Reference3 versionReference = this.GetOldVersionReference(project, referenceName, ref empty);
        if (versionReference == null || this.IsLatestReferenceVersion(empty))
          return;
        this.AddReference(project, versionReference, SpecExplorerPackage.ReferenceUpgradeHelper.LatestestXrtRuntimeReferenceFullName);
      }

      public bool NeedUpgrade
      {
        get
        {
          return this.references.Count > 0;
        }
      }

      public bool Upgrade()
      {
        try
        {
          foreach (SpecExplorerPackage.ReferenceUpgradeItem reference in this.references)
          {
            reference.Reference.Remove();
            reference.Project.References.Add(reference.LatestReferencePath);
            reference.Project.Project.Save("");
          }
          this.references.Clear();
          return true;
        }
        catch (Exception ex)
        {
          this.references.Clear();
          return false;
        }
      }
    }

    internal sealed class ReferenceUpgradeItem
    {
      public VSProject2 Project;
      public Reference3 Reference;
      public string LatestReferencePath;
    }

    private class Win32Window : IWin32Window
    {
      private IntPtr handle;

      internal Win32Window(IntPtr handle)
      {
        this.handle = handle;
      }

      public IntPtr Handle
      {
        get
        {
          return this.handle;
        }
      }
    }

    private class SpecExplorerError : ErrorTask
    {
      private SpecExplorerPackage package;

      internal bool IsParsingError { get; private set; }

      internal SpecExplorerError(
        SpecExplorerPackage package,
        TaskCategory category,
        DiagnosisKind kind,
        string fileName,
        bool isParsingError,
        int line,
        int column,
        string message)
      {
        if (package == null)
          throw new ArgumentNullException(package.ToString());
        if (fileName == null)
          throw new ArgumentNullException(fileName.ToString());
        this.package = package;
        this.IsParsingError = isParsingError;
        switch ((int) kind)
        {
          case 0:
            this.Priority = TaskPriority.High;
            this.ErrorCategory = TaskErrorCategory.Error;
            break;
          case 1:
            this.Priority = TaskPriority.Normal;
            this.ErrorCategory = TaskErrorCategory.Warning;
            break;
          case 2:
            this.Priority = TaskPriority.Low;
            this.ErrorCategory = TaskErrorCategory.Message;
            break;
        }
        int length1 = fileName.IndexOf("$");
        if (length1 >= 0)
          fileName = fileName.Substring(0, length1);
        int length2 = fileName.LastIndexOf("?");
        if (length2 >= 0)
          fileName = fileName.Substring(0, length2);
        this.Document = fileName;
        this.Line = line - 1;
        this.Column = column - 1;
        this.Text = message;
        this.Category = category;
      }

      protected override void OnNavigate(EventArgs e)
      {
        string document = this.Document;
        if (FileNames.HasScriptExtension(document))
        {
          this.package.NavigateTo(document, this.Line, this.Column);
        }
        else
        {
          try
          {
            (this.package.DTE.ItemOperations.OpenFile(document, (string) null).Selection as TextSelection).GotoLine(this.Line + 1, false);
          }
          catch
          {
          }
        }
      }
    }

        public int UpdateProjectCfg_Begin(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int UpdateProjectCfg_Done(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            return VSConstants.S_OK;
        }
    }
}
