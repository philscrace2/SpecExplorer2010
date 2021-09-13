// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.MachineConfigBuilder
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.Xrt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Microsoft.SpecExplorer
{
  internal class MachineConfigBuilder
  {
    private ExplorerConfiguration explorerConfig;
    private ICordCompiler compiler;
    private ApplicationBase host;
    private IProgram program;
    private EventAdapter eventAdapter;

    public MachineConfigBuilder(
      ApplicationBase host,
      EventAdapter eventAdapter,
      ExplorerConfiguration explorerConfig)
    {
      this.host = host;
      this.explorerConfig = explorerConfig;
      this.eventAdapter = eventAdapter;
      this.compiler = this.host.GetRequiredService<ICordProvider>().CreateCordCompiler();
      this.program = this.host.GetRequiredService<IProgram>();
    }

    public bool Build(out IMachine machine, out IConfiguration config, out IAssembly mainAssembly)
    {
      machine = (IMachine) null;
      mainAssembly = (IAssembly) null;
      config = (IConfiguration) null;
      this.eventAdapter.SwitchState(ExplorationState.Building);
      this.eventAdapter.ProgressMessage(VerbosityLevel.Medium, "Checking scripts...");
      bool hasErrors = true;
      if (this.explorerConfig.Assemblies.Count == 0)
        this.eventAdapter.DiagMessage(DiagnosisKind.Error, "no assemblies provided.");
      else if (this.explorerConfig.Scripts.Count == 0)
        this.eventAdapter.DiagMessage(DiagnosisKind.Error, "no scripts provided.");
      else if (this.LoadAssemblies(out mainAssembly))
      {
        this.program.MainAssembly = mainAssembly;
        if (this.Compile())
        {
          if (string.IsNullOrEmpty(this.explorerConfig.Machine))
            hasErrors = false;
          else if (this.TryGetMachine(out machine))
          {
            config = machine.Configuration;
            if (config == null)
            {
              this.eventAdapter.DiagMessage(DiagnosisKind.Error, "cannot find the machine configuration");
            }
            else
            {
              this.Configure(config);
              hasErrors = false;
            }
          }
        }
      }
      return this.ReportMessageAndSwitchState(hasErrors);
    }

    private void Configure(IConfiguration config)
    {
      ExplorationOptions options1 = config.OptionSet.GetOptions<ExplorationOptions>();
      options1.OnTheFlyAllowEndingAtEventStates = this.explorerConfig.AllowEndingAtEventStates;
      if (this.explorerConfig.OnTheFlyMaximumExperimentCount.HasValue)
        options1.OnTheFlyMaximumExperimentCount = this.explorerConfig.OnTheFlyMaximumExperimentCount.Value;
      IOptionSetManager requiredService = this.host.GetRequiredService<IOptionSetManager>();
      requiredService.CurrentOptionSet.Configure(config.OptionSet);
      ExtendedOptions options2 = requiredService.CurrentOptionSet.GetOptions<ExtendedOptions>();
      bool flag = options2.Tracing == TraceLevel.Off;
      this.host.SetTracing(options2.Tracing, flag ? (string) null : this.explorerConfig.Machine + ".selog");
    }

    private bool TryGetMachine(out IMachine machine)
    {
      string machine1 = this.explorerConfig.Machine;
      machine = this.compiler.GetMachine(machine1);
      if (this.compiler.ErrorDispatcher.HasErrors)
        return false;
      if (machine != null)
        return true;
      this.eventAdapter.DiagMessage(DiagnosisKind.Error, string.Format("cannot find main machine {0}.", (object) machine1));
      return false;
    }

    private bool ReportMessageAndSwitchState(bool hasErrors)
    {
      if (hasErrors)
      {
        this.eventAdapter.ProgressMessage(VerbosityLevel.Medium, "Checking script failed.");
        this.eventAdapter.SwitchState(ExplorationState.FailedBuilding);
        return false;
      }
      this.eventAdapter.ProgressMessage(VerbosityLevel.Medium, "Checking script succeeded.");
      this.eventAdapter.SwitchState(ExplorationState.FinishedBuilding);
      return true;
    }

    private bool Compile()
    {
      if (this.program.SystemAssemblies.MicrosoftXrtRuntime != null)
        this.compiler.AddAssembly(this.program.SystemAssemblies.MicrosoftXrtRuntime);
      this.compiler.AddAssembly(this.program.SystemAssemblies.MSCorLib);
      this.compiler.ErrorDispatcher.ErrorReported += new EventHandler<ErrorReport>(this.OnCompilingError);
      List<string> stringList = new List<string>();
      stringList.Add(Path.Combine(this.host.PersistentConfigurationBaseDir, this.host.Configuration.DefaultCordPreludeFileName));
      stringList.AddRange((IEnumerable<string>) this.explorerConfig.Scripts);
      return this.compiler.Compile(stringList.ToArray(), CompilationMode.ParseAndCheck) == 0;
    }

    private bool LoadAssemblies(out IAssembly mainAssembly)
    {
      bool flag = true;
      mainAssembly = (IAssembly) null;
      foreach (string assembly1 in (IEnumerable<string>) this.explorerConfig.Assemblies)
      {
        IAssembly assembly2 = this.program.LoadAssemblyFrom(assembly1, false);
        if (assembly2 == null)
        {
          this.eventAdapter.DiagMessage(DiagnosisKind.Warning, string.Format("Cannot load assembly '{0}'", (object) assembly1));
        }
        else
        {
          if (!(this.explorerConfig.Assemblies is IList))
            throw new InvalidOperationException("Cannot decide the main assembly due to 'assemblies' is not a List.");
          if (flag)
          {
            mainAssembly = assembly2;
            flag = false;
          }
          this.compiler.AddAssembly(assembly2);
        }
      }
      return true;
    }

    private void OnCompilingError(object sender, ErrorReport report) => this.eventAdapter.DiagMessage(report.Kind.ToDiagnosisKind(), report.Description, (object) new TextLocation(report.FileName, (short) report.Line, (short) report.Column));
  }
}
