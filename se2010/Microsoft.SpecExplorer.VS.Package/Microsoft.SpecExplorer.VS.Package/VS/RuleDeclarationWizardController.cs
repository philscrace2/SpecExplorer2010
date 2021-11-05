using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using EnvDTE;
using Microsoft.ActionMachines.Cord;

namespace Microsoft.SpecExplorer.VS
{
	internal class RuleDeclarationWizardController
	{
		private enum WizardState
		{
			InConfigSelection,
			InActionsSelection,
			InHostClassSelection,
			InTypeBinding,
			Finished
		}

		private AssistedProcedureWizardWindow wizardWindow;

		private ActionSelectionControl actionSelection;

		private TypeMapControl typeMapSelection;

		private CodeElementViewer hostClassViewer;

		private GenericSelectionControl configSelection;

		private Stack<WizardState> stateStack;

		private ProcedureType[] typesNeedingHostClass;

		private int currentITypeHostIndex;

		private HashSet<ProcedureType> typesNeedingTypeBinding;

		private bool isFurtherTypeBindingRequired;

		private SpecExplorerPackage package;

		public ActionConfigClauseResolver ResolveActionClause;

		public SourceBindingTypeProvider GetSourceBindingTypes;

		private WizardState CurrentState
		{
			get
			{
				return stateStack.Peek();
			}
		}

		public RuleDeclarationWizardData WizardData { get; private set; }

		public RuleDeclarationWizardController(SpecExplorerPackage package)
		{
			this.package = package;
		}

		public bool LaunchWizard()
		{
			InitializeControls();
			InitializeStateVariables();
			LoadConfigs();
			UpdateUserControl(true);
			bool? flag = null;
			try
			{
				int hr = package.UIShell.EnableModeless(0);
				package.AssertOk(hr);
				flag = wizardWindow.ShowDialog();
			}
			finally
			{
				int hr2 = package.UIShell.EnableModeless(1);
				package.AssertOk(hr2);
			}
			if (flag.HasValue)
			{
				return flag.Value;
			}
			return false;
		}

		private bool TryMovingToNextState()
		{
			bool refreshControl = true;
			switch (CurrentState)
			{
			case WizardState.Finished:
				return false;
			case WizardState.InConfigSelection:
			{
				if (configSelection.SelectedItem == null)
				{
					package.NotificationDialog(Resources.SpecExplorer, "Select a configuration having action clause.");
					return false;
				}
				Project projectByUniqueName = package.GetProjectByUniqueName((configSelection.SelectedItem as ConfigInfo).ContainerProject);
				package.Assert(projectByUniqueName != null, "Unexpected Error: unable to locate container project");
				if (!package.ProjectHasCSharpClass(projectByUniqueName))
				{
					package.NotificationDialog(Resources.SpecExplorer, "Project containing the selected configuration does not have any CSharp class to host rule method stub.");
					return false;
				}
				refreshControl = WizardData.ConfigInfo == null || !WizardData.ConfigInfo.Equals(configSelection.SelectedItem);
				WizardData.ConfigInfo = (ConfigInfo)configSelection.SelectedItem;
				stateStack.Push(WizardState.InActionsSelection);
				break;
			}
			case WizardState.InActionsSelection:
			{
				IEnumerable<ConfigClause> selectedActions = actionSelection.ControlModel.SelectedActions;
				if (selectedActions.Count() == 0)
				{
					package.NotificationDialog(Resources.SpecExplorer, "At least one action is to be selected");
					return false;
				}
				Dictionary<ProcedureType, HashSet<MethodDescriptor>> methodDescriptors;
				HashSet<ProcedureType> adapterTypes;
				ResolveActionClause(selectedActions, out methodDescriptors, out adapterTypes);
				Dictionary<ProcedureType, CodeClass> existTypeBinding;
				typesNeedingTypeBinding = GetSourceBindingTypes(methodDescriptors, GetCodeElementsFromProjectFiles(), out existTypeBinding);
				foreach (KeyValuePair<ProcedureType, CodeClass> item in existTypeBinding)
				{
					WizardData.TypeBindingMap[item.Key] = item.Value;
				}
				typesNeedingHostClass = methodDescriptors.Keys.ToArray();
				isFurtherTypeBindingRequired = typesNeedingHostClass.Intersect(typesNeedingTypeBinding).Count() < typesNeedingTypeBinding.Count();
				WizardData.AdapterTypes = adapterTypes;
				WizardData.MethodDescriptors = methodDescriptors;
				currentITypeHostIndex = 0;
				stateStack.Push(WizardState.InHostClassSelection);
				break;
			}
			case WizardState.InHostClassSelection:
			{
				CodeElementAndContainerPair codeElementAndContainerPair = hostClassViewer.ViewerModel.RetrieveSelectedItems((vsCMElement)1).SingleOrDefault();
				if (codeElementAndContainerPair == null || !(codeElementAndContainerPair.Element is CodeClass))
				{
					package.NotificationDialog(Resources.SpecExplorer, "One host class is required to be selected");
					return false;
				}
				CodeElement element = codeElementAndContainerPair.Element;
				CodeClass value = (CodeClass)(object)((element is CodeClass) ? element : null);
				WizardData.HostClassMap[typesNeedingHostClass[currentITypeHostIndex++]] = value;
				if (currentITypeHostIndex < typesNeedingHostClass.Length)
				{
					stateStack.Push(WizardState.InHostClassSelection);
					break;
				}
				foreach (ProcedureType item2 in typesNeedingTypeBinding)
				{
					CodeClass value2;
					WizardData.TypeBindingMap[item2] = (WizardData.HostClassMap.TryGetValue(item2, out value2) ? value2 : null);
				}
				stateStack.Push(isFurtherTypeBindingRequired ? WizardState.InTypeBinding : WizardState.Finished);
				break;
			}
			case WizardState.InTypeBinding:
				foreach (TypeMapUnit item3 in typeMapSelection.TypeMap)
				{
					WizardData.TypeBindingMap[item3.ImplementationType] = item3.ModelClass;
				}
				stateStack.Push(WizardState.Finished);
				break;
			}
			UpdateUserControl(refreshControl);
			return true;
		}

		private bool TryMovingToPreviousState()
		{
			switch (CurrentState)
			{
			case WizardState.InConfigSelection:
			case WizardState.Finished:
				return false;
			case WizardState.InHostClassSelection:
			case WizardState.InTypeBinding:
				currentITypeHostIndex--;
				break;
			}
			stateStack.Pop();
			UpdateUserControl(false);
			return true;
		}

		private void UpdateUserControl(bool refreshControl)
		{
			switch (CurrentState)
			{
			case WizardState.InConfigSelection:
				wizardWindow.IsStartState = true;
				wizardWindow.IsFinalState = false;
				wizardWindow.BannerHeader = "Select Config";
				wizardWindow.BannerText = "Select the cord config containing the action declarations to create rule methods for";
				wizardWindow.LoadUserControl(configSelection);
				break;
			case WizardState.InActionsSelection:
				wizardWindow.IsStartState = false;
				wizardWindow.IsFinalState = false;
				wizardWindow.BannerHeader = "Select Actions";
				wizardWindow.BannerText = "Select actions for which rule methods will be declared";
				LoadActions(refreshControl);
				wizardWindow.LoadUserControl(actionSelection);
				break;
			case WizardState.InHostClassSelection:
				wizardWindow.IsStartState = false;
				wizardWindow.IsFinalState = !isFurtherTypeBindingRequired && currentITypeHostIndex == typesNeedingHostClass.Length - 1;
				wizardWindow.BannerHeader = "Select Host Class for Rule Methods";
				wizardWindow.BannerText = "Select host class for rule methods to model actions of type: " + typesNeedingHostClass[currentITypeHostIndex].ShortName;
				LoadCodeElementsFromProjectFiles();
				wizardWindow.LoadUserControl(hostClassViewer);
				break;
			case WizardState.InTypeBinding:
				wizardWindow.IsStartState = false;
				wizardWindow.IsFinalState = true;
				wizardWindow.BannerHeader = "Bind Types";
				wizardWindow.BannerText = "Select a model type to map each of the following implementation parameter types";
				LoadTypeBindingRequirements();
				wizardWindow.LoadUserControl(typeMapSelection);
				break;
			case WizardState.Finished:
				break;
			}
		}

		private void InitializeControls()
		{
			wizardWindow = new AssistedProcedureWizardWindow("Modeling Guidance: Declare Rules");
			IntPtr owner = default(IntPtr);
			int dialogOwnerHwnd = package.UIShell.GetDialogOwnerHwnd(out owner);
			package.AssertOk(dialogOwnerHwnd);
			WindowInteropHelper windowInteropHelper = new WindowInteropHelper(wizardWindow);
			windowInteropHelper.Owner = owner;
			actionSelection = new ActionSelectionControl();
			typeMapSelection = new TypeMapControl
			{
				ContainerWindow = wizardWindow,
				Package = package
			};
			configSelection = new GenericSelectionControl(false, "Configurations");
			hostClassViewer = new CodeElementViewer();
			hostClassViewer.ViewerModel.CodeElementDisplayTextFabricator = FabricateCodeElementDisplayText;
			wizardWindow.NextPageRequestedEvent += delegate
			{
				if (TryMovingToNextState() && CurrentState == WizardState.Finished)
				{
					wizardWindow.DialogResult = true;
				}
			};
			wizardWindow.PreviousPageRequestedEvent += delegate
			{
				TryMovingToPreviousState();
			};
		}

		private void InitializeStateVariables()
		{
			WizardData = new RuleDeclarationWizardData();
			stateStack = new Stack<WizardState>();
			stateStack.Push(WizardState.InConfigSelection);
		}

		private void LoadConfigs()
		{
			configSelection.ItemList.Clear();
			foreach (string allScope in package.CordScopeManager.AllScopes)
			{
				ICordDesignTimeManager cordDesignTimeManager = package.CordScopeManager.GetCordDesignTimeManager(allScope);
				foreach (Config allConfiguration in cordDesignTimeManager.AllConfigurations)
				{
					if (allConfiguration.Clauses.Any((ConfigClause cl) => cl is ConfigClause.ImportMethod || cl is ConfigClause.ImportAllFromScope || cl is ConfigClause.DeclareMethod))
					{
						configSelection.ItemList.Add(new ConfigInfo(allScope, allConfiguration.Location.FileName, allConfiguration.Name));
					}
				}
			}
		}

		private void LoadActions(bool configChanged)
		{
			if (configChanged)
			{
				ICordDesignTimeManager cordDesignTimeManager = package.CordScopeManager.GetCordDesignTimeManager(WizardData.ConfigInfo.ContainerProject);
				actionSelection.ControlModel.LoadActions(cordDesignTimeManager.AllConfigurations.FirstOrDefault((Config confName) => confName.Name == WizardData.ConfigInfo.ConfigName).Clauses);
			}
		}

		private void LoadCodeElementsFromProjectFiles()
		{
			hostClassViewer.ViewerModel.LoadCodeElements(GetCodeElementsFromProjectFiles(), false, CodeElementExpandOptions.ExpandToNamespaces | CodeElementExpandOptions.ExpandToClasses, false);
		}

		private IEnumerable<CodeElement> GetCodeElementsFromProjectFiles()
		{
			Project project = package.GetProjectByUniqueName(WizardData.ConfigInfo.ContainerProject);
			package.Assert(project != null, "Unexpected Error: Unable to locate container project");
			foreach (ProjectItem projItem in package.GetAuthoredCSharpDocuments(project.ProjectItems))
			{
				foreach (CodeElement codeElement in projItem.FileCodeModel.CodeElements)
				{
					yield return codeElement;
				}
			}
		}

		private void LoadTypeBindingRequirements()
		{
			Project projectByUniqueName = package.GetProjectByUniqueName(WizardData.ConfigInfo.ContainerProject);
			if (projectByUniqueName != null)
			{
				IEnumerable<ProcedureType> types = from typeMap in WizardData.TypeBindingMap
					where typeMap.Value == null
					select typeMap.Key;
				typeMapSelection.LoadImplementationTypes(types, GetCodeElementsFromProjectFiles());
			}
		}

		private string FabricateCodeElementDisplayText(CodeElementItem codeElement)
		{
			string prototype = codeElement.GetPrototype(false);
			if (codeElement.RootElement != null)
			{
				ProjectItem projectItem = codeElement.RootElement.ProjectItem;
				package.Assert(projectItem.ContainingProject != null, "Unexpected Error: Unable to locate container project");
				string text = package.ComputePathRelativeToProject(projectItem.ContainingProject, projectItem);
				package.Assert(!string.IsNullOrEmpty(text), "Unexpected Error: Unable to compute relative file path");
				return string.Format("{0} [{1}]", prototype, text);
			}
			return prototype;
		}
	}
}
