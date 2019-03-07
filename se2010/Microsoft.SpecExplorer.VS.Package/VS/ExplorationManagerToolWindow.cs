// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ExplorationManagerToolWindow
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using Microsoft.ActionMachines.Cord;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Xrt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Microsoft.SpecExplorer.VS
{
  [Guid("6DC56C89-A22C-44a8-B43F-58AB60F25121")]
  public class ExplorationManagerToolWindow : ToolWindowPane
  {
    private const int TimerPeriod = 1000;
    private ElementHost elementHost;
    private ExplorationManagerControl control;
    private SpecExplorerPackage package;
    private Microsoft.VisualStudio.Shell.SelectionContainer selectionContainer;
    private ITrackSelection trackSelection;
    private System.Threading.Thread workerThread;
    private ExplorationManagerToolWindow.OperationStatus status;
    private Dictionary<string, System.Type> postProcessorTypeMap;
    private Dictionary<string, string> postProcessorDisplayNameMap;
    private IVsFileChangeEx vsFileChangeEx;
    private IVsWindowFrame propertyWindowFrame;
    private uint pdwCookie;
    private bool isValidating;

    public ExplorationManagerToolWindow()
      : base((System.IServiceProvider) null)
    {
      this.Caption = Microsoft.SpecExplorer.Resources.ExplorationManagerToolWindowTitle;
      this.BitmapResourceID = 602;
      this.BitmapIndex = 0;
    }

    protected override void Initialize()
    {
      base.Initialize();
      this.elementHost = new ElementHost();
    }

    public override void OnToolWindowCreated()
    {
      base.OnToolWindowCreated();
      this.package = this.Package as SpecExplorerPackage;
      this.InitializePostProcessors();
      this.control = new ExplorationManagerControl((Func<ComponentBase>) (() => this.package.CoreServices), this.package.DTE.Solution.IsOpen, (IDictionary<string, string>) this.postProcessorDisplayNameMap);
      this.elementHost.Child = (UIElement) this.control;
      this.RegisterEventHandlers();
      this.InitializePropertyWindow();
      this.SetUserContext();
      this.vsFileChangeEx = (IVsFileChangeEx) this.GetService(typeof (SVsFileChangeEx));
    }

    private IVsWindowFrame PropertyWindowFrame
    {
      get
      {
        if (this.propertyWindowFrame == null)
        {
          Guid rguidPersistenceSlot = new Guid("{EEFA5220-E298-11D0-8F78-00A0C9110057}");
          IVsWindowFrame ppWindowFrame;
          if (ErrorHandler.Succeeded(this.package.UIShell.FindToolWindow(524288U, ref rguidPersistenceSlot, out ppWindowFrame)) && ppWindowFrame != null)
            this.propertyWindowFrame = ppWindowFrame;
        }
        return this.propertyWindowFrame;
      }
    }

    private void InitializePropertyWindow()
    {
      this.selectionContainer = new Microsoft.VisualStudio.Shell.SelectionContainer();
      this.trackSelection = this.GetService(typeof (STrackSelection)) as ITrackSelection;
      this.package.Assert(this.trackSelection != null);
      this.selectionContainer.SelectableObjects = (ICollection) null;
      this.selectionContainer.SelectedObjects = (ICollection) null;
      this.package.AssertOk(this.trackSelection.OnSelectChange((ISelectionContainer) this.selectionContainer));
    }

    private void RegisterEventHandlers()
    {
      this.control.add_Validate(new EventHandler(this.OnValidate));
      this.package.SolutionBuildFinished += new EventHandler<SolutionBuildEventArgs>(this.OnSolutionBuildFinished);
      this.control.add_AbortOperation(new EventHandler(this.OnAbortOperating));
      this.control.add_NavigateToMachine(new EventHandler<MachineEventArgs>(this.OnNavigateToMachine));
      this.control.add_Explore(new EventHandler<MachineEventArgs>(this.OnExplore));
      this.control.add_OnTheFlyTest(new EventHandler<MachineEventArgs>(this.OnOnTheFly));
      this.control.add_OnTheFlyTestReplay(new EventHandler<MachineEventArgs>(this.OnOnTheFlyReplay));
      this.control.add_GenerateTestCode(new EventHandler<MachineEventArgs>(this.OnGenerateTestCode));
      this.control.add_ExecutePostProcessing(new EventHandler<MachineEventArgs>(this.OnExcutePostProcessing));
      this.control.add_ShowProperties(new EventHandler<MachineEventArgs>(this.OnShowProperties));
      this.control.add_ChangeSelectedMachine(new EventHandler<MachineEventArgs>(this.OnChangeSelectedMachine));
    }

    private void InitializePostProcessors()
    {
      if (PostProcessorHelper.LoadCustomizedPostProcessingTypes(Path.Combine(this.package.Session.InstallDir, Microsoft.SpecExplorer.Resources.ExtensionDirectoryName), (IHost) this.package, ref this.postProcessorTypeMap, ref this.postProcessorDisplayNameMap))
        return;
      this.postProcessorTypeMap = new Dictionary<string, System.Type>();
      this.postProcessorDisplayNameMap = new Dictionary<string, string>();
    }

    private void SetUserContext()
    {
      object pvar;
      ErrorHandler.ThrowOnFailure(((IVsWindowFrame) this.Frame).GetProperty(-3010, out pvar));
      ErrorHandler.ThrowOnFailure(((IVsUserContext) pvar).AddAttribute(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1, "keyword", "microsoft.specexplorer.explorationmanager"));
    }

    private void OnChangeSelectedMachine(object sender, MachineEventArgs e)
    {
      IVsWindowFrame propertyWindowFrame = this.PropertyWindowFrame;
      if (propertyWindowFrame == null || propertyWindowFrame.IsVisible() != 0)
        return;
      this.TrackSelectedMachines(e.Machines);
    }

    private void OnShowProperties(object sender, MachineEventArgs e)
    {
      IVsWindowFrame propertyWindowFrame = this.PropertyWindowFrame;
      if (propertyWindowFrame == null)
        return;
      this.package.AssertOk(propertyWindowFrame.Show());
      this.TrackSelectedMachines(e.Machines);
    }

    private void TrackSelectedMachines(IList<Machine> machines)
    {
      if (((ICollection<Machine>) machines).Count == 1)
      {
        MachinePropertyTypeDescriptor propertyOfMachine = this.GetPropertyOfMachine(machines[0]);
        if (propertyOfMachine == null)
          return;
        Microsoft.VisualStudio.Shell.SelectionContainer selectionContainer = this.selectionContainer;
        List<MachinePropertyTypeDescriptor> propertyTypeDescriptorList1 = new List<MachinePropertyTypeDescriptor>();
        propertyTypeDescriptorList1.Add(propertyOfMachine);
        List<MachinePropertyTypeDescriptor> propertyTypeDescriptorList2 = propertyTypeDescriptorList1;
        selectionContainer.SelectedObjects = (ICollection) propertyTypeDescriptorList2;
        this.package.AssertOk(this.trackSelection.OnSelectChange((ISelectionContainer) this.selectionContainer));
      }
      else
      {
        this.selectionContainer.SelectedObjects = (ICollection) null;
        this.package.AssertOk(this.trackSelection.OnSelectChange((ISelectionContainer) this.selectionContainer));
      }
    }

    private MachinePropertyTypeDescriptor GetPropertyOfMachine(
      Machine machine)
    {
      Project project = ProjectUtils.GetAllRealProjects(this.package.DTE).FirstOrDefault<Project>((Func<Project, bool>) (p => p.UniqueName == machine.Container.UniqueName));
      if (project != null)
      {
        ICordDesignTimeManager designTimeManager = this.package.CordScopeManager.GetCordDesignTimeManager(project.UniqueName);
        this.package.Assert(designTimeManager != null);
        if (designTimeManager != null)
        {
          IDictionary<string, string> machineSwitches = designTimeManager.GetMachineSwitches(machine.Name);
          List<MachinePropertyDescriptor> propertyDescriptorList = new List<MachinePropertyDescriptor>();
          IOptionSetManager requiredService = (IOptionSetManager) this.package.CoreServices.GetRequiredService<IOptionSetManager>();
          foreach (PropertyDescriptor property in requiredService.GetProperties((Visibility) 2))
          {
            string str;
            if (!machineSwitches.TryGetValue(property.Name.ToLower(), out str))
            {
              object obj;
              str = !requiredService.TryGetDefaultValue(property, out obj) || obj == null ? string.Empty : obj.ToString();
            }
            else if (str == null)
              str = string.Empty;
            propertyDescriptorList.Add(new MachinePropertyDescriptor(property, (object) str));
          }
          return new MachinePropertyTypeDescriptor(machine.Name, new PropertyDescriptorCollection((PropertyDescriptor[]) propertyDescriptorList.ToArray()));
        }
      }
      return (MachinePropertyTypeDescriptor) null;
    }

    private void OnValidate(object sender, EventArgs e)
    {
      this.control.StartOperation();
      this.control.ProgressMessage(Microsoft.SpecExplorer.Resources.ValidationInProgress);
      this.package.ProgressMessage((VerbosityLevel) 0, Microsoft.SpecExplorer.Resources.ValidationInProgress);
      this.package.ClearErrorList();
      this.package.DTE.Documents.SaveAll();
      IEnumerable<Project> containingCordScript = ProjectUtils.GetProjectsContainingCordScript(this.package.DTE, (ICordDesignTimeScopeManager) this.package.CoreServices.GetRequiredService<ICordDesignTimeScopeManager>());
      int num = containingCordScript.Count<Project>();
      if (num > 0)
      {
        this.isValidating = true;
        IVsSolutionBuildManager2 solutionBuildManager = this.package.SolutionBuildManager;
        this.package.AssertOk(solutionBuildManager.AdviseUpdateSolutionEvents((IVsUpdateSolutionEvents) this.package, out this.pdwCookie));
        if (ErrorHandler.Succeeded(solutionBuildManager.StartUpdateProjectConfigurations((uint) num, containingCordScript.Select<Project, IVsHierarchy>((Func<Project, IVsHierarchy>) (p => this.package.ToHierarchy(p))).ToArray<IVsHierarchy>(), 65536U, 1)))
          return;
        this.package.AssertOk(solutionBuildManager.UnadviseUpdateSolutionEvents(this.pdwCookie));
        this.isValidating = false;
        this.package.DiagMessage((DiagnosisKind) 0, Microsoft.SpecExplorer.Resources.FailedToBuildProject, (object) null);
        this.package.ProgressMessage((VerbosityLevel) 0, Microsoft.SpecExplorer.Resources.ValidationFailed);
        this.control.ProgressMessage(Microsoft.SpecExplorer.Resources.ValidationFailed);
        this.package.MakeErrorListVisible();
        this.control.FinishOperation();
      }
      else
      {
        this.package.ProgressMessage((VerbosityLevel) 0, Microsoft.SpecExplorer.Resources.ValidationSucceeded);
        this.control.ProgressMessage(Microsoft.SpecExplorer.Resources.ValidationSucceeded);
        this.control.FinishOperation();
      }
    }

    private void OnSolutionBuildFinished(object sender, SolutionBuildEventArgs e)
    {
      if (!this.isValidating)
        return;
      if (!e.IsCanceled && e.IsSucceeded && this.package.ValidateAllScripts())
      {
        this.package.ProgressMessage((VerbosityLevel) 0, Microsoft.SpecExplorer.Resources.ValidationSucceeded);
        this.control.ProgressMessage(Microsoft.SpecExplorer.Resources.ValidationSucceeded);
      }
      else
      {
        this.package.ProgressMessage((VerbosityLevel) 0, Microsoft.SpecExplorer.Resources.ValidationFailed);
        this.control.ProgressMessage(Microsoft.SpecExplorer.Resources.ValidationFailed);
        this.package.MakeErrorListVisible();
      }
      this.isValidating = false;
      IVsSolutionBuildManager2 service = this.GetService(typeof (SVsSolutionBuildManager)) as IVsSolutionBuildManager2;
      this.package.Assert(service != null, "Failed to retrieve solution build manager service.");
      this.package.AssertOk(service.UnadviseUpdateSolutionEvents(this.pdwCookie));
      this.pdwCookie = 0U;
      this.control.FinishOperation();
    }

    private void OnAbortOperating(object sender, EventArgs e)
    {
      this.AbortOperationAsync();
    }

    private void AbortOperationAsync()
    {
      if (this.workerThread == null)
        return;
      if (this.status == ExplorationManagerToolWindow.OperationStatus.None)
        return;
      try
      {
        this.workerThread.Abort();
      }
      catch (ThreadStateException ex)
      {
        this.package.FatalError(ex.Message, (Exception) ex);
      }
    }

    internal void AbortOperation()
    {
      this.AbortOperationAsync();
      if (this.workerThread == null || this.workerThread.Join(1000))
        return;
      this.package.NotificationDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, Microsoft.SpecExplorer.Resources.AbortingExploration);
      this.workerThread.Join();
    }

    private void OnNavigateToMachine(object sender, MachineEventArgs e)
    {
      Machine machine = e.Machines[0];
      Project project = ProjectUtils.GetAllRealProjects(this.package.DTE).FirstOrDefault<Project>((Func<Project, bool>) (p => p.UniqueName == machine.Container.UniqueName));
      if (project != null)
      {
        ICordDesignTimeManager designTimeManager = this.package.CordScopeManager.GetCordDesignTimeManager(project.UniqueName);
        if (designTimeManager != null)
        {
          MachineDefinition machineDefinition = ((IEnumerable<MachineDefinition>) designTimeManager.AllMachines).FirstOrDefault<MachineDefinition>((Func<MachineDefinition, bool>) (m => (string) m.Name == machine.Name));
          if (machineDefinition != null)
          {
            int line = ((SyntaxElement) machineDefinition).Location.StartLine > 0 ? ((SyntaxElement) machineDefinition).Location.StartLine - 1 : ((SyntaxElement) machineDefinition).Location.StartLine;
            int column = ((SyntaxElement) machineDefinition).Location.StartColumn > 0 ? ((SyntaxElement) machineDefinition).Location.StartColumn - 1 : ((SyntaxElement) machineDefinition).Location.StartColumn;
            this.package.NavigateTo(((SyntaxElement) machineDefinition).Location.FileName, line, column);
            return;
          }
        }
      }
      this.package.NotificationDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, string.Format("Cannot navigate to machine '{0}'.", (object) machine.Name));
    }

    private void OnExplore(object sender, MachineEventArgs e)
    {
      this.Work(TaskTypes.Exploring, e, (string) null);
    }

    private void OnExcutePostProcessing(object sender, MachineEventArgs e)
    {
      this.Work(TaskTypes.RunningPostProcessors, e, (string) null);
    }

    private void OnGenerateTestCode(object sender, MachineEventArgs e)
    {
      this.Work(TaskTypes.GeneratingTestCode, e, (string) null);
    }

    private void OnOnTheFly(object sender, MachineEventArgs e)
    {
      this.Work(TaskTypes.OnTheFlyTesting, e, (string) null);
    }

    private void OnOnTheFlyReplay(object sender, MachineEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
      openFileDialog.Multiselect = false;
      openFileDialog.Filter = string.Format("Exploration result files (*{0})|*{0}", (object) ".seexpl");
      if (this.package.DTE != null && this.package.DTE.Solution != null && !string.IsNullOrEmpty(this.package.DTE.Solution.FullName))
        openFileDialog.InitialDirectory = Path.GetDirectoryName(this.package.DTE.Solution.FullName);
      bool? nullable = openFileDialog.ShowDialog();
      if (nullable.HasValue && nullable.Value)
        this.Work(TaskTypes.OnTheFlyReplayTest, e, openFileDialog.FileName);
      else
        this.control.FinishOperation();
    }

    private void Work(TaskTypes taskType, MachineEventArgs e, string replayFilePath = null)
    {
      this.workerThread = new System.Threading.Thread((ThreadStart) (() =>
      {
        this.status = ExplorationManagerToolWindow.OperationStatus.Running;
        this.control.StartOperation();
        ExplorationWorker explorationWorker = new ExplorationWorker(this.package, this.control, e.Machines, e.ReExplore, this.vsFileChangeEx, taskType, e.PostProcessors, replayFilePath);
        try
        {
          explorationWorker.Execute();
        }
        catch (ThreadAbortException ex)
        {
          System.Threading.Thread.ResetAbort();
          this.control.ProgressMessage(Microsoft.SpecExplorer.Resources.AbortingExploration);
          this.status = ExplorationManagerToolWindow.OperationStatus.Aborting;
        }
        explorationWorker.Cleanup(this.status == ExplorationManagerToolWindow.OperationStatus.Aborting);
        this.control.FinishOperation();
        this.status = ExplorationManagerToolWindow.OperationStatus.None;
      }));
      this.workerThread.Start();
    }

    internal void SwitchView(bool availableView)
    {
      if (this.control == null)
        return;
      this.AbortOperationAsync();
      this.control.UpdateView(availableView);
    }

    internal void UpdateMachineList()
    {
      if (this.control == null)
        return;
      this.control.UpdateMachines();
    }

    public override IWin32Window Window
    {
      get
      {
        return (IWin32Window) this.elementHost;
      }
    }

    private enum OperationStatus
    {
      None,
      Running,
      Aborting,
    }
  }
}
