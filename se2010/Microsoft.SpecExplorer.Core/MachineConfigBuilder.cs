using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	internal class MachineConfigBuilder
	{
		private ExplorerConfiguration explorerConfig;

		private ICordCompiler compiler;

		private ApplicationBase host;

		private IProgram program;

		private EventAdapter eventAdapter;

		public MachineConfigBuilder(ApplicationBase host, EventAdapter eventAdapter, ExplorerConfiguration explorerConfig)
		{
			this.host = host;
			this.explorerConfig = explorerConfig;
			this.eventAdapter = eventAdapter;
			ICordProvider requiredService = this.host.GetRequiredService<ICordProvider>();
			compiler = requiredService.CreateCordCompiler();
			program = this.host.GetRequiredService<IProgram>();
		}

		public bool Build(out IMachine machine, out IConfiguration config, out IAssembly mainAssembly)
		{
			machine = null;
			mainAssembly = null;
			config = null;
			eventAdapter.SwitchState(ExplorationState.Building);
			eventAdapter.ProgressMessage(VerbosityLevel.Medium, "Checking scripts...");
			bool hasErrors = true;
			if (explorerConfig.Assemblies.Count == 0)
			{
				eventAdapter.DiagMessage(DiagnosisKind.Error, "no assemblies provided.");
			}
			else if (explorerConfig.Scripts.Count == 0)
			{
				eventAdapter.DiagMessage(DiagnosisKind.Error, "no scripts provided.");
			}
			else if (LoadAssemblies(out mainAssembly))
			{
				program.MainAssembly = mainAssembly;
				if (Compile())
				{
					if (string.IsNullOrEmpty(explorerConfig.Machine))
					{
						hasErrors = false;
					}
					else if (TryGetMachine(out machine))
					{
						config = machine.Configuration;
						if (config == null)
						{
							eventAdapter.DiagMessage(DiagnosisKind.Error, "cannot find the machine configuration");
						}
						else
						{
							Configure(config);
							hasErrors = false;
						}
					}
				}
			}
			return ReportMessageAndSwitchState(hasErrors);
		}

		private void Configure(IConfiguration config)
		{
			ExplorationOptions options = config.OptionSet.GetOptions<ExplorationOptions>();
			options.OnTheFlyAllowEndingAtEventStates = explorerConfig.AllowEndingAtEventStates;
			if (explorerConfig.OnTheFlyMaximumExperimentCount.HasValue)
			{
				options.OnTheFlyMaximumExperimentCount = explorerConfig.OnTheFlyMaximumExperimentCount.Value;
			}
			IOptionSetManager requiredService = host.GetRequiredService<IOptionSetManager>();
			requiredService.CurrentOptionSet.Configure(config.OptionSet);
			ExtendedOptions options2 = requiredService.CurrentOptionSet.GetOptions<ExtendedOptions>();
			bool flag = options2.Tracing == TraceLevel.Off;
			host.SetTracing(options2.Tracing, flag ? null : (explorerConfig.Machine + ".selog"));
		}

		private bool TryGetMachine(out IMachine machine)
		{
			string machine2 = explorerConfig.Machine;
			machine = compiler.GetMachine(machine2);
			if (compiler.ErrorDispatcher.HasErrors)
			{
				return false;
			}
			if (machine == null)
			{
				eventAdapter.DiagMessage(DiagnosisKind.Error, string.Format("cannot find main machine {0}.", machine2));
				return false;
			}
			return true;
		}

		private bool ReportMessageAndSwitchState(bool hasErrors)
		{
			if (hasErrors)
			{
				eventAdapter.ProgressMessage(VerbosityLevel.Medium, "Checking script failed.");
				eventAdapter.SwitchState(ExplorationState.FailedBuilding);
				return false;
			}
			eventAdapter.ProgressMessage(VerbosityLevel.Medium, "Checking script succeeded.");
			eventAdapter.SwitchState(ExplorationState.FinishedBuilding);
			return true;
		}

		private bool Compile()
		{
			if (program.SystemAssemblies.MicrosoftXrtRuntime != null)
			{
				compiler.AddAssembly(program.SystemAssemblies.MicrosoftXrtRuntime);
			}
			compiler.AddAssembly(program.SystemAssemblies.MSCorLib);
			compiler.ErrorDispatcher.ErrorReported += OnCompilingError;
			List<string> list = new List<string>();
			string item = Path.Combine(host.PersistentConfigurationBaseDir, host.Configuration.DefaultCordPreludeFileName);
			list.Add(item);
			list.AddRange(explorerConfig.Scripts);
			return compiler.Compile(list.ToArray(), CompilationMode.ParseAndCheck) == 0;
		}

		private bool LoadAssemblies(out IAssembly mainAssembly)
		{
			bool flag = true;
			mainAssembly = null;
			foreach (string assembly2 in explorerConfig.Assemblies)
			{
				IAssembly assembly = program.LoadAssemblyFrom(assembly2, false);
				if (assembly == null)
				{
					eventAdapter.DiagMessage(DiagnosisKind.Warning, string.Format("Cannot load assembly '{0}'", assembly2));
					continue;
				}
				if (!(explorerConfig.Assemblies is IList))
				{
					throw new InvalidOperationException("Cannot decide the main assembly due to 'assemblies' is not a List.");
				}
				if (flag)
				{
					mainAssembly = assembly;
					flag = false;
				}
				compiler.AddAssembly(assembly);
			}
			return true;
		}

		private void OnCompilingError(object sender, ErrorReport report)
		{
			eventAdapter.DiagMessage(report.Kind.ToDiagnosisKind(), report.Description, new TextLocation(report.FileName, (short)report.Line, (short)report.Column));
		}
	}
}
