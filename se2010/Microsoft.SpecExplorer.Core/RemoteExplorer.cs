// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.RemoteExplorer
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.Compiler.Metadata;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

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
      set
      {
        lock (this.syncRoot)
          this.state = value;
      }
      get
      {
        lock (this.syncRoot)
          return this.state;
      }
    }

    public RemoteExplorer()
    {
      AppDomain.CurrentDomain.AssemblyResolve += (ResolveEventHandler) ((sender, args) => Session.ResolveAssemblyFromPotentialOtherLoadContext(args.Name));
      this.AbortLock = new object();
    }

    public void Configure(
      ExplorerConfiguration explorerConfig,
      EventManager eventManager,
      ExplorerMediator explorerMediator,
      bool isRemoteAppDomain)
    {
      this.explorerConfig = explorerConfig;
      Version targetPlatformVersion;
      string runtimeFolder;
      this.AnalyzeTargetPlatformAndRuntimeAssemblyPath((IEnumerable<string>) this.explorerConfig.Assemblies, out targetPlatformVersion, out runtimeFolder);
      this.application = new SpecExplorerApplicationBase(explorerMediator.InstallDir, true, explorerMediator, targetPlatformVersion, runtimeFolder, RemoteExplorer.GetDefinedConstraintSolverTimeoutValue(this.explorerConfig.MachineSwitches));
      this.eventAdapter = new EventAdapter(eventManager, (IRemoteExplorer) this);
      this.application.Setup.Add((IComponent) new DefaultErrorReportProvider(this.eventAdapter));
      this.application.GetService<IMachineExplorationExplorerProvider>().ExplorationStatisticsEventHandler = (EventHandler<ExplorationStatisticsEventArgs>) ((sender, args) => this.eventAdapter.ShowStatistics(args.Statistics));
      Console.SetOut((TextWriter) new RedirectedTextWriter(this.eventAdapter));
      Debug.Listeners.Add((TraceListener) new TextWriterTraceListener((TextWriter) new RedirectedTextWriter(this.eventAdapter)));
      this.workerWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
      this.workerThread = new Thread(new ThreadStart(this.Work));
      this.workerThread.CurrentCulture = new CultureInfo("en-US");
      this.state = ExplorationState.Created;
    }

    private static int? GetDefinedConstraintSolverTimeoutValue(
      IDictionary<string, string> machineSwitches)
    {
      string s;
      int result;
      return machineSwitches.TryGetValue("constraintsolvertimeout", out s) && int.TryParse(s, out result) ? new int?(result) : new int?();
    }

    private void AnalyzeTargetPlatformAndRuntimeAssemblyPath(
      IEnumerable<string> assemblies,
      out Version targetPlatformVersion,
      out string runtimeFolder)
    {
      targetPlatformVersion = (Version) null;
      runtimeFolder = (string) null;
      if (assemblies == null || assemblies.Count<string>() == 0)
        return;
      string assemblyFile = assemblies.FirstOrDefault<string>();
      if (string.IsNullOrEmpty(assemblyFile))
        return;
      System.Reflection.Assembly assembly1;
      try
      {
        assembly1 = System.Reflection.Assembly.LoadFrom(assemblyFile);
      }
      catch
      {
        return;
      }
      if (assembly1 == (System.Reflection.Assembly) null)
        return;
      AssemblyName[] referencedAssemblies = assembly1.GetReferencedAssemblies();
      if (referencedAssemblies == null)
        return;
      AssemblyName assemblyName = ((IEnumerable<AssemblyName>) referencedAssemblies).FirstOrDefault<AssemblyName>((Func<AssemblyName, bool>) (r => r.Name.Equals("mscorlib", StringComparison.CurrentCultureIgnoreCase)));
      if (assemblyName == null)
        return;
      targetPlatformVersion = assemblyName.Version;
      AssemblyName assemblyRef = ((IEnumerable<AssemblyName>) referencedAssemblies).FirstOrDefault<AssemblyName>((Func<AssemblyName, bool>) (r => r.Name.Equals("Microsoft.Xrt.Runtime", StringComparison.CurrentCultureIgnoreCase)));
      if (assemblyRef == null)
        return;
      System.Reflection.Assembly assembly2 = (System.Reflection.Assembly) null;
      try
      {
        assembly2 = System.Reflection.Assembly.Load(assemblyRef);
      }
      catch
      {
      }
      if (!(assembly2 != (System.Reflection.Assembly) null) || string.IsNullOrEmpty(assembly2.CodeBase))
        return;
      runtimeFolder = Path.GetDirectoryName(new Uri(assembly2.CodeBase).AbsolutePath);
    }

    public void Abort() => this.DisposeWorker(new int?(-1));

    private void DisposeWorker(int? joinTimeout)
    {
      if (this.workerThread != null)
      {
        lock (this.AbortLock)
        {
          this.workerThread.Abort();
          if (joinTimeout.HasValue)
            this.workerThread.Join(joinTimeout.Value);
          else if (this.application != null)
            this.workerThread.Join(this.application.Configuration.Options.ConstraintSolverTimeout);
        }
        this.State = ExplorationState.Aborted;
        this.workerThread = (Thread) null;
      }
      if (this.application != null)
      {
        this.application.Dispose();
        this.application = (SpecExplorerApplicationBase) null;
      }
      if (this.workerWaitHandle == null)
        return;
      this.workerWaitHandle.Close();
      this.workerWaitHandle = (EventWaitHandle) null;
    }

    public override void Dispose()
    {
      if (this.IsDisposed)
        return;
      this.DisposeWorker(new int?());
      base.Dispose();
      GC.SuppressFinalize((object) this);
    }

    public void StartBuild()
    {
      lock (this.syncRoot)
      {
        if (this.state != ExplorationState.Created)
          return;
        this.workerThread.Start();
      }
    }

    public void StartExploration()
    {
      lock (this.syncRoot)
      {
        if (this.state == ExplorationState.Created)
          this.workerThread.Start();
        this.workerWaitHandle.Set();
      }
    }

    private void Wait() => this.workerWaitHandle.WaitOne();

    private void Work()
    {
      CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
      try
      {
        IMachine machine = (IMachine) null;
        IAssembly mainAssembly = (IAssembly) null;
        IConfiguration config = (IConfiguration) null;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        if (!new MachineConfigBuilder((ApplicationBase) this.application, this.eventAdapter, this.explorerConfig).Build(out machine, out config, out mainAssembly))
          return;
        this.Wait();
        this.Explore(machine, config);
      }
      catch (ThreadInterruptedException ex)
      {
      }
      catch (FileLoadException ex)
      {
        FieldInfo field = ex.GetType().GetField("_HResult", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == (FieldInfo) null)
        {
          this.eventAdapter.RecoverFromFatalError((Exception) new MissingMemberException("Cannot get HRESULT property from FileLoadException"));
          this.eventAdapter.SwitchState(ExplorationState.Aborted);
          this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
        }
        else
        {
          if ((int) field.GetValue((object) ex) == -2146233063)
            return;
          this.eventAdapter.RecoverFromFatalError((Exception) ex);
          this.eventAdapter.SwitchState(ExplorationState.Aborted);
          this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
        }
      }
      catch (ConformanceTestingException ex)
      {
        this.eventAdapter.DiagMessage(DiagnosisKind.Error, "fatal conformance testing failure: " + ex.Message);
        this.eventAdapter.Log("=== remote explorer failure ===");
        this.eventAdapter.Log(ex.Message);
        this.eventAdapter.SwitchState(ExplorationState.Aborted);
        this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
      }
      catch (UnsupportedILException ex)
      {
        this.eventAdapter.DiagMessage(DiagnosisKind.Error, string.Format("fatal execution failure: {0}.\r\nSetting type \"{1}\" to native with \"Microsoft.Xrt.Runtime.NativeTypeAttribute\" or in XRT.Config file might solve the problem.", (object) ex.Message, (object) ex.TypeName));
        this.eventAdapter.Log("=== remote explorer failure ===");
        this.eventAdapter.Log(ex.Message);
        this.eventAdapter.SwitchState(ExplorationState.Aborted);
        this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
      }
      catch (InvalidProbeException ex)
      {
        TextLocation textLocation = !(ex.Location.Document is ITextDocument document1) ? new TextLocation("<unspecified source>", (short) 1, (short) 1) : new TextLocation(document1.ShortName, (short) document1.GetLine(ex.Location.StartPosition), (short) document1.GetColumn(ex.Location.StartPosition));
        this.eventAdapter.DiagMessage(DiagnosisKind.Error, string.Format("Invalid state probe '{0}': {1}.", (object) ex.ProbeName, (object) ex.Message), (object) textLocation);
        this.eventAdapter.Log("=== remote explorer failure ===");
        this.eventAdapter.Log(ex.Message);
        this.eventAdapter.SwitchState(ExplorationState.Aborted);
        this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
      }
      catch (ExplorationRuntimeException ex)
      {
        this.eventAdapter.DiagMessage(DiagnosisKind.Error, "fatal execution failure: " + ex.Message);
        this.eventAdapter.Log("=== remote explorer failure ===");
        this.eventAdapter.Log(ex.Message);
        this.eventAdapter.SwitchState(ExplorationState.Aborted);
        this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
      }
      catch (InvalidMetadataException ex)
      {
        this.eventAdapter.DiagMessage(DiagnosisKind.Error, "fatal execution failure: " + ex.Message);
        this.eventAdapter.Log("=== remote explorer failure ===");
        this.eventAdapter.Log(ex.Message);
        this.eventAdapter.SwitchState(ExplorationState.Aborted);
        this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
      }
      catch (NonDeterministicChoicesException ex)
      {
        this.eventAdapter.DiagMessage(DiagnosisKind.Error, ex.Message);
        this.eventAdapter.Log("=== remote explorer failure ===");
        this.eventAdapter.Log(ex.Message);
        this.eventAdapter.SwitchState(ExplorationState.Aborted);
        this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
      }
      catch (Exception ex)
      {
        if (!(ex is ThreadAbortException))
        {
          this.eventAdapter.RecoverFromFatalError(ex);
          this.eventAdapter.SwitchState(ExplorationState.Aborted);
          this.eventAdapter.ProgressMessage(VerbosityLevel.Minimal, "Exploration job aborted.");
        }
        else
          throw;
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = currentCulture;
      }
    }

    private void Explore(IMachine machine, IConfiguration config)
    {
      lock (this.syncRoot)
      {
        if (this.state == ExplorationState.Exploring)
          return;
      }
      ExplorationOptions options = this.application.GetRequiredService<IOptionSetManager>().CurrentOptionSet.GetOptions<ExplorationOptions>();
      this.exploringOperator = this.explorerConfig.ExplorationMode != ExplorationMode.Exploration ? (IExploringOperator) new TestingOperator(machine, config, this.eventAdapter, this.explorerConfig, options, this.workerWaitHandle, (IRemoteExplorer) this) : (IExploringOperator) new ExploringOperator(machine, config, this.eventAdapter, this.explorerConfig, options, this.workerWaitHandle, (IRemoteExplorer) this);
      this.exploringOperator.ExplorationResultUpdated += (EventHandler<ExplorationResultEventArgs>) ((sender, args) => this.eventAdapter.UpdateExplorationResult(args.ExplorationResult));
      this.exploringOperator.Explore();
    }

    public void SuspendExploration()
    {
      if (this.exploringOperator == null)
        throw new InvalidOperationException("The exploring operator has not been created.");
      this.exploringOperator.SuspendExploration();
    }

    public ExplorationResult ExplorationResult => this.exploringOperator != null ? this.exploringOperator.ExplorationResult : (ExplorationResult) null;

    public IEnumerable<string> TempAssemblyFiles
    {
      get
      {
        if (this.application != null)
        {
          ICodeGeneratorProvider service = this.application.GetService<ICodeGeneratorProvider>();
          if (service != null)
            return service.TempAssemblyFiles;
        }
        return Enumerable.Empty<string>();
      }
    }
  }
}
