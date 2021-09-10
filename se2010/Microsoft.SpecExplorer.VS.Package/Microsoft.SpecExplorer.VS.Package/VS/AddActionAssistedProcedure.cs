// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.AddActionAssistedProcedure
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using Microsoft.ActionMachines.Cord;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Interop;


namespace Microsoft.SpecExplorer.VS
{
  internal class AddActionAssistedProcedure : IAssistedProcedure
  {
    private SpecExplorerPackage package;
    private AddActionWizardController wizardController;
    private PackageScriptsManipulator scriptManipulator;
    private uint pdwCookie;
    private bool isValidating;

    public AddActionAssistedProcedure(SpecExplorerPackage package)
    {
      this.package = package;
      this.wizardController = new AddActionWizardController(package);
      this.wizardController.ValidateConfigNameInput += new ItemNameInputValidator(this.ValidateConfigName);
      this.wizardController.ValidateScriptNameInput += new ItemNameInputValidator(this.ValidateScriptName);
      this.scriptManipulator = new PackageScriptsManipulator(package);
      this.package.SolutionBuildFinished += new EventHandler<SolutionBuildEventArgs>(this.OnSolutionBuildFinished);
    }

    public void Invoke()
    {
      if (ProjectUtils.GetAllRealProjects(this.package.DTE).Count<Project>() <= 0)
      {
        this.package.NotificationDialog("Assisted Procedure Launch Failed", "No solution has been opened.");
      }
      else
      {
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
          this.package.DiagMessage((DiagnosisKind) 0, Resources.FailedToBuildProject, (object) null);
          this.package.ProgressMessage((VerbosityLevel) 0, Resources.ValidationFailed);
          this.package.MakeErrorListVisible();
          this.package.NotificationDialog("Assisted Procedure Launch Failed", "Failed Launching Rule Declaration Assisted Procedure, due to Validation failure. Kindly go through the Error List for details");
        }
        else
          this.Launch();
      }
    }

    private void OnSolutionBuildFinished(object sender, SolutionBuildEventArgs e)
    {
      if (!this.isValidating)
        return;
      if (!e.IsCanceled && e.IsSucceeded && this.package.ValidateAllScripts())
      {
        this.package.ProgressMessage((VerbosityLevel) 0, Resources.ValidationSucceeded);
        this.Launch();
      }
      else
      {
        this.package.ProgressMessage((VerbosityLevel) 0, Resources.ValidationFailed);
        this.package.MakeErrorListVisible();
        this.package.Session.Host.NotificationDialog("Assisted Procedure Launch Failed", "Failed Launching Add Action Assisted Procedure, due to Validation failure. Kindly go through the Error List for details");
      }
      this.isValidating = false;
      this.package.AssertOk(this.package.SolutionBuildManager.UnadviseUpdateSolutionEvents(this.pdwCookie));
      this.pdwCookie = 0U;
    }

    private void Launch()
    {
      if (!this.wizardController.LaunchWizard() || !this.scriptManipulator.CommitChanges(this.wizardController.WizardData))
        return;
      AssistedProcedureReportWindow procedureReportWindow = new AssistedProcedureReportWindow(this.scriptManipulator.Report, "Action Import Result");
      new WindowInteropHelper((System.Windows.Window) procedureReportWindow).Owner = this.package.MainWindowHandle;
      procedureReportWindow.ShowDialog();
    }

    private string ValidateScriptName(string newScriptName, string containerProject)
    {
      if (string.IsNullOrEmpty(newScriptName) || ((IEnumerable<char>) Path.GetInvalidFileNameChars()).Any<char>((Func<char, bool>) (c => newScriptName.Contains<char>(c))))
        return "Invalid file name pattern";
      Project projectByUniqueName = this.package.GetProjectByUniqueName(containerProject);
      if (projectByUniqueName == null)
        return (string) null;
      string directoryName = Path.GetDirectoryName(projectByUniqueName.FullName);
      if (!File.Exists(Path.Combine(directoryName, FileNames.HasScriptExtension(newScriptName) ? newScriptName : newScriptName + ".cord")))
        return (string) null;
      return "Project folder " + directoryName + " already contains a script named " + newScriptName;
    }

    private string ValidateConfigName(string newConfigName, string containerProject)
    {
      if (string.IsNullOrEmpty(newConfigName) || !Regex.IsMatch(newConfigName, "^([_a-zA-Z][_a-zA-Z0-9]*)$"))
        return "Invalid identifier pattern for config name";
      if (CordUtils.CordKeywords.Contains(newConfigName))
        return "Configuration name cannot be a Cord Keyword.";
      if (containerProject == null)
        return (string) null;
      ICordDesignTimeManager designTimeManager = this.package.CordScopeManager.GetCordDesignTimeManager(containerProject);
      if (designTimeManager == null || !((IEnumerable<Config>) designTimeManager.AllConfigurations).Any<Config>((Func<Config, bool>) (config => (string) config.Name == newConfigName)))
        return (string) null;
      return "config " + newConfigName + " already exists in project " + containerProject;
    }
  }
}
