using System;
using System.Collections.Generic;
using System.Compiler.Metadata;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	internal class RemoteExplorer : DisposableMarshalByRefObject, IRemoteExplorer, IDisposable
	{
		private SpecExplorerApplicationBase application;

		private ExplorerConfiguration explorerConfig;

		private Thread workerThread;

		private EventWaitHandle workerWaitHandle;

		private EventAdapter eventAdapter;

		private ExplorationState state;

		private IExploringOperator exploringOperator;

		private readonly object syncRoot = new object();

		public object AbortLock { get; private set; }

		public ExplorationState State
		{
			get
			{
				lock (syncRoot)
				{
					return state;
				}
			}
			set
			{
				lock (syncRoot)
				{
					state = value;
				}
			}
		}

		public ExplorationResult ExplorationResult
		{
			get
			{
				if (exploringOperator != null)
				{
					return exploringOperator.ExplorationResult;
				}
				return null;
			}
		}

		public IEnumerable<string> TempAssemblyFiles
		{
			get
			{
				if (application != null)
				{
					ICodeGeneratorProvider service = application.GetService<ICodeGeneratorProvider>();
					if (service != null)
					{
						return service.TempAssemblyFiles;
					}
				}
				return Enumerable.Empty<string>();
			}
		}

		public RemoteExplorer()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) => Session.ResolveAssemblyFromPotentialOtherLoadContext(args.Name);
			AbortLock = new object();
		}

		public void Configure(ExplorerConfiguration explorerConfig, EventManager eventManager, ExplorerMediator explorerMediator, bool isRemoteAppDomain)
		{
			this.explorerConfig = explorerConfig;
			Version targetPlatformVersion;
			string runtimeFolder;
			AnalyzeTargetPlatformAndRuntimeAssemblyPath(this.explorerConfig.Assemblies, out targetPlatformVersion, out runtimeFolder);
			application = new SpecExplorerApplicationBase(explorerMediator.InstallDir, true, explorerMediator, targetPlatformVersion, runtimeFolder, GetDefinedConstraintSolverTimeoutValue(this.explorerConfig.MachineSwitches));
			eventAdapter = new EventAdapter(eventManager, this);
			application.Setup.Add(new DefaultErrorReportProvider(eventAdapter));
			IMachineExplorationExplorerProvider service = application.GetService<IMachineExplorationExplorerProvider>();
			service.ExplorationStatisticsEventHandler = delegate(object sender, ExplorationStatisticsEventArgs args)
			{
				eventAdapter.ShowStatistics(args.Statistics);
			};
			Console.SetOut(new RedirectedTextWriter(eventAdapter));
			Debug.Listeners.Add(new TextWriterTraceListener(new RedirectedTextWriter(eventAdapter)));
			workerWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
			workerThread = new Thread(Work);
			workerThread.CurrentCulture = new CultureInfo("en-US");
			state = ExplorationState.Created;
		}

		private static int? GetDefinedConstraintSolverTimeoutValue(IDictionary<string, string> machineSwitches)
		{
			string value;
			int result;
			if (machineSwitches.TryGetValue("constraintsolvertimeout", out value) && int.TryParse(value, out result))
			{
				return result;
			}
			return null;
		}

		private void AnalyzeTargetPlatformAndRuntimeAssemblyPath(IEnumerable<string> assemblies, out Version targetPlatformVersion, out string runtimeFolder)
		{
			targetPlatformVersion = null;
			runtimeFolder = null;
			if (assemblies == null || assemblies.Count() == 0)
			{
				return;
			}
			string text = assemblies.FirstOrDefault();
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			Assembly assembly = null;
			try
			{
				assembly = Assembly.LoadFrom(text);
			}
			catch
			{
				return;
			}
			if (assembly == null)
			{
				return;
			}
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			if (referencedAssemblies == null)
			{
				return;
			}
			AssemblyName assemblyName = referencedAssemblies.FirstOrDefault((AssemblyName r) => r.Name.Equals("mscorlib", StringComparison.CurrentCultureIgnoreCase));
			if (assemblyName == null)
			{
				return;
			}
			targetPlatformVersion = assemblyName.Version;
			AssemblyName assemblyName2 = referencedAssemblies.FirstOrDefault((AssemblyName r) => r.Name.Equals("Microsoft.Xrt.Runtime", StringComparison.CurrentCultureIgnoreCase));
			if (assemblyName2 != null)
			{
				Assembly assembly2 = null;
				try
				{
					assembly2 = Assembly.Load(assemblyName2);
				}
				catch
				{
				}
				if (assembly2 != null && !string.IsNullOrEmpty(assembly2.CodeBase))
				{
					runtimeFolder = Path.GetDirectoryName(new Uri(assembly2.CodeBase).AbsolutePath);
				}
			}
		}

		public void Abort()
		{
			DisposeWorker(-1);
		}

		private void DisposeWorker(int? joinTimeout)
		{
			if (workerThread != null)
			{
				lock (AbortLock)
				{
					workerThread.Abort();
					if (joinTimeout.HasValue)
					{
						workerThread.Join(joinTimeout.Value);
					}
					else if (application != null)
					{
						workerThread.Join(application.Configuration.Options.ConstraintSolverTimeout);
					}
				}
				State = ExplorationState.Aborted;
				workerThread = null;
			}
			if (application != null)
			{
				application.Dispose();
				application = null;
			}
			if (workerWaitHandle != null)
			{
				workerWaitHandle.Close();
				workerWaitHandle = null;
			}
		}

		public override void Dispose()
		{
			if (!IsDisposed)
			{
				DisposeWorker(null);
				base.Dispose();
				GC.SuppressFinalize(this);
			}
		}

		public void StartBuild()
		{
			lock (syncRoot)
			{
				if (state == ExplorationState.Created)
				{
					workerThread.Start();
				}
			}
		}

		public void StartExploration()
		{
			lock (syncRoot)
			{
				if (state == ExplorationState.Created)
				{
					workerThread.Start();
				}
				workerWaitHandle.Set();
			}
		}

		private void Wait()
		{
			workerWaitHandle.WaitOne();
		}

		private void Work()
		{
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				IMachine machine = null;
				IAssembly mainAssembly = null;
				IConfiguration config = null;
				Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
				MachineConfigBuilder machineConfigBuilder = new MachineConfigBuilder(application, eventAdapter, explorerConfig);
				if (machineConfigBuilder.Build(out machine, out config, out mainAssembly))
				{
					machineConfigBuilder = null;
					Wait();
					Explore(machine, config);
				}
			}
			catch (ThreadInterruptedException)
			{
			}
			catch (FileLoadException ex2)
			{
				FieldInfo field = ex2.GetType().GetField("_HResult", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field == null)
				{
					eventAdapter.RecoverFromFatalError(new MissingMemberException("Cannot get HRESULT property from FileLoadException"));
					eventAdapter.SwitchState(ExplorationState.Aborted);
					eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
					return;
				}
				int num = (int)field.GetValue(ex2);
				if (num != -2146233063)
				{
					eventAdapter.RecoverFromFatalError(ex2);
					eventAdapter.SwitchState(ExplorationState.Aborted);
					eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
				}
			}
			catch (ConformanceTestingException ex3)
			{
				eventAdapter.DiagMessage(DiagnosisKind.Error, "fatal conformance testing failure: " + ex3.Message);
				eventAdapter.Log("=== remote explorer failure ===");
				eventAdapter.Log(ex3.Message);
				eventAdapter.SwitchState(ExplorationState.Aborted);
				eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
			}
			catch (UnsupportedILException ex4)
			{
				eventAdapter.DiagMessage(DiagnosisKind.Error, string.Format("fatal execution failure: {0}.\r\nSetting type \"{1}\" to native with \"Microsoft.Xrt.Runtime.NativeTypeAttribute\" or in XRT.Config file might solve the problem.", ex4.Message, ex4.TypeName));
				eventAdapter.Log("=== remote explorer failure ===");
				eventAdapter.Log(ex4.Message);
				eventAdapter.SwitchState(ExplorationState.Aborted);
				eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
			}
			catch (InvalidProbeException ex5)
			{
				ITextDocument textDocument = ex5.Location.Document as ITextDocument;
				TextLocation textLocation = ((textDocument == null) ? new TextLocation("<unspecified source>", 1, 1) : new TextLocation(textDocument.ShortName, (short)textDocument.GetLine(ex5.Location.StartPosition), (short)textDocument.GetColumn(ex5.Location.StartPosition)));
				eventAdapter.DiagMessage(DiagnosisKind.Error, string.Format("Invalid state probe '{0}': {1}.", ex5.ProbeName, ex5.Message), textLocation);
				eventAdapter.Log("=== remote explorer failure ===");
				eventAdapter.Log(ex5.Message);
				eventAdapter.SwitchState(ExplorationState.Aborted);
				eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
			}
			catch (ExplorationRuntimeException ex6)
			{
				eventAdapter.DiagMessage(DiagnosisKind.Error, "fatal execution failure: " + ex6.Message);
				eventAdapter.Log("=== remote explorer failure ===");
				eventAdapter.Log(ex6.Message);
				eventAdapter.SwitchState(ExplorationState.Aborted);
				eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
			}
			catch (InvalidMetadataException ex7)
			{
				eventAdapter.DiagMessage(DiagnosisKind.Error, "fatal execution failure: " + ((Exception)(object)ex7).Message);
				eventAdapter.Log("=== remote explorer failure ===");
				eventAdapter.Log(((Exception)(object)ex7).Message);
				eventAdapter.SwitchState(ExplorationState.Aborted);
				eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
			}
			catch (NonDeterministicChoicesException ex8)
			{
				eventAdapter.DiagMessage(DiagnosisKind.Error, ex8.Message);
				eventAdapter.Log("=== remote explorer failure ===");
				eventAdapter.Log(ex8.Message);
				eventAdapter.SwitchState(ExplorationState.Aborted);
				eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
			}
			catch (Exception ex9)
			{
				if (!(ex9 is ThreadAbortException))
				{
					eventAdapter.RecoverFromFatalError(ex9);
					eventAdapter.SwitchState(ExplorationState.Aborted);
					eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
					return;
				}
				throw;
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		private void Explore(IMachine machine, IConfiguration config)
		{
			lock (syncRoot)
			{
				if (state == ExplorationState.Exploring)
				{
					return;
				}
			}
			IOptionSetManager requiredService = application.GetRequiredService<IOptionSetManager>();
			ExplorationOptions options = requiredService.CurrentOptionSet.GetOptions<ExplorationOptions>();
			if (explorerConfig.ExplorationMode == ExplorationMode.Exploration)
			{
				exploringOperator = new ExploringOperator(machine, config, eventAdapter, explorerConfig, options, workerWaitHandle, this);
			}
			else
			{
				exploringOperator = new TestingOperator(machine, config, eventAdapter, explorerConfig, options, workerWaitHandle, this);
			}
			exploringOperator.ExplorationResultUpdated += delegate(object sender, ExplorationResultEventArgs args)
			{
				eventAdapter.UpdateExplorationResult(args.ExplorationResult);
			};
			exploringOperator.Explore();
		}

		public void SuspendExploration()
		{
			if (exploringOperator == null)
			{
				throw new InvalidOperationException("The exploring operator has not been created.");
			}
			exploringOperator.SuspendExploration();
		}
	}
}
