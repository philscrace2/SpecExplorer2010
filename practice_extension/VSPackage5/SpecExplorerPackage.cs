using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.SpecExplorer;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.ActionMachines.Cord;
using Microsoft.Modeling;
using Microsoft.SpecExplorer.Runtime.Testing;
using Microsoft.SpecExplorer.Viewer;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.VSHelp;
using Microsoft.VisualStudio.VSHelp80;
using Microsoft.Xrt;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using VSLangProj80;
using Microsoft.SpecExplorer.VS.Common;
using Microsoft.SpecExplorer.ErrorReporting;
using Microsoft.SpecExplorer.VS;
using Microsoft.SpecExplorer.VS.Package55;


namespace Microsoft.SpecExplorer
{

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [ProvideToolWindow(typeof(StateComparisonView), Height = 480, MultiInstances = false, Orientation = ToolWindowOrientation.Right, Style = VsDockStyle.Tabbed, Transient = true, Width = 640, Window = "{4a9b7e51-aa16-11d0-a8c5-00a0c921a4d2}")]
    [ProvideKeyBindingTable("04C7681D-A337-4705-8AD9-2206D31A9F7B", 508)]
    [ProvideEditorFactory(typeof(EditorFactory), 503, TrustLevel = __VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
    [ProvideToolWindow(typeof(StepBrowserToolWindow), Height = 480, MultiInstances = false, Orientation = ToolWindowOrientation.Right, Style = VsDockStyle.Tabbed, Transient = true, Width = 640, Window = "{4a9b7e51-aa16-11d0-a8c5-00a0c921a4d2}")]
    //[ProvideToolWindow(typeof(WorkflowToolWindow), Orientation = ToolWindowOrientation.none, Style = VsDockStyle.Tabbed, Width = 250, Window = "{B1E99781-AB81-11D0-B683-00AA00A3EE26}")]
    [ProvideToolWindow(typeof(ExplorationManagerToolWindow), Style = VsDockStyle.Tabbed, Window = "{3AE79031-E1BC-11D0-8F78-00A0C9110057}")]
    //[DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\10.0")]
    ////[InstalledProductRegistration(false, "Spec Explorer for Visual Studio 2010 (version 3.5.3146.0)", "Spec Explorer Modeling and Testing Environment, (c) 2009 Microsoft Corporation.", "3.5.3146.0", IconResourceID = 600)]
    //[ProvideLoadKey("Standard", "2.0", "Spec Explorer for VS", "Microsoft", 400)]
    //[ProvideMenuResource(1000, 32)]
    //[ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")]
    //[ProvideToolWindow(typeof(StateBrowserToolWindow), Height = 480, MultiInstances = false, Orientation = ToolWindowOrientation.Right, Style = VsDockStyle.Tabbed, Transient = true, Width = 640, Window = "{4a9b7e51-aa16-11d0-a8c5-00a0c921a4d2}")]
    //[ProvideEditorExtension(typeof(EditorFactory), ".cord", 32)]
    //[ProvideEditorLogicalView(typeof(EditorFactory), "{7651A703-06E5-11D1-8EBD-00A0C90F26EA}", IsTrusted = true)]
    //[ProvideEditorFactory(typeof(ViewDocumentFactory), 509, TrustLevel = __VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
    //[ProvideEditorExtension(typeof(ViewDocumentFactory), ".seexpl", 32)]
    //[ProvideEditorLogicalView(typeof(ViewDocumentFactory), "{00000000-0000-0000-0000-000000000000}", IsTrusted = true)]
    //[ProvideEditorFactory(typeof(SummaryDocumentFactory), 511, TrustLevel = __VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
    //[ProvideEditorExtension(typeof(SummaryDocumentFactory), ".sesum", 32)]
    //[ProvideEditorLogicalView(typeof(SummaryDocumentFactory), "{00000000-0000-0000-0000-000000000000}", IsTrusted = true)]
    [ProvideService(typeof(SGlobalService))]
    //[Guid("f9b9b97b-5213-4c39-b0df-9b44a2b97c58")]
    //[ProvideSolutionProps("SpecExplorer.ActivityCompletionStatus")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidVSPackage5PkgString)]
    public sealed class SpecExplorerPackage : Package, IHost, IDisposable, IVsSolutionEvents,IVsTrackProjectDocumentsEvents2, IVsUpdateSolutionEvents,  IVsPersistSolutionProps, IVsPersistSolutionOpts
    {
        private VerbosityLevel verbosity = (VerbosityLevel)2;
        private bool loggingEnabled = true;
        private IVsUIShell uiShell;
        private TaskCategory currentTaskCategory = TaskCategory.BuildCompile;
        private Dictionary<Tuple<string, string>, string> projectRenameQueries = new Dictionary<Tuple<string, string>, string>();
        private string lastUsedGuidance = string.Empty;
        //nternal EditorFactory editorFactory;
        //private ViewDocumentFactory viewFactory;
        private ISession session;
        private IExtensionManager extensionManager;
        private int errorsSuppressed;
        private DTE dte;
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

        public event EventHandler<SolutionBuildEventArgs> SolutionBuildFinished;

        internal ICordDesignTimeScopeManager CordScopeManager
        {
            get
            {
                return (ICordDesignTimeScopeManager)this.CoreServices.GetRequiredService<ICordDesignTimeScopeManager>();
            }
        }
        

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public SpecExplorerPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            if (this.session == null)
            {
                Microsoft.SpecExplorer.Session session = new Microsoft.SpecExplorer.Session((IHost)this);
                //session.Application.Setup.Add((IComponent)new CordCompletionProvider(this));
                this.session = (ISession)session;
                //((IServiceContainer)this).AddService(typeof(SGlobalService), (object)new GlobalService((ComponentBase)session), true);
                //if (this.SessionInitialized != null)
                //    this.SessionInitialized((object)this, (EventArgs)null);
            }
            //this.InitializeScriptDesignTime();
            //this.InitializeViewDefinitionManager();

            RegisterVSService();
            RegisterHelps();
            RegisterExplorationManagerCommands();
            // Microsoft.SpecExplorer.Session session = new Microsoft.SpecExplorer.Session((IHost)this);


        }
        #endregion

        private void RegisterVSService()
        {
            IServiceContainer serviceContainer = (IServiceContainer)this;
            this.extensionManager = (IExtensionManager)new ExtensionManager();
            serviceContainer.AddService(typeof(IExtensionManager), (object)this.extensionManager, true);
            serviceContainer.AddService(typeof(IHost), (object)this, true);
        }


        private void RegisterHelps()
        {
            OleMenuCommandService service = this.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (service == null)
                return;
            OleMenuCommand oleMenuCommand1 = new OleMenuCommand(new EventHandler(this.OpenSpecExplorerHomePage), new CommandID(GuidList.guidSpecExplorerCmdSet, (int) PkgCmdIDList.cmdidHomePage));
            service.AddCommand(oleMenuCommand1);
            OleMenuCommand oleMenuCommand2 = new OleMenuCommand(new EventHandler(this.OpenSpecExplorerDocumentation), new CommandID(GuidList.guidSpecExplorerCmdSet, (int)PkgCmdIDList.cmdidHelp));
            service.AddCommand((MenuCommand)oleMenuCommand2);
            OleMenuCommand oleMenuCommand3 = new OleMenuCommand(new EventHandler(this.OpenSpecExplorerForum), new CommandID(GuidList.guidSpecExplorerCmdSet, (int)PkgCmdIDList.cmdidForum));
            service.AddCommand((MenuCommand)oleMenuCommand3);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            service = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != service)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidVSPackage5CmdSet, (int)PkgCmdIDList.cmdidMyCommand);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                service.AddCommand(menuItem);
            }

            service = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != service)
            {
                //Create the command for the menu item.
                CommandID menuCommandID1 = new CommandID(GuidList.guidSpecExplorerPackageCmdSet, (int)PkgCmdIDList.cmdidHomePage);
                MenuCommand menuItem = new MenuCommand(OpenSpecExplorerHomePage, menuCommandID1);
                service.AddCommand(menuItem);
            }
        }

        private void RegisterExplorationManagerCommands()
        {
            //OleMenuCommandService service = this.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            //if (service == null)
            //    return;
            //OleMenuCommand oleMenuCommand = new OleMenuCommand(new EventHandler(this.ShowExplorationManagerToolWindow), new CommandID(GuidList.guidSpecExplorerCmdSet, 262));
            //service.AddCommand((MenuCommand)oleMenuCommand);

            OleMenuCommandService service = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != service)
            {
                //Create the command for the menu item.
                CommandID explorertionWindowComamnd = new CommandID(GuidList.guidSpecExplorerPackageCmdSet, (int)PkgCmdIDList.cmdidExplorationManagerToolWindow);
                MenuCommand menuItem = new MenuCommand(ShowExplorationManagerToolWindow, explorertionWindowComamnd);
                service.AddCommand(menuItem);
            }
        }

        private void ShowExplorationManagerToolWindow(object sender, EventArgs e)
        {
            ErrorHandler.ThrowOnFailure(((IVsWindowFrame)this.FindToolWindow<ExplorationManagerToolWindow>().Frame).Show());
        }

        internal T FindToolWindow<T>() where T : ToolWindowPane
        {
            T toolWindow = (T)this.FindToolWindow(typeof(T), 0, true);
            this.Assert((object)toolWindow != null && toolWindow.Frame != null, Resources.CanNotCreateWindow);
            return toolWindow;
        }

        private void OpenSpecExplorerHomePage(object sender, EventArgs e)
        {
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

        public event EventHandler SessionInitialized;

        private void InitializeScriptDesignTime()
        {
            //this.CordScopeManager.BeforeParseScript += new EventHandler<ScriptParseEventArgs>(this.OnBeforeParseScript);
            //this.CordScopeManager.ScopeChanged += new EventHandler<ScopeChangeEventArgs>(this.OnScopeChanged);
        }

        private void InitializeViewDefinitionManager()
        {
            //IViewDefinitionManager requiredService = (IViewDefinitionManager)this.CoreServices.GetRequiredService<IViewDefinitionManager>();
            //if (!this.DTE.Solution.IsOpen || string.IsNullOrEmpty(this.DTE.Solution.FileName))
            //    return;
            //Stream stream = this.OpenViewDefinitionStream(this.GetViewDefinitionFileName());
            //if (stream != null)
            //{
            //    using (stream)
            //    {
            //        try
            //        {
            //            requiredService.Load(stream);
            //        }
            //        catch (ViewDefinitionManagerException ex)
            //        {
            //            this.NotificationDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, string.Format("Error occured while loading view definitions:\n{0}", (object)((Exception)ex).Message));
            //        }
            //    }
            //}
            //else
            //    requiredService.SetDeferredLoading((Func<Stream>)(() =>
            //    {
            //        string definitionFileName = this.GetViewDefinitionFileName();
            //        if (string.IsNullOrEmpty(definitionFileName))
            //            return (Stream)null;
            //        return this.OpenViewDefinitionStream(definitionFileName);
            //    }));
        }

        private void InitializeSession()
        {
            if (this.session == null)
            {
                Microsoft.SpecExplorer.Session session = new Microsoft.SpecExplorer.Session((IHost)this);
                session.Application.Setup.Add((IComponent)new CordCompletionProvider(this));
                this.session = (ISession)session;
                ((IServiceContainer)this).AddService(typeof(SGlobalService), (object)new GlobalService((ComponentBase)session), true);
                if (this.SessionInitialized != null)
                    this.SessionInitialized((object)this, (EventArgs)null);
            }
            this.InitializeScriptDesignTime();
            this.InitializeViewDefinitionManager();
        }

        private class SpecExplorerError : ErrorTask
        {
            //private SpecExplorerPackage package;

            //internal bool IsParsingError { get; private set; }

            //internal SpecExplorerError(
            //  SpecExplorerPackage package,
            //  TaskCategory category,
            //  DiagnosisKind kind,
            //  string fileName,
            //  bool isParsingError,
            //  int line,
            //  int column,
            //  string message)
            //{
            //    if (package == null)
            //        throw new ArgumentNullException(package.ToString());
            //    if (fileName == null)
            //        throw new ArgumentNullException(fileName.ToString());
            //    this.package = package;
            //    this.IsParsingError = isParsingError;
            //    switch ((int)kind)
            //    {
            //        case 0:
            //            this.Priority = TaskPriority.High;
            //            this.ErrorCategory = TaskErrorCategory.Error;
            //            break;
            //        case 1:
            //            this.Priority = TaskPriority.Normal;
            //            this.ErrorCategory = TaskErrorCategory.Warning;
            //            break;
            //        case 2:
            //            this.Priority = TaskPriority.Low;
            //            this.ErrorCategory = TaskErrorCategory.Message;
            //            break;
            //    }
            //    int length1 = fileName.IndexOf("$");
            //    if (length1 >= 0)
            //        fileName = fileName.Substring(0, length1);
            //    int length2 = fileName.LastIndexOf("?");
            //    if (length2 >= 0)
            //        fileName = fileName.Substring(0, length2);
            //    this.Document = fileName;
            //    this.Line = line - 1;
            //    this.Column = column - 1;
            //    this.Text = message;
            //    this.Category = category;
            //}

            protected override void OnNavigate(EventArgs e)
            {
                //string document = this.Document;
                //if (FileNames.HasScriptExtension(document))
                //{
                //    this.package.NavigateTo(document, this.Line, this.Column);
                //}
                //else
                //{
                //    try
                //    {
                //        (this.DTE.ItemOperations.OpenFile(document, (string)null).Selection as TextSelection).GotoLine(this.Line + 1, false);
                //    }
                //    catch
                //    {
                //    }
                //}
            }
        }
        private bool DebugPaneIsAvailable
        {
            get
            {
                return true;
            }
        }

        public void Log(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;
            string pszDescription = line.EndsWith("\n", StringComparison.CurrentCultureIgnoreCase) ? line : line + "\r\n";
            IVsActivityLog service = this.GetService(typeof(SVsActivityLog)) as IVsActivityLog;
            if (service != null)
                this.AssertOk(service.LogEntry(3U, "SpecExplorer", pszDescription));
            if (this.DebugPaneIsAvailable)
            {
                this.DebugPane.OutputString(string.Format("{0}", (object)pszDescription));
                this.DebugPane.ForceItemsToTaskList();
            }
            else
                Console.WriteLine("{0}", (object)line);
        }

        private OutputWindowPane SpecExplorerPane
        {
            get
            {
                if (this.specExplorerPane == null)
                {
                    OutputWindow outputWindow = this.DTE.Windows.Item((object)"{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}").Object as OutputWindow;
                    foreach (OutputWindowPane outputWindowPane in outputWindow.OutputWindowPanes)
                    {
                        if (outputWindowPane.Name == Resources.SpecExplorer)
                        {
                            this.specExplorerPane = outputWindowPane;
                            break;
                        }
                    }
                    if (this.specExplorerPane == null)
                        this.specExplorerPane = outputWindow.OutputWindowPanes.Add(Resources.SpecExplorer);
                }
                return this.specExplorerPane;
            }
        }

        internal DTE DTE
        {
            get
            {
                if (this.dte == null)
                    this.dte = this.GetRequiredService<DTE>(typeof(DTE));
                return this.dte;
            }
        }

        private OutputWindowPane DebugPane
        {
            get
            {
                if (this.debugPane == null)
                {
                    OutputWindow outputWindow = this.DTE.Windows.Item((object)"{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}").Object as OutputWindow;
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

        public void AssertOk(int hr)
        {
            if (ErrorHandler.Succeeded(hr))
                return;
            this.FatalError(string.Format("assertion on COM call failed (hr={0})", (object)hr));
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
                stringBuilder.AppendLine("involved exception: " + (object)exception);
            stringBuilder.AppendLine("===========");
            string str = stringBuilder.ToString();
            this.Log(str);
            new ErrorReportBuilder((IHost)this).GenerateErrorReport(str, ((IEnumerable<Exception>)exceptions).FirstOrDefault<Exception>());
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

        public void RecoverFromFatalError(Exception exception)
        {
            if (exception == null)
                return;
            string pszText;
            if (!(exception is InvalidOperationException) || string.IsNullOrEmpty(exception.Message) || !exception.Message.StartsWith("Spec Explorer VS integration"))
                pszText = "";
            else
                pszText = exception.Message;
            IVsUIShell service = (IVsUIShell)this.GetService(typeof(SVsUIShell));
            Guid empty = Guid.Empty;
            int pnResult;
            this.AssertOk(service.ShowMessageBox(0U, ref empty, "Spec Explorer", pszText, string.Empty, 0U, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_CRITICAL, 0, out pnResult));
        }


        public void ProgressMessage(VerbosityLevel verbosity, string message)
        {
            if (verbosity > this.verbosity)
                return;
            this.SpecExplorerPane.OutputString(message + "\r\n");
            if (verbosity > 0)
                return;
            //this.AssertOk(this.Statusbar.SetText(message));
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
                TextLocation textLocation = (TextLocation)location;
                //fileName = ((TextLocation)textLocation).FileName ?? Microsoft.SpecExplorer.Resources.SpecExplorer;
                line = (int)((TextLocation)textLocation).FirstLine;
                column = (int)((TextLocation)textLocation).FirstColumn;
            }
            //else
            //    fileName = location == null ? Microsoft.SpecExplorer.Resources.SpecExplorer : location.ToString() ?? Microsoft.SpecExplorer.Resources.SpecExplorer;
            this.SpecExplorerPane.OutputString(str + message + "\r\n");
            //this.ErrorList.Tasks.Add((Task)new SpecExplorerPackage.SpecExplorerError(this, this.currentTaskCategory, kind, fileName, isParsingError, line, column, message));
        }



        public VerbosityLevel Verbosity
        {
            get
            {
                return this.verbosity;
            }
        }

        public bool Logging
        {
            get
            {
                return this.loggingEnabled;
            }
        }

        public bool TryGetExtensionData(string key, object inputValue, out object outputValue)
        {
            outputValue = null;
            return true;
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "VSPackage5",
                       string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));
        }

        private Project GetContainingProject(MemberInfo member)
        {
            //Project containingProject = (Project)null;
            //foreach (Project allRealProject in ProjectUtils.GetAllRealProjects(this.DTE))
            //{
            //    if (allRealProject != null && this.IsContainingProject(allRealProject, member, out containingProject))
            //        return containingProject;
            //}
            return (Project)null;
        }

        public bool TryFindLocation(MemberInfo member, out TextLocation location)
        {
            try
            {
                location = new TextLocation();
                Project containingProject = this.GetContainingProject(member);
                if (containingProject == null)
                    return false;
                CodeModel codeModel = containingProject.CodeModel;
                if (codeModel == null)
                    return false;
                CodeType codeType = codeModel.CodeTypeFromFullName(member.TypeName);
                if (member.Kind == null)
                {
                    if (codeType == null)
                        return false;
                    //location = this.MakeLocation(codeType as CodeElement);
                    return true;
                }
                if (member.Kind.Equals(2))
                {
                    //foreach (CodeElement allMember in codeType.GetAllMembers())
                    //{
                    //    if (allMember.Kind == vsCMElement.vsCMElementVariable && allMember == member)
                    //    {
                    //        location = this.MakeLocation(allMember);
                    //        return true;
                    //    }
                    //}
                    return false;
                }
                if (member.Kind.Equals(1))
                {
                    string[] array = member.ParameterTypes.ToArray<string>();
                    //foreach (CodeElement allMember in codeType.GetAllMembers())
                    //{
                    //    if (allMember.Kind == vsCMElement.vsCMElementFunction)
                    //    {
                    //        CodeFunction codeFunction = allMember as CodeFunction;
                    //        if (allMember == member && codeFunction != null)
                    //        {
                    //            int length = array.Length;
                    //            int num = 0;
                    //            if (length == codeFunction.Parameters.Count)
                    //            {
                    //                bool flag = false;
                    //                foreach (CodeParameter parameter in codeFunction.Parameters)
                    //                {
                    //                    string str = array[num++];
                    //                    if (parameter.Type.TypeKind != vsCMTypeRef.vsCMTypeRefArray && parameter.Type.AsFullName != str || parameter.Type.TypeKind == vsCMTypeRef.vsCMTypeRefArray && !str.Contains(parameter.Type.ElementType.AsFullName))
                    //                    {
                    //                        flag = true;
                    //                        break;
                    //                    }
                    //                }
                    //                if (!flag)
                    //                {
                    //                    location = this.MakeLocation(allMember);
                    //                    return true;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    return false;
                }
                if (member.Kind.Equals(3))
                    return false;
                //foreach (CodeElement allMember in codeType.GetAllMembers())
                //{
                //    if (allMember.Kind == vsCMElement.vsCMElementProperty && allMember == member)
                //    {
                //        location = this.MakeLocation(allMember);
                //        return true;
                //    }
                //}
                return false;
            }
            catch (Exception ex)
            {
                this.DiagMessage((DiagnosisKind)1, string.Format("A Spec Explorer/Visual Studio integration error occurred: \r\n{0}.\r\n Please report this error to support team.", (object)ex.ToString()), (object)null);
                location = new TextLocation();
                return false;
            }
        }

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
            VsShellUtilities.OpenDocument((System.IServiceProvider)this, fileName, logicalView, out hierarchy, out itemID, out windowFrame, out view);
            object pvar;
            ErrorHandler.ThrowOnFailure(windowFrame.GetProperty(-4004, out pvar));
            IVsTextBuffer buffer = pvar as IVsTextBuffer;
            //this.Assert(buffer != null, "Unable to get document buffer");
            IVsTextManager mgr = this.GetService(typeof(VsTextManagerClass)) as IVsTextManager;
            new System.Threading.Thread((ThreadStart)(() =>
            {
                System.Threading.Thread.Sleep(200);
                this.AssertOk(mgr.NavigateToLineAndColumn(buffer, ref logicalView, line, column, line, column));
            })).Start();
        }

        public void NotificationDialog(string title, string message)
        {
            this.NotificationDialog(title, message, OLEMSGICON.OLEMSGICON_INFO);
        }

        private void NotificationDialog(string title, string message, OLEMSGICON icon)
        {
            IVsUIShell service = (IVsUIShell)this.GetService(typeof(SVsUIShell));
            Guid empty = Guid.Empty;
            int pnResult;
            this.AssertOk(service.ShowMessageBox(0U, ref empty, title, message, string.Empty, 0U, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, icon, 0, out pnResult));
        }

        public MessageResult DecisionDialog(
          string title,
          string message,
          MessageButton messageButton)
        {
            IVsUIShell service = (IVsUIShell)this.GetService(typeof(SVsUIShell));
            Guid empty = Guid.Empty;
            int pnResult = 0;
            //this.AssertOk(service.ShowMessageBox(0U, ref empty, title, message, string.Empty, 0U, messageButton.ToOleMessageButton(), OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_NOICON, 0, out pnResult));
            return (MessageResult)pnResult;
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
                    return (IWin32Window)new Win32Window(phwnd);
                return (IWin32Window)null;
            }
        }

        internal IVsUIShell UIShell
        {
            get
            {
                if (this.uiShell == null)
                    this.uiShell = this.GetRequiredService<IVsUIShell>(typeof(SVsUIShell));
                return this.uiShell;
            }
        }

        internal S GetRequiredService<S>(System.Type type) where S : class
        {
            S s1 = default(S);
            if (this.CoreServices != null)
            {
                S service = this.CoreServices.GetService<S>();
                if ((object)service != null)
                    return service;
            }
            object service1 = this.GetService(type);
            if (service1 == null)
            {
                this.FatalError(string.Format("cannot find required service {0}", (object)type.FullName));
                return default(S);
            }
            S s2 = service1 as S;
            if ((object)s2 != null)
                return s2;
            this.FatalError(string.Format("cannot get interface {0} of service {1}", (object)typeof(S).FullName, (object)type.FullName));
            return default(S);
        }

        public ComponentBase CoreServices
        {
            get
            {
                return this.session as ComponentBase;
            }
        }
        public ISession Session
        {
            get
            {
                return this.session;
            }
        }

        //public ISession Session { get; set; }
        public ErrorListProvider ErrorList { get; set; }
        public IVsSolutionBuildManager2 SolutionBuildManager { get; set; }

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

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            IServiceContainer serviceContainer = (IServiceContainer)this;
            serviceContainer.RemoveService(typeof(IExtensionManager), true);
            serviceContainer.RemoveService(typeof(IHost), true);
            this.DisposeSession();
            if (this.errorList != null)
            {
                this.errorList.Dispose();
                this.errorList = (ErrorListProvider)null;
            }
            base.Dispose(disposing);
            this.disposed = true;
        }

        private void DisposeSession()
        {
            ((IServiceContainer)this).RemoveService(typeof(SGlobalService));
            (this.session as IDisposable).Dispose();
            this.session = (ISession)null;
            if (this.SessionDisposed == null)
                return;
            this.SessionDisposed((object)this, (EventArgs)null);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        public event EventHandler SessionDisposed;

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            //try
            //{
            //    if (this.editorFactory != null)
            //        this.editorFactory.Clear();
            //    this.errorsSuppressed = 0;
            //    if (this.CoreServices != null)
            //        ((IViewDefinitionManager)this.CoreServices.GetRequiredService<IViewDefinitionManager>()).Reset();
            //    this.FindToolWindow<ExplorationManagerToolWindow>().SwitchView(false);
            //    this.FindToolWindow<WorkflowToolWindow>().UnloadWindowContent();
            //    this.ActivityCompletionStatus = string.Empty;
            //    this.UnregisterAllProjectsFromCordScopeManager();
            //    this.ClearErrorList();
            //    this.SpecExplorerPane.Clear();
            //}
            //finally
            //{
            //    this.DisposeSession();
            //}
            return 0;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            //if (this.Session != null)
            //{
            //    Project dteProject = this.ToDteProject(pRealHierarchy);
            //    if (dteProject != null)
            //    {
            //        this.CheckAndConvertProject(dteProject);
            //        this.RegisterProjectToCordScopeManager(dteProject);
            //    }
            //}
            return 0;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            //if (this.Session != null)
            //{
            //    Project dteProject = this.ToDteProject(pHierarchy);
            //    if (dteProject != null)
            //    {
            //        this.CheckAndConvertProject(dteProject);
            //        this.RegisterProjectToCordScopeManager(dteProject);
            //    }
            //}
            return 0;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            //this.InitializeSession();
            //SpecExplorerPackage.ReferenceUpgradeHelper referenceUpgradeHelper = new SpecExplorerPackage.ReferenceUpgradeHelper();
            //foreach (Project allRealProject in ProjectUtils.GetAllRealProjects(this.DTE))
            //{
            //    if (allRealProject != null)
            //    {
            //        this.CheckAndConvertProject(allRealProject);
            //        referenceUpgradeHelper.Handle(allRealProject);
            //    }
            //}
            //if (referenceUpgradeHelper.NeedUpgrade && MessageResult.YES == this.DecisionDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, string.Format("{0} finds the projects that you are opening contain references created by previous version of {0}, which might cause unexpected behaviors. Would you like to upgrade it to the latest version?", (object)Microsoft.SpecExplorer.Resources.SpecExplorer), MessageButton.YESNOCANCEL))
            //    this.NotificationDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, "Project references created by previous version of have been upgraded successfully");
            //foreach (Project allRealProject in ProjectUtils.GetAllRealProjects(this.DTE))
            //{
            //    if (allRealProject != null)
            //        this.RegisterProjectToCordScopeManager(allRealProject);
            //}
            //this.FindToolWindow<ExplorationManagerToolWindow>().SwitchView(true);
            //if (this.WasWorkflowLoadedFlag || this.FindToolWindow<WorkflowToolWindow>().IsWindowVisible)
            //    this.LoadWorkflowToolWindow();
            return 0;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            //Project dteProject = this.ToDteProject(pHierarchy);
            //if (dteProject != null)
            //    this.UnregisterProjectFromCordScopeManager(dteProject);
            return 0;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return 0;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            //Project dteProject = this.ToDteProject(pRealHierarchy);
            //if (dteProject != null)
            //    this.UnregisterProjectFromCordScopeManager(dteProject);
            return 0;
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

        public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags,
            VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            throw new NotImplementedException();
        }

        public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices,
            string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
        {
            throw new NotImplementedException();
        }

        public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices,
            string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
        {
            throw new NotImplementedException();
        }

        public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices,
            string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            throw new NotImplementedException();
        }

        public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices,
            string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            throw new NotImplementedException();
        }

        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames,
            VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
        {
            throw new NotImplementedException();
        }

        public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices,
            string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            throw new NotImplementedException();
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames,
            VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult,
            VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            throw new NotImplementedException();
        }

        public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices,
            string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            throw new NotImplementedException();
        }

        public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments,
            VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult,
            VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            throw new NotImplementedException();
        }

        public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags,
            VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            throw new NotImplementedException();
        }

        public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments,
            VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult,
            VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            throw new NotImplementedException();
        }

        public int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices,
            string[] rgpszMkDocuments, uint[] rgdwSccStatus)
        {
            throw new NotImplementedException();
        }

        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            throw new NotImplementedException();
        }

        public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            throw new NotImplementedException();
        }

        public int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            throw new NotImplementedException();
        }

        public int UpdateSolution_Cancel()
        {
            throw new NotImplementedException();
        }

        public int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            throw new NotImplementedException();
        }

        public int SaveUserOptions(IVsSolutionPersistence pPersistence)
        {
            throw new NotImplementedException();
        }

        public int LoadUserOptions(IVsSolutionPersistence pPersistence, uint grfLoadOpts)
        {
            throw new NotImplementedException();
        }

        public int WriteUserOptions(IStream pOptionsStream, string pszKey)
        {
            throw new NotImplementedException();
        }

        public int ReadUserOptions(IStream pOptionsStream, string pszKey)
        {
            throw new NotImplementedException();
        }

        public int QuerySaveSolutionProps(IVsHierarchy pHierarchy, VSQUERYSAVESLNPROPS[] pqsspSave)
        {
            throw new NotImplementedException();
        }

        public int SaveSolutionProps(IVsHierarchy pHierarchy, IVsSolutionPersistence pPersistence)
        {
            throw new NotImplementedException();
        }

        public int WriteSolutionProps(IVsHierarchy pHierarchy, string pszKey, IPropertyBag pPropBag)
        {
            throw new NotImplementedException();
        }

        public int ReadSolutionProps(IVsHierarchy pHierarchy, string pszProjectName, string pszProjectMk, string pszKey, int fPreLoad,
            IPropertyBag pPropBag)
        {
            throw new NotImplementedException();
        }

        public int OnProjectLoadFailure(IVsHierarchy pStubHierarchy, string pszProjectName, string pszProjectMk, string pszKey)
        {
            throw new NotImplementedException();
        }

        public void CollectScripts(List<string> scripts, ProjectItems projectProjectItems)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> CollectReferences(Project project)
        {
            throw new NotImplementedException();
        }

        public bool BuildProject(Project project)
        {
            throw new NotImplementedException();
        }

        public void ClearErrorList()
        {
            throw new NotImplementedException();
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

        public ICordDesignTimeManager GetDesignTimeForProject(Project projectUnitProject)
        {
            throw new NotImplementedException();
        }

        public void MakeErrorListVisible()
        {
            throw new NotImplementedException();
        }

        public bool ValidateAllScripts()
        {
            throw new NotImplementedException();
        }

        public IVsHierarchy ToHierarchy(Project project)
        {
            throw new NotImplementedException();
        }

        public Project GetProjectByUniqueName(string containerProjectName)
        {
            throw new NotImplementedException();
        }

        public ICordDesignTimeManager GetDesignTimeForCordDocument(CordDocument cordDocument)
        {
            throw new NotImplementedException();
        }

        public void UnregisterCordDocumentFromDesignTimeManager(CordDocument doc)
        {
            throw new NotImplementedException();
        }

        public void RegisterCordDocumentToDesignTimeManager(CordDocument doc, bool b)
        {
            throw new NotImplementedException();
        }
    }
}
