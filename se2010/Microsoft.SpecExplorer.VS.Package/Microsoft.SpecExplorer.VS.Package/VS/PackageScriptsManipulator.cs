// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.PackageScriptsManipulator
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.ActionMachines.Cord;
using System;
using System.Collections.Generic;
using System.Linq;

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
      this.Report = new ScriptManipulationReport();
      string containerScriptPath = wizardData.IsScriptToBeCreated ? this.package.CreateScriptAndAddToProject(wizardData.ProjectName, wizardData.ScriptName) : wizardData.ScriptName;
      if (containerScriptPath == null)
        return false;
      this.syntaxElementBuilder = new CordSyntaxElementBuilder(this.package, wizardData.ProjectName, containerScriptPath);
      ICordDesignTimeManager designTimeManager = this.package.CordScopeManager.GetCordDesignTimeManager(wizardData.ProjectName);
      CoordinationScript scriptSyntax = designTimeManager.GetScriptSyntax(containerScriptPath);
      Config config = wizardData.IsConfigToBeCreated ? this.AddConfigToScript(scriptSyntax, wizardData.ConfigName) : ((IEnumerable<Config>) scriptSyntax.Configs).FirstOrDefault<Config>((Func<Config, bool>) (conf => (string) conf.Name == wizardData.ConfigName));
      this.AddActionImportsToConfig(config, wizardData.CodeElementsToBeImported);
      if (wizardData.IsConfigToBeCreated)
        designTimeManager.TrackChange((SyntaxElement) scriptSyntax);
      else
        designTimeManager.TrackChange((SyntaxElement) config);
      designTimeManager.ComputeChanges(new Action<string, string>(this.ComputeChangeCallBack));
      return true;
    }

    private Config AddConfigToScript(CoordinationScript scriptSyntax, string configName)
    {
      Config config = this.syntaxElementBuilder.CreateConfig(configName);
      ((ICollection<Config>) scriptSyntax.Configs).Add(config);
      return config;
    }

    private void AddActionImportsToConfig(
      Config config,
      IEnumerable<CodeElementAndContainerPair> codeElements)
    {
      foreach (CodeElementAndContainerPair codeElement in codeElements)
      {
        ConfigClause.ImportMethod actionImport = this.syntaxElementBuilder.CreateActionImport(codeElement.Element, codeElement.Container);
        if (actionImport != null)
        {
          ((ICollection<ConfigClause>) config.Clauses).Add((ConfigClause) actionImport);
          this.Report.InsertedClauses.Add(((object) actionImport).ToString());
        }
      }
    }

    private void ComputeChangeCallBack(string scriptName, string newScriptContent)
    {
      this.package.editorFactory.OpenDocument(scriptName);
      CordDocument cordDocumentByName = this.package.editorFactory.GetOpenedCordDocumentByName(scriptName);
      this.package.Assert(cordDocumentByName != null);
      cordDocumentByName.SetBufferContent(newScriptContent);
    }
  }
}
