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
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidVSPackage5PkgString)]
    public sealed class VSPackage5Package : Package, IHost
    {
        private VerbosityLevel verbosity = (VerbosityLevel)2;
        private bool loggingEnabled = true;
        private IVsUIShell uiShell;
        private TaskCategory currentTaskCategory = TaskCategory.BuildCompile;
        private Dictionary<Tuple<string, string>, string> projectRenameQueries = new Dictionary<Tuple<string, string>, string>();
        private string lastUsedGuidance = string.Empty;
        //internal EditorFactory editorFactory;
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
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public VSPackage5Package()
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
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidVSPackage5CmdSet, (int)PkgCmdIDList.cmdidMyCommand);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );
            }

            Microsoft.SpecExplorer.Session session = new Microsoft.SpecExplorer.Session((IHost)this);

            //if (this.session == null)
            //{
            //    Microsoft.SpecExplorer.Session session = new Microsoft.SpecExplorer.Session((IHost)this);
            //    //session.Application.Setup.Add((IComponent)new CordCompletionProvider(this));
            //    this.session = (ISession)session;
            //    ((IServiceContainer)this).AddService(typeof(SGlobalService), (object)new GlobalService((ComponentBase)session), true);
            //    if (this.SessionInitialized != null)
            //        this.SessionInitialized((object)this, (EventArgs)null);
            //}
            //this.InitializeScriptDesignTime();
            //this.InitializeViewDefinitionManager();
        }
        #endregion 

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
                switch ((int)kind)
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
                        if (outputWindowPane.Name == "")
                        {
                            this.specExplorerPane = outputWindowPane;
                            break;
                        }
                    }
                    if (this.specExplorerPane == null)
                        this.specExplorerPane = outputWindow.OutputWindowPanes.Add("");
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

    }
}
