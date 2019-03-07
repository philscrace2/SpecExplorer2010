// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.RuleDeclarationWizardController
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using Microsoft.ActionMachines.Cord;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Microsoft.SpecExplorer.VS
{
  internal class RuleDeclarationWizardController
  {
    private AssistedProcedureWizardWindow wizardWindow;
    private ActionSelectionControl actionSelection;
    private TypeMapControl typeMapSelection;
    private CodeElementViewer hostClassViewer;
    private GenericSelectionControl configSelection;
    private Stack<RuleDeclarationWizardController.WizardState> stateStack;
    private ProcedureType[] typesNeedingHostClass;
    private int currentITypeHostIndex;
    private HashSet<ProcedureType> typesNeedingTypeBinding;
    private bool isFurtherTypeBindingRequired;
    private SpecExplorerPackage package;
    public ActionConfigClauseResolver ResolveActionClause;
    public SourceBindingTypeProvider GetSourceBindingTypes;

    private RuleDeclarationWizardController.WizardState CurrentState
    {
      get
      {
        return this.stateStack.Peek();
      }
    }

    public RuleDeclarationWizardController(SpecExplorerPackage package)
    {
      this.package = package;
    }

    public RuleDeclarationWizardData WizardData { get; private set; }

    public bool LaunchWizard()
    {
      this.InitializeControls();
      this.InitializeStateVariables();
      this.LoadConfigs();
      this.UpdateUserControl(true);
      bool? nullable = new bool?();
      try
      {
        this.package.AssertOk(this.package.UIShell.EnableModeless(0));
        nullable = this.wizardWindow.ShowDialog();
      }
      finally
      {
        this.package.AssertOk(this.package.UIShell.EnableModeless(1));
      }
      if (nullable.HasValue)
        return nullable.Value;
      return false;
    }

    private bool TryMovingToNextState()
    {
      bool refreshControl = true;
      switch (this.CurrentState)
      {
        case RuleDeclarationWizardController.WizardState.InConfigSelection:
          if (this.configSelection.SelectedItem == null)
          {
            this.package.NotificationDialog(Resources.SpecExplorer, "Select a configuration having action clause.");
            return false;
          }
          Project projectByUniqueName = this.package.GetProjectByUniqueName((this.configSelection.SelectedItem as ConfigInfo).ContainerProject);
          this.package.Assert(projectByUniqueName != null, "Unexpected Error: unable to locate container project");
          if (!this.package.ProjectHasCSharpClass(projectByUniqueName))
          {
            this.package.NotificationDialog(Resources.SpecExplorer, "Project containing the selected configuration does not have any CSharp class to host rule method stub.");
            return false;
          }
          refreshControl = (this.WizardData.ConfigInfo != null ? (this.WizardData.ConfigInfo.Equals((object) this.configSelection.SelectedItem) ? 1 : 0) : 0) == 0;
          this.WizardData.ConfigInfo = (ConfigInfo) this.configSelection.SelectedItem;
          this.stateStack.Push(RuleDeclarationWizardController.WizardState.InActionsSelection);
          break;
        case RuleDeclarationWizardController.WizardState.InActionsSelection:
          IEnumerable<ConfigClause> selectedActions = this.actionSelection.ControlModel.SelectedActions;
          if (selectedActions.Count<ConfigClause>() == 0)
          {
            this.package.NotificationDialog(Resources.SpecExplorer, "At least one action is to be selected");
            return false;
          }
          Dictionary<ProcedureType, HashSet<MethodDescriptor>> methodDescriptors;
          HashSet<ProcedureType> adapterTypes;
          int num = this.ResolveActionClause(selectedActions, out methodDescriptors, out adapterTypes) ? 1 : 0;
          Dictionary<ProcedureType, CodeClass> existTypeBinding;
          this.typesNeedingTypeBinding = this.GetSourceBindingTypes(methodDescriptors, this.GetCodeElementsFromProjectFiles(), out existTypeBinding);
          foreach (KeyValuePair<ProcedureType, CodeClass> keyValuePair in existTypeBinding)
            this.WizardData.TypeBindingMap[keyValuePair.Key] = keyValuePair.Value;
          this.typesNeedingHostClass = ((IEnumerable<ProcedureType>) methodDescriptors.Keys).ToArray<ProcedureType>();
          this.isFurtherTypeBindingRequired = ((IEnumerable<ProcedureType>) this.typesNeedingHostClass).Intersect<ProcedureType>((IEnumerable<ProcedureType>) this.typesNeedingTypeBinding).Count<ProcedureType>() < this.typesNeedingTypeBinding.Count<ProcedureType>();
          this.WizardData.AdapterTypes = adapterTypes;
          this.WizardData.MethodDescriptors = methodDescriptors;
          this.currentITypeHostIndex = 0;
          this.stateStack.Push(RuleDeclarationWizardController.WizardState.InHostClassSelection);
          break;
        case RuleDeclarationWizardController.WizardState.InHostClassSelection:
          CodeElementAndContainerPair andContainerPair = this.hostClassViewer.ViewerModel.RetrieveSelectedItems(vsCMElement.vsCMElementClass).SingleOrDefault<CodeElementAndContainerPair>();
          if (andContainerPair == null || !(andContainerPair.Element is CodeClass))
          {
            this.package.NotificationDialog(Resources.SpecExplorer, "One host class is required to be selected");
            return false;
          }
          this.WizardData.HostClassMap[this.typesNeedingHostClass[this.currentITypeHostIndex++]] = andContainerPair.Element as CodeClass;
          if (this.currentITypeHostIndex < this.typesNeedingHostClass.Length)
          {
            this.stateStack.Push(RuleDeclarationWizardController.WizardState.InHostClassSelection);
            break;
          }
          foreach (ProcedureType key in this.typesNeedingTypeBinding)
          {
            CodeClass codeClass;
            this.WizardData.TypeBindingMap[key] = this.WizardData.HostClassMap.TryGetValue(key, out codeClass) ? codeClass : (CodeClass) null;
          }
          this.stateStack.Push(this.isFurtherTypeBindingRequired ? RuleDeclarationWizardController.WizardState.InTypeBinding : RuleDeclarationWizardController.WizardState.Finished);
          break;
        case RuleDeclarationWizardController.WizardState.InTypeBinding:
          foreach (TypeMapUnit type in (Collection<TypeMapUnit>) this.typeMapSelection.TypeMap)
            this.WizardData.TypeBindingMap[type.ImplementationType] = type.ModelClass;
          this.stateStack.Push(RuleDeclarationWizardController.WizardState.Finished);
          break;
        case RuleDeclarationWizardController.WizardState.Finished:
          return false;
      }
      this.UpdateUserControl(refreshControl);
      return true;
    }

    private bool TryMovingToPreviousState()
    {
      switch (this.CurrentState)
      {
        case RuleDeclarationWizardController.WizardState.InConfigSelection:
        case RuleDeclarationWizardController.WizardState.Finished:
          return false;
        case RuleDeclarationWizardController.WizardState.InHostClassSelection:
        case RuleDeclarationWizardController.WizardState.InTypeBinding:
          --this.currentITypeHostIndex;
          break;
      }
      int num = (int) this.stateStack.Pop();
      this.UpdateUserControl(false);
      return true;
    }

    private void UpdateUserControl(bool refreshControl)
    {
      switch (this.CurrentState)
      {
        case RuleDeclarationWizardController.WizardState.InConfigSelection:
          this.wizardWindow.IsStartState = true;
          this.wizardWindow.IsFinalState = false;
          this.wizardWindow.BannerHeader = "Select Config";
          this.wizardWindow.BannerText = "Select the cord config containing the action declarations to create rule methods for";
          this.wizardWindow.LoadUserControl((UserControl) this.configSelection);
          break;
        case RuleDeclarationWizardController.WizardState.InActionsSelection:
          this.wizardWindow.IsStartState = false;
          this.wizardWindow.IsFinalState = false;
          this.wizardWindow.BannerHeader = "Select Actions";
          this.wizardWindow.BannerText = "Select actions for which rule methods will be declared";
          this.LoadActions(refreshControl);
          this.wizardWindow.LoadUserControl((UserControl) this.actionSelection);
          break;
        case RuleDeclarationWizardController.WizardState.InHostClassSelection:
          this.wizardWindow.IsStartState = false;
          this.wizardWindow.IsFinalState = !this.isFurtherTypeBindingRequired && this.currentITypeHostIndex == this.typesNeedingHostClass.Length - 1;
          this.wizardWindow.BannerHeader = "Select Host Class for Rule Methods";
          this.wizardWindow.BannerText = "Select host class for rule methods to model actions of type: " + this.typesNeedingHostClass[this.currentITypeHostIndex].ShortName;
          this.LoadCodeElementsFromProjectFiles();
          this.wizardWindow.LoadUserControl((UserControl) this.hostClassViewer);
          break;
        case RuleDeclarationWizardController.WizardState.InTypeBinding:
          this.wizardWindow.IsStartState = false;
          this.wizardWindow.IsFinalState = true;
          this.wizardWindow.BannerHeader = "Bind Types";
          this.wizardWindow.BannerText = "Select a model type to map each of the following implementation parameter types";
          this.LoadTypeBindingRequirements();
          this.wizardWindow.LoadUserControl((UserControl) this.typeMapSelection);
          break;
      }
    }

    private void InitializeControls()
    {
      this.wizardWindow = new AssistedProcedureWizardWindow("Modeling Guidance: Declare Rules");
      IntPtr phwnd;
      this.package.AssertOk(this.package.UIShell.GetDialogOwnerHwnd(out phwnd));
      new WindowInteropHelper((System.Windows.Window) this.wizardWindow).Owner = phwnd;
      this.actionSelection = new ActionSelectionControl();
      this.typeMapSelection = new TypeMapControl()
      {
        ContainerWindow = (System.Windows.Window) this.wizardWindow,
        Package = this.package
      };
      this.configSelection = new GenericSelectionControl(false, "Configurations");
      this.hostClassViewer = new CodeElementViewer();
      this.hostClassViewer.ViewerModel.CodeElementDisplayTextFabricator = new Func<CodeElementItem, string>(this.FabricateCodeElementDisplayText);
      this.wizardWindow.NextPageRequestedEvent += (EventHandler) delegate
      {
        if (!this.TryMovingToNextState() || this.CurrentState != RuleDeclarationWizardController.WizardState.Finished)
          return;
        this.wizardWindow.DialogResult = new bool?(true);
      };
      this.wizardWindow.PreviousPageRequestedEvent += (EventHandler) delegate
      {
        this.TryMovingToPreviousState();
      };
    }

    private void InitializeStateVariables()
    {
      this.WizardData = new RuleDeclarationWizardData();
      this.stateStack = new Stack<RuleDeclarationWizardController.WizardState>();
      this.stateStack.Push(RuleDeclarationWizardController.WizardState.InConfigSelection);
    }

    private void LoadConfigs()
    {
      this.configSelection.ItemList.Clear();
      foreach (string allScope in (IEnumerable<string>) this.package.CordScopeManager.get_AllScopes())
      {
        using (IEnumerator<Config> enumerator = ((IEnumerable<Config>) this.package.CordScopeManager.GetCordDesignTimeManager(allScope).get_AllConfigurations()).GetEnumerator())
        {
          while (((IEnumerator) enumerator).MoveNext())
          {
            Config current = enumerator.Current;
            if (((IEnumerable<ConfigClause>) current.Clauses).Any<ConfigClause>((Func<ConfigClause, bool>) (cl =>
            {
              if (!(cl is ConfigClause.ImportMethod) && !(cl is ConfigClause.ImportAllFromScope))
                return cl is ConfigClause.DeclareMethod;
              return true;
            })))
              this.configSelection.ItemList.Add((ICordSyntaxElementInfo) new ConfigInfo(allScope, ((SyntaxElement) current).get_Location().get_FileName(), (string) current.Name));
          }
        }
      }
    }

    private void LoadActions(bool configChanged)
    {
      if (!configChanged)
        return;
      this.actionSelection.ControlModel.LoadActions((IEnumerable<ConfigClause>) ((IEnumerable<Config>) this.package.CordScopeManager.GetCordDesignTimeManager(this.WizardData.ConfigInfo.ContainerProject).get_AllConfigurations()).FirstOrDefault<Config>((Func<Config, bool>) (confName => (string) confName.Name == this.WizardData.ConfigInfo.ConfigName)).Clauses);
    }

    private void LoadCodeElementsFromProjectFiles()
    {
      this.hostClassViewer.ViewerModel.LoadCodeElements((IEnumerable) this.GetCodeElementsFromProjectFiles(), false, CodeElementExpandOptions.ExpandToNamespaces | CodeElementExpandOptions.ExpandToClasses, false);
    }

    private IEnumerable<CodeElement> GetCodeElementsFromProjectFiles()
    {
      Project project = this.package.GetProjectByUniqueName(this.WizardData.ConfigInfo.ContainerProject);
      this.package.Assert(project != null, "Unexpected Error: Unable to locate container project");
      foreach (ProjectItem authoredCsharpDocument in this.package.GetAuthoredCSharpDocuments(project.ProjectItems))
      {
        IEnumerator enumerator = authoredCsharpDocument.FileCodeModel.CodeElements.GetEnumerator();
        try
        {
          while (enumerator.MoveNext())
          {
            CodeElement elem = (CodeElement) enumerator.Current;
            yield return elem;
          }
        }
        finally
        {
          IDisposable disposable = enumerator as IDisposable;
          disposable?.Dispose();
        }
      }
    }

    private void LoadTypeBindingRequirements()
    {
      if (this.package.GetProjectByUniqueName(this.WizardData.ConfigInfo.ContainerProject) == null)
        return;
      this.typeMapSelection.LoadImplementationTypes(this.WizardData.TypeBindingMap.Where<KeyValuePair<ProcedureType, CodeClass>>((Func<KeyValuePair<ProcedureType, CodeClass>, bool>) (typeMap => typeMap.Value == null)).Select<KeyValuePair<ProcedureType, CodeClass>, ProcedureType>((Func<KeyValuePair<ProcedureType, CodeClass>, ProcedureType>) (typeMap => typeMap.Key)), (IEnumerable) this.GetCodeElementsFromProjectFiles());
    }

    private string FabricateCodeElementDisplayText(CodeElementItem codeElement)
    {
      string prototype = codeElement.GetPrototype(false);
      if (codeElement.RootElement == null)
        return prototype;
      ProjectItem projectItem = codeElement.RootElement.ProjectItem;
      this.package.Assert(projectItem.ContainingProject != null, "Unexpected Error: Unable to locate container project");
      string relativeToProject = this.package.ComputePathRelativeToProject(projectItem.ContainingProject, projectItem);
      this.package.Assert(!string.IsNullOrEmpty(relativeToProject), "Unexpected Error: Unable to compute relative file path");
      return string.Format("{0} [{1}]", (object) prototype, (object) relativeToProject);
    }

    private enum WizardState
    {
      InConfigSelection,
      InActionsSelection,
      InHostClassSelection,
      InTypeBinding,
      Finished,
    }
  }
}
