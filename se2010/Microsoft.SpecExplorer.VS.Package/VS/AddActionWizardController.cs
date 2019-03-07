// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.AddActionWizardController
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using EnvDTE80;
using Microsoft.ActionMachines.Cord;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Microsoft.SpecExplorer.VS
{
  internal class AddActionWizardController
  {
    private Stack<AddActionWizardController.WizardState> stateStack;
    private AssistedProcedureWizardWindow wizardWindow;
    private GenericSelectionControl configSelection;
    private GenericSelectionControl configCreation;
    private GenericSelectionControl scriptCreation;
    private CodeElementViewer codeElementViewer;
    private SpecExplorerPackage package;
    public ItemNameInputValidator ValidateConfigNameInput;
    public ItemNameInputValidator ValidateScriptNameInput;

    private AddActionWizardController.WizardState CurrentState
    {
      get
      {
        return this.stateStack.Peek();
      }
    }

    public AddActionWizardData WizardData { get; private set; }

    public AddActionWizardController(SpecExplorerPackage package)
    {
      this.package = package;
    }

    public bool LaunchWizard()
    {
      this.InitializeControls();
      this.InitializeStateVariables();
      this.LoadConfigs();
      this.LoadScripts();
      this.LoadProjects();
      this.UpdateUserControl();
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

    private void InitializeControls()
    {
      this.wizardWindow = new AssistedProcedureWizardWindow("Modeling Guidance: Import Actions");
      this.configSelection = new GenericSelectionControl(false, "Configurations");
      this.configCreation = new GenericSelectionControl(true, "Scripts");
      this.scriptCreation = new GenericSelectionControl(true, "Projects");
      this.codeElementViewer = new CodeElementViewer();
      this.codeElementViewer.ViewerModel.CodeElementDisplayTextFabricator = new Func<CodeElementItem, string>(this.FabricateCodeElementDisplayText);
      this.codeElementViewer.ViewerModel.CodeElementContentValidator = new Func<CodeElement, bool>(this.ValidateCodeElementContent);
      IntPtr phwnd;
      this.package.AssertOk(this.package.UIShell.GetDialogOwnerHwnd(out phwnd));
      new WindowInteropHelper((System.Windows.Window) this.wizardWindow).Owner = phwnd;
      this.configCreation.TextInputLabel = "New Config Name";
      this.scriptCreation.TextInputLabel = "New Script Name";
      this.wizardWindow.NextPageRequestedEvent += (EventHandler) delegate
      {
        if (!this.TryMovingToNextState() || this.CurrentState != AddActionWizardController.WizardState.Finished)
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
      this.WizardData = new AddActionWizardData();
      this.stateStack = new Stack<AddActionWizardController.WizardState>();
      this.stateStack.Push(AddActionWizardController.WizardState.InConfigSelection);
    }

    private void LoadConfigs()
    {
      this.configSelection.ItemList.Clear();
      foreach (string allScope in (IEnumerable<string>) this.package.CordScopeManager.AllScopes)
      {
        using (IEnumerator<Config> enumerator = ((IEnumerable<Config>) this.package.CordScopeManager.GetCordDesignTimeManager(allScope).AllConfigurations).GetEnumerator())
        {
          while (((IEnumerator) enumerator).MoveNext())
          {
            Config current = enumerator.Current;
            this.configSelection.ItemList.Add((ICordSyntaxElementInfo) new ConfigInfo(allScope, ((SyntaxElement) current).Location().get_FileName(), (string) current.Name));
          }
        }
      }
      this.configSelection.ItemList.Add((ICordSyntaxElementInfo) new ConfigInfo());
    }

    private void LoadScripts()
    {
      this.configCreation.ItemList.Clear();
      foreach (string allScope in (IEnumerable<string>) this.package.CordScopeManager.AllScopes)
      {
        foreach (string managedScript in (IEnumerable<string>) this.package.CordScopeManager.GetCordDesignTimeManager(allScope).ManagedScripts)
          this.configCreation.ItemList.Add((ICordSyntaxElementInfo) new ScriptInfo(allScope, managedScript));
      }
      this.configCreation.ItemList.Add((ICordSyntaxElementInfo) new ScriptInfo());
    }

    private void LoadProjects()
    {
      this.scriptCreation.ItemList.Clear();
      foreach (string allScope in (IEnumerable<string>) this.package.CordScopeManager.AllScopes())
        this.scriptCreation.ItemList.Add((ICordSyntaxElementInfo) new ProjectInfo(allScope));
    }

    private void LoadCodeElements()
    {
      Project projectByUniqueName = this.package.GetProjectByUniqueName(this.WizardData.ProjectName);
      if (projectByUniqueName == null)
        return;
      this.codeElementViewer.ViewerModel.LoadCodeElements((IEnumerable) projectByUniqueName.CodeModel.CodeElements, true, CodeElementExpandOptions.ExpandAll, true);
    }

    private string FabricateCodeElementDisplayText(CodeElementItem codeElement)
    {
      string str1 = codeElement.GetPrototype(false);
      if ((codeElement.Kind == CodeElementItemType.Event || codeElement.Kind == CodeElementItemType.Function) && !this.WizardData.IsScriptToBeCreated)
      {
        CoordinationScript scriptSyntax = this.package.CordScopeManager.GetCordDesignTimeManager(this.WizardData.ProjectName).GetScriptSyntax(this.WizardData.ScriptName);
        if (scriptSyntax != null)
        {
          foreach (string str2 in ((IEnumerable<Namespace>) ((IEnumerable<Namespace>) scriptSyntax.GlobalNamespaces).OrderByDescending<Namespace, int>((Func<Namespace, int>) (nameSp => ((InstantiatedName) nameSp.Name).Flatten().Length))).Select<Namespace, string>((Func<Namespace, string>) (nameSp => ((InstantiatedName) nameSp.Name).Flatten())))
            str1 = str1.Replace(str2 + ".", string.Empty);
        }
      }
      return str1;
    }

    private bool ValidateCodeElementContent(CodeElement codeElement)
    {
      switch (codeElement.Kind)
      {
        case vsCMElement.vsCMElementClass:
          return !codeElement.FullName.StartsWith("System.");
        case vsCMElement.vsCMElementFunction:
          return CordSyntaxElementBuilder.IsFunctionValid(codeElement as CodeFunction);
        case vsCMElement.vsCMElementNamespace:
          CodeNamespace codeNamespace = codeElement as CodeNamespace;
          if (codeNamespace != null)
            return codeNamespace.IsValid();
          return false;
        case vsCMElement.vsCMElementInterface:
          return true;
        case vsCMElement.vsCMElementEvent:
          return CordSyntaxElementBuilder.IsEventValid(codeElement as CodeEvent);
        default:
          return false;
      }
    }

    private bool TryMovingToNextState()
    {
      switch (this.CurrentState)
      {
        case AddActionWizardController.WizardState.InConfigSelection:
          ConfigInfo selectedItem1 = (ConfigInfo) this.configSelection.SelectedItem;
          if (string.IsNullOrEmpty(selectedItem1.ConfigName))
          {
            this.stateStack.Push(AddActionWizardController.WizardState.InScriptSelection);
          }
          else
          {
            this.WizardData.ConfigName = selectedItem1.ConfigName;
            this.WizardData.IsConfigToBeCreated = false;
            this.WizardData.ScriptName = selectedItem1.ContainerScript;
            this.WizardData.IsScriptToBeCreated = false;
            this.WizardData.ProjectName = selectedItem1.ContainerProject;
            this.stateStack.Push(AddActionWizardController.WizardState.InCodeElementSelection);
          }
          this.UpdateUserControl();
          break;
        case AddActionWizardController.WizardState.InScriptSelection:
          this.WizardData.ConfigName = this.configCreation.TextInputValue;
          this.WizardData.IsConfigToBeCreated = true;
          ScriptInfo selectedItem2 = (ScriptInfo) this.configCreation.SelectedItem;
          string message = this.ValidateConfigNameInput(this.WizardData.ConfigName, selectedItem2.ContainerProject);
          if (!string.IsNullOrEmpty(message))
          {
            this.package.NotificationDialog(Resources.SpecExplorer, message);
            return false;
          }
          if (string.IsNullOrEmpty(selectedItem2.ScriptName))
          {
            this.stateStack.Push(AddActionWizardController.WizardState.InProjectSelection);
          }
          else
          {
            this.WizardData.ScriptName = selectedItem2.ScriptName;
            this.WizardData.IsScriptToBeCreated = false;
            this.WizardData.ProjectName = selectedItem2.ContainerProject;
            this.stateStack.Push(AddActionWizardController.WizardState.InCodeElementSelection);
          }
          this.UpdateUserControl();
          break;
        case AddActionWizardController.WizardState.InProjectSelection:
          if (this.scriptCreation.SelectedItem == null)
          {
            this.package.NotificationDialog(Resources.SpecExplorer, "No project has been opened.");
            return false;
          }
          this.WizardData.ScriptName = this.scriptCreation.TextInputValue;
          this.WizardData.IsScriptToBeCreated = true;
          this.WizardData.ProjectName = (this.scriptCreation.SelectedItem as ProjectInfo).ProjectName;
          if (!this.WizardData.ProjectName.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
          {
            this.package.NotificationDialog(Resources.SpecExplorer, "Cord script can only be added into a C# project.");
            return false;
          }
          string str1 = this.ValidateScriptNameInput(this.WizardData.ScriptName, this.WizardData.ProjectName);
          string str2 = this.ValidateConfigNameInput(this.WizardData.ConfigName, this.WizardData.ProjectName);
          if (!string.IsNullOrEmpty(str1) || !string.IsNullOrEmpty(str2))
          {
            this.package.NotificationDialog(Resources.SpecExplorer, str1 + "\n" + str2);
            return false;
          }
          this.stateStack.Push(AddActionWizardController.WizardState.InCodeElementSelection);
          this.UpdateUserControl();
          break;
        case AddActionWizardController.WizardState.InCodeElementSelection:
          this.WizardData.CodeElementsToBeImported = (IEnumerable<CodeElementAndContainerPair>) this.codeElementViewer.ViewerModel.RetrieveSelectedItems(vsCMElement.vsCMElementFunction, vsCMElement.vsCMElementEvent);
          if (this.WizardData.CodeElementsToBeImported.Count<CodeElementAndContainerPair>() == 0)
          {
            this.package.NotificationDialog(Resources.SpecExplorer, "At least one method or event is required to be selected");
            return false;
          }
          this.stateStack.Push(AddActionWizardController.WizardState.Finished);
          break;
        case AddActionWizardController.WizardState.Finished:
        case AddActionWizardController.WizardState.Cancelled:
          return false;
      }
      return true;
    }

    private bool TryMovingToPreviousState()
    {
      switch (this.CurrentState)
      {
        case AddActionWizardController.WizardState.InConfigSelection:
        case AddActionWizardController.WizardState.Finished:
        case AddActionWizardController.WizardState.Cancelled:
          return false;
        default:
          int num = (int) this.stateStack.Pop();
          this.UpdateUserControl();
          return true;
      }
    }

    private void UpdateUserControl()
    {
      this.wizardWindow.WarningTextVisible = Visibility.Collapsed;
      switch (this.CurrentState)
      {
        case AddActionWizardController.WizardState.InConfigSelection:
          this.wizardWindow.IsStartState = true;
          this.wizardWindow.IsFinalState = false;
          this.wizardWindow.BannerText = "Select a Cord Config where imported model action declarations will be added.";
          this.wizardWindow.BannerHeader = "Configuration Selection";
          this.wizardWindow.WanringText = "Please make sure the projects from which actions are imported have been opened.";
          this.wizardWindow.WarningTextVisible = Visibility.Visible;
          this.wizardWindow.LoadUserControl((UserControl) this.configSelection);
          break;
        case AddActionWizardController.WizardState.InScriptSelection:
          this.wizardWindow.IsStartState = false;
          this.wizardWindow.IsFinalState = false;
          this.wizardWindow.BannerText = "Input a new config name and select a script that is to contain the new config.";
          this.wizardWindow.BannerHeader = "New Configuration Creation";
          this.wizardWindow.LoadUserControl((UserControl) this.configCreation);
          break;
        case AddActionWizardController.WizardState.InProjectSelection:
          this.wizardWindow.IsStartState = false;
          this.wizardWindow.IsFinalState = false;
          this.wizardWindow.BannerText = "Input a new script name and select a C# project that is to contain the new script.";
          this.wizardWindow.BannerHeader = "New Script Creation";
          this.wizardWindow.LoadUserControl((UserControl) this.scriptCreation);
          break;
        case AddActionWizardController.WizardState.InCodeElementSelection:
          this.wizardWindow.IsStartState = false;
          this.wizardWindow.IsFinalState = true;
          this.wizardWindow.BannerText = "Select implementation methods/events to add as model action declarations.";
          this.wizardWindow.BannerHeader = "Existing Method/Event Selection";
          this.LoadCodeElements();
          this.wizardWindow.LoadUserControl((UserControl) this.codeElementViewer);
          break;
        default:
          throw new InvalidOperationException("Not supposed to have any other state");
      }
    }

    private enum WizardState
    {
      InConfigSelection,
      InScriptSelection,
      InProjectSelection,
      InCodeElementSelection,
      Finished,
      Cancelled,
    }
  }
}
