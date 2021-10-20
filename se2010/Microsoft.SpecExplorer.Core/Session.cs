using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.ActionMachines.Cord;
using Microsoft.SpecExplorer.ModelingGuidance;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.Viewer;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	public class Session : ComponentBase, ISession
	{
		private IHost host;

		private string installDir;

		private static bool resolving;

		public ApplicationBase Application { get; private set; }

		public IHost Host
		{
			get
			{
				return host;
			}
		}

		public string InstallDir
		{
			get
			{
				if (installDir == null)
				{
					string text = string.Empty;
					string arg = (installDir = Path.GetDirectoryName(new Uri(typeof(Session).Assembly.CodeBase).LocalPath));
					while (!string.IsNullOrEmpty(installDir) && !File.Exists(Path.Combine(installDir, "xrt.config")))
					{
						try
						{
							installDir = Path.GetDirectoryName(installDir);
						}
						catch (Exception ex)
						{
							text += string.Format("Failed to get directory name from '{0}' : {1}", installDir, ex.Message);
							installDir = null;
						}
					}
					if (installDir == null)
					{
						text += string.Format("Failed to find xrt.config file in path '{0}'.", arg);
						Host.FatalError("Cannot retrieve installation directory." + text);
					}
				}
				return installDir;
			}
		}

		public string ConfigurationDir
		{
			get
			{
				return InstallDir;
			}
		}

		public Session(IHost host)
		{
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			this.host = host;
			Application = new SpecExplorerApplicationBase(ConfigurationDir, false, new ExplorerMediator(this));
			Application.Setup.Add(this);
			RegisterServices();
			AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) => ResolveAssemblyFromPotentialOtherLoadContext(args.Name);
		}

		private void RegisterServices()
		{
			CordDesignTimeScopeManager component = new CordDesignTimeScopeManager(new string[1] { Path.Combine(ConfigurationDir, Application.Configuration.DefaultCordPreludeFileName) });
			Application.Setup.Add(component);
			Application.Setup.Add(new ViewDefinitionManager(host));
			Application.Setup.Add(new GuidanceLoaderImpl());
		}

		internal static Assembly ResolveAssemblyFromPotentialOtherLoadContext(string name)
		{
			if (resolving)
			{
				return null;
			}
			try
			{
				resolving = true;
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					if (assembly.FullName == name)
					{
						return assembly;
					}
				}
				return Assembly.Load(name);
			}
			catch
			{
				return null;
			}
			finally
			{
				resolving = false;
			}
		}

		public IExplorer CreateExplorer(ICollection<string> assemblies, ICollection<string> scripts, ExplorationMode explorationMode, string machine, string outputDir, string replayPath, int? onTheFlyMaximumExperimentCount, IDictionary<string, string> machineSwitches, bool allowEndingAtEventStates)
		{
			ExplorerConfiguration explorerConfig = new ExplorerConfiguration(assemblies, scripts, explorationMode, machine, machineSwitches, outputDir, replayPath, onTheFlyMaximumExperimentCount, allowEndingAtEventStates);
			return new Explorer(this, explorerConfig);
		}

		public ITestCodeGenerator CreateTestCodeGenerator(TransitionSystem transitionSystem)
		{
			return new StaticTestCodeGenerator(Host, transitionSystem);
		}

		public override void Dispose()
		{
			if (!IsDisposed && Application != null)
			{
				Application.Dispose();
				Application = null;
			}
			base.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
