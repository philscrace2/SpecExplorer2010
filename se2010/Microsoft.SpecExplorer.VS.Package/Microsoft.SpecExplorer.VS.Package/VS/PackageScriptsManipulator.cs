using System.Collections.Generic;
using System.Linq;
using Microsoft.ActionMachines.Cord;

namespace Microsoft.SpecExplorer.VS
{
	internal class PackageScriptsManipulator
	{
		private SpecExplorerPackage package;

		private CordSyntaxElementBuilder syntaxElementBuilder;

		public ScriptManipulationReport Report { get; private set; }

		public PackageScriptsManipulator(SpecExplorerPackage package)
		{
			this.package = package;
		}

		public bool CommitChanges(AddActionWizardData wizardData)
		{
			Report = new ScriptManipulationReport();
			string text = (wizardData.IsScriptToBeCreated ? package.CreateScriptAndAddToProject(wizardData.ProjectName, wizardData.ScriptName) : wizardData.ScriptName);
			if (text == null)
			{
				return false;
			}
			syntaxElementBuilder = new CordSyntaxElementBuilder(package, wizardData.ProjectName, text);
			ICordDesignTimeManager cordDesignTimeManager = package.CordScopeManager.GetCordDesignTimeManager(wizardData.ProjectName);
			CoordinationScript scriptSyntax = cordDesignTimeManager.GetScriptSyntax(text);
			Config config = (wizardData.IsConfigToBeCreated ? AddConfigToScript(scriptSyntax, wizardData.ConfigName) : scriptSyntax.Configs.FirstOrDefault((Config conf) => conf.Name == wizardData.ConfigName));
			AddActionImportsToConfig(config, wizardData.CodeElementsToBeImported);
			if (wizardData.IsConfigToBeCreated)
			{
				cordDesignTimeManager.TrackChange(scriptSyntax);
			}
			else
			{
				cordDesignTimeManager.TrackChange(config);
			}
			cordDesignTimeManager.ComputeChanges(ComputeChangeCallBack);
			return true;
		}

		private Config AddConfigToScript(CoordinationScript scriptSyntax, string configName)
		{
			Config config = syntaxElementBuilder.CreateConfig(configName);
			scriptSyntax.Configs.Add(config);
			return config;
		}

		private void AddActionImportsToConfig(Config config, IEnumerable<CodeElementAndContainerPair> codeElements)
		{
			foreach (CodeElementAndContainerPair codeElement in codeElements)
			{
				ConfigClause.ImportMethod importMethod = syntaxElementBuilder.CreateActionImport(codeElement.Element, codeElement.Container);
				if (importMethod != null)
				{
					config.Clauses.Add(importMethod);
					Report.InsertedClauses.Add(importMethod.ToString());
				}
			}
		}

		private void ComputeChangeCallBack(string scriptName, string newScriptContent)
		{
			package.editorFactory.OpenDocument(scriptName);
			CordDocument openedCordDocumentByName = package.editorFactory.GetOpenedCordDocumentByName(scriptName);
			package.Assert(openedCordDocumentByName != null);
			openedCordDocumentByName.SetBufferContent(newScriptContent);
		}
	}
}
