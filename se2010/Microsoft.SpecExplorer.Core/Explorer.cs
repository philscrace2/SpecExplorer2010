// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Explorer
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading;

namespace Microsoft.SpecExplorer
{
  internal class Explorer : IExplorer, IDisposable, IExplorerUpdateUI
  {
    private Microsoft.SpecExplorer.Session session;
    private ExplorerConfiguration explorerConfig;
    private bool oneAppDomain;
    private AppDomain remoteExplorerAppDomain;
    internal IRemoteExplorer remoteExplorer;
    private bool remoteAppDomainTearingDown;
    private ExplorationState state;
    private EventWaitHandle buildWaitHandle;
    private Timer timer;
    private EventWaitHandle exploreWaitHandle;
    private ExplorationResult explorationResult;
    private bool disposed;

    public event EventHandler<ExplorationStatisticsEventArgs> ExplorationStatisticsProgress;

    public event EventHandler<TestingStatisticsEventArgs> TestingStatisticsProgress;

    public event EventHandler<TestCaseFinishedEventArgs> TestCaseFinishedProgress;

    public event EventHandler<ExplorationStateChangedEventArgs> ExplorationStateChanged;

    internal Explorer(Microsoft.SpecExplorer.Session session, ExplorerConfiguration explorerConfig)
    {
      this.session = session;
      this.explorerConfig = explorerConfig;
      this.state = ExplorationState.Created;
      this.buildWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
      this.exploreWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
    }

    public ISession Session => (ISession) this.session;

    public bool Sandboxed
    {
      get => !this.oneAppDomain;
      set => this.oneAppDomain = !value;
    }

    public ExplorationState State => this.state;

    public WaitHandle StartBuilding()
    {
      if (this.remoteExplorer == null)
        this.CreateRemoteExplorer();
      this.remoteExplorer.StartBuild();
      return (WaitHandle) this.buildWaitHandle;
    }

    public WaitHandle StartExploration()
    {
      string s;
      int result;
      if (this.explorerConfig.MachineSwitches.TryGetValue("explorationtimeout", out s) && int.TryParse(s, out result))
        this.timer = new Timer((TimerCallback) (obj => this.Abort()), (object) null, result, result);
      if (this.remoteExplorer == null)
        this.CreateRemoteExplorer();
      this.remoteExplorer.StartExploration();
      return (WaitHandle) this.exploreWaitHandle;
    }

    public ExplorationResult ExplorationResult
    {
      get
      {
        if (this.explorationResult == null)
        {
          lock (this)
          {
            if (this.remoteExplorer != null)
              this.explorationResult = this.remoteExplorer.ExplorationResult;
          }
        }
        return this.explorationResult;
      }
    }

    public void Abort()
    {
      this.SwitchState(ExplorationState.Aborted);
      this.Dispose();
    }

    public virtual void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      lock (this)
      {
        if (!this.disposed && disposing)
        {
          if (this.timer != null)
          {
            this.timer.Dispose();
            this.timer = (Timer) null;
          }
          if (this.exploreWaitHandle != null)
          {
            this.exploreWaitHandle.Close();
            this.exploreWaitHandle = (EventWaitHandle) null;
          }
          if (this.buildWaitHandle != null)
          {
            this.buildWaitHandle.Close();
            this.buildWaitHandle = (EventWaitHandle) null;
          }
          this.TearDownRemoteExplorer();
        }
        this.disposed = true;
      }
    }

    ~Explorer() => this.Dispose(false);

    private void CreateRemoteExplorer()
    {
      if (!this.oneAppDomain)
      {
        AppDomainSetup info = new AppDomainSetup();
        info.LoaderOptimization = LoaderOptimization.MultiDomain;
        string localPath = new Uri(typeof (RemoteExplorer).Assembly.CodeBase).LocalPath;
        info.ApplicationBase = Path.GetDirectoryName(localPath);
        this.remoteExplorerAppDomain = AppDomain.CreateDomain(string.Format("SE explorer for '{0}'", (object) this.explorerConfig.Machine), (Evidence) null, info);
        this.remoteExplorer = this.remoteExplorerAppDomain.CreateInstanceAndUnwrap(typeof (RemoteExplorer).Assembly.FullName, typeof (RemoteExplorer).FullName) as IRemoteExplorer;
      }
      else
      {
        this.remoteExplorerAppDomain = (AppDomain) null;
        this.remoteExplorer = (IRemoteExplorer) new RemoteExplorer();
      }
      if (this.remoteExplorer == null)
        this.session.Host.FatalError("unable to bind correct type of remote explorer");
      this.remoteExplorer.Configure(this.explorerConfig, this.CreateEventManagerForRemote(), new ExplorerMediator(this.session), !this.oneAppDomain);
    }

    private EventManager CreateEventManagerForRemote()
    {
      EventManager eventManager = new EventManager();
      eventManager.RegisterEventObserver((EventObserver) new SwitchStateEventObserver((IExplorerUpdateUI) this));
      eventManager.RegisterEventObserver((EventObserver) new ShowStatisticsObserver((IExplorerUpdateUI) this));
      eventManager.RegisterEventObserver((EventObserver) new ShowTestCaseFinishedProgressEventObserver((IExplorerUpdateUI) this));
      eventManager.RegisterEventObserver((EventObserver) new UpdateExplorationResultEventObserver((IExplorerUpdateUI) this));
      eventManager.RegisterEventObserver((EventObserver) new ProgressMessageEventObserver(this.session.Host));
      eventManager.RegisterEventObserver((EventObserver) new LogEventObserver(this.session.Host));
      eventManager.RegisterEventObserver((EventObserver) new DiagMessageEventObserver(this.session.Host));
      eventManager.RegisterEventObserver((EventObserver) new RecoverFromFatalErrorEventObserver(this.session.Host));
      return eventManager;
    }

    private void TearDownRemoteExplorer()
    {
      lock (this)
      {
        if (this.remoteAppDomainTearingDown)
          return;
        this.remoteAppDomainTearingDown = true;
        try
        {
          string[] strArray = (string[]) null;
          if (this.remoteExplorer != null)
          {
            this.remoteExplorer.Abort();
            strArray = this.remoteExplorer.TempAssemblyFiles.ToArray<string>();
            this.explorationResult = this.remoteExplorer.ExplorationResult;
            this.remoteExplorer.Dispose();
            this.remoteExplorer = (IRemoteExplorer) null;
          }
          if (this.remoteExplorerAppDomain != null)
          {
            try
            {
              AppDomain.Unload(this.remoteExplorerAppDomain);
            }
            catch (CannotUnloadAppDomainException ex)
            {
              this.session.Host.RecoverFromFatalError((Exception) ex);
            }
            finally
            {
              this.remoteExplorerAppDomain = (AppDomain) null;
              if (strArray != null)
              {
                foreach (string path in strArray)
                {
                  try
                  {
                    File.Delete(path);
                  }
                  catch
                  {
                  }
                }
              }
            }
          }
        }
        finally
        {
          this.remoteAppDomainTearingDown = false;
        }
      }
      GC.Collect();
    }

    public void SwitchState(ExplorationState state)
    {
      lock (this)
      {
        if (this.disposed)
          return;
        ExplorationState state1 = this.state;
        this.state = state;
        switch (state)
        {
          case ExplorationState.FailedBuilding:
            this.buildWaitHandle.Set();
            break;
          case ExplorationState.FinishedBuilding:
            this.buildWaitHandle.Set();
            break;
          case ExplorationState.FinishedExploring:
            this.exploreWaitHandle.Set();
            break;
          case ExplorationState.Aborted:
            this.buildWaitHandle.Set();
            this.exploreWaitHandle.Set();
            break;
        }
        if (state1 == state || this.ExplorationStateChanged == null)
          return;
        this.ExplorationStateChanged((object) this, new ExplorationStateChangedEventArgs(state1, state));
      }
    }

    public void ShowStatistics(ExplorationStatistics statistics)
    {
      EventHandler<ExplorationStatisticsEventArgs> statisticsProgress = this.ExplorationStatisticsProgress;
      if (statisticsProgress == null)
        return;
      statisticsProgress((object) this, new ExplorationStatisticsEventArgs(statistics));
    }

    public void ShowStatistics(TestingStatistics statistics)
    {
      EventHandler<TestingStatisticsEventArgs> statisticsProgress = this.TestingStatisticsProgress;
      if (statisticsProgress == null)
        return;
      statisticsProgress((object) this, new TestingStatisticsEventArgs(statistics));
    }

    public void ShowTestCaseFinishedProgress(TestCaseFinishedEventArgs args)
    {
      EventHandler<TestCaseFinishedEventArgs> finishedProgress = this.TestCaseFinishedProgress;
      if (finishedProgress == null)
        return;
      finishedProgress((object) this, args);
    }

    public event EventHandler<ExplorationResultEventArgs> ExplorationResultUpdated;

    public void UpdateExplorationResult(ExplorationResult explorationResult)
    {
      EventHandler<ExplorationResultEventArgs> explorationResultUpdated = this.ExplorationResultUpdated;
      if (explorationResultUpdated == null)
        return;
      explorationResultUpdated((object) this, new ExplorationResultEventArgs(explorationResult));
    }
  }
}
