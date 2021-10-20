using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	internal class Explorer : IExplorer, IDisposable, IExplorerUpdateUI
	{
		private Session session;

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

		public ISession Session
		{
			get
			{
				return session;
			}
		}

		public bool Sandboxed
		{
			get
			{
				return !oneAppDomain;
			}
			set
			{
				oneAppDomain = !value;
			}
		}

		public ExplorationState State
		{
			get
			{
				return state;
			}
		}

		public ExplorationResult ExplorationResult
		{
			get
			{
				if (explorationResult == null)
				{
					lock (this)
					{
						if (remoteExplorer != null)
						{
							explorationResult = remoteExplorer.ExplorationResult;
						}
					}
				}
				return explorationResult;
			}
		}

		public event EventHandler<ExplorationStatisticsEventArgs> ExplorationStatisticsProgress;

		public event EventHandler<TestingStatisticsEventArgs> TestingStatisticsProgress;

		public event EventHandler<TestCaseFinishedEventArgs> TestCaseFinishedProgress;

		public event EventHandler<ExplorationStateChangedEventArgs> ExplorationStateChanged;

		public event EventHandler<ExplorationResultEventArgs> ExplorationResultUpdated;

		internal Explorer(Session session, ExplorerConfiguration explorerConfig)
		{
			this.session = session;
			this.explorerConfig = explorerConfig;
			state = ExplorationState.Created;
			buildWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
			exploreWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
		}

		public WaitHandle StartBuilding()
		{
			if (remoteExplorer == null)
			{
				CreateRemoteExplorer();
			}
			remoteExplorer.StartBuild();
			return buildWaitHandle;
		}

		public WaitHandle StartExploration()
		{
			string value;
			int result;
			if (explorerConfig.MachineSwitches.TryGetValue("explorationtimeout", out value) && int.TryParse(value, out result))
			{
				timer = new Timer(delegate
				{
					Abort();
				}, null, result, result);
			}
			if (remoteExplorer == null)
			{
				CreateRemoteExplorer();
			}
			remoteExplorer.StartExploration();
			return exploreWaitHandle;
		}

		public void Abort()
		{
			SwitchState(ExplorationState.Aborted);
			Dispose();
		}

		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!disposed && disposing)
				{
					if (timer != null)
					{
						timer.Dispose();
						timer = null;
					}
					if (exploreWaitHandle != null)
					{
						exploreWaitHandle.Close();
						exploreWaitHandle = null;
					}
					if (buildWaitHandle != null)
					{
						buildWaitHandle.Close();
						buildWaitHandle = null;
					}
					TearDownRemoteExplorer();
				}
				disposed = true;
			}
		}

		~Explorer()
		{
			Dispose(false);
		}

		private void CreateRemoteExplorer()
		{
			if (!oneAppDomain)
			{
				AppDomainSetup appDomainSetup = new AppDomainSetup();
				appDomainSetup.LoaderOptimization = LoaderOptimization.MultiDomain;
				string localPath = new Uri(typeof(RemoteExplorer).Assembly.CodeBase).LocalPath;
				appDomainSetup.ApplicationBase = Path.GetDirectoryName(localPath);
				remoteExplorerAppDomain = AppDomain.CreateDomain(string.Format("SE explorer for '{0}'", explorerConfig.Machine), null, appDomainSetup);
				remoteExplorer = remoteExplorerAppDomain.CreateInstanceAndUnwrap(typeof(RemoteExplorer).Assembly.FullName, typeof(RemoteExplorer).FullName) as IRemoteExplorer;
			}
			else
			{
				remoteExplorerAppDomain = null;
				remoteExplorer = new RemoteExplorer();
			}
			if (remoteExplorer == null)
			{
				session.Host.FatalError("unable to bind correct type of remote explorer");
			}
			remoteExplorer.Configure(explorerConfig, CreateEventManagerForRemote(), new ExplorerMediator(session), !oneAppDomain);
		}

		private EventManager CreateEventManagerForRemote()
		{
			EventManager eventManager = new EventManager();
			eventManager.RegisterEventObserver(new SwitchStateEventObserver(this));
			eventManager.RegisterEventObserver(new ShowStatisticsObserver(this));
			eventManager.RegisterEventObserver(new ShowTestCaseFinishedProgressEventObserver(this));
			eventManager.RegisterEventObserver(new UpdateExplorationResultEventObserver(this));
			eventManager.RegisterEventObserver(new ProgressMessageEventObserver(session.Host));
			eventManager.RegisterEventObserver(new LogEventObserver(session.Host));
			eventManager.RegisterEventObserver(new DiagMessageEventObserver(session.Host));
			eventManager.RegisterEventObserver(new RecoverFromFatalErrorEventObserver(session.Host));
			return eventManager;
		}

		private void TearDownRemoteExplorer()
		{
			lock (this)
			{
				if (remoteAppDomainTearingDown)
				{
					return;
				}
				remoteAppDomainTearingDown = true;
				try
				{
					string[] array = null;
					if (remoteExplorer != null)
					{
						remoteExplorer.Abort();
						array = remoteExplorer.TempAssemblyFiles.ToArray();
						explorationResult = remoteExplorer.ExplorationResult;
						remoteExplorer.Dispose();
						remoteExplorer = null;
					}
					if (remoteExplorerAppDomain != null)
					{
						try
						{
							AppDomain.Unload(remoteExplorerAppDomain);
						}
						catch (CannotUnloadAppDomainException exception)
						{
							session.Host.RecoverFromFatalError(exception);
						}
						finally
						{
							remoteExplorerAppDomain = null;
							if (array != null)
							{
								string[] array2 = array;
								foreach (string path in array2)
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
					remoteAppDomainTearingDown = false;
				}
			}
			GC.Collect();
		}

		public void SwitchState(ExplorationState state)
		{
			lock (this)
			{
				if (!disposed)
				{
					ExplorationState explorationState = this.state;
					this.state = state;
					switch (state)
					{
					case ExplorationState.FailedBuilding:
						buildWaitHandle.Set();
						break;
					case ExplorationState.FinishedBuilding:
						buildWaitHandle.Set();
						break;
					case ExplorationState.FinishedExploring:
						exploreWaitHandle.Set();
						break;
					case ExplorationState.Aborted:
						buildWaitHandle.Set();
						exploreWaitHandle.Set();
						break;
					}
					if (explorationState != state && this.ExplorationStateChanged != null)
					{
						this.ExplorationStateChanged(this, new ExplorationStateChangedEventArgs(explorationState, state));
					}
				}
			}
		}

		public void ShowStatistics(ExplorationStatistics statistics)
		{
			EventHandler<ExplorationStatisticsEventArgs> explorationStatisticsProgress = this.ExplorationStatisticsProgress;
			if (explorationStatisticsProgress != null)
			{
				explorationStatisticsProgress(this, new ExplorationStatisticsEventArgs(statistics));
			}
		}

		public void ShowStatistics(TestingStatistics statistics)
		{
			EventHandler<TestingStatisticsEventArgs> testingStatisticsProgress = this.TestingStatisticsProgress;
			if (testingStatisticsProgress != null)
			{
				testingStatisticsProgress(this, new TestingStatisticsEventArgs(statistics));
			}
		}

		public void ShowTestCaseFinishedProgress(TestCaseFinishedEventArgs args)
		{
			EventHandler<TestCaseFinishedEventArgs> testCaseFinishedProgress = this.TestCaseFinishedProgress;
			if (testCaseFinishedProgress != null)
			{
				testCaseFinishedProgress(this, args);
			}
		}

		public void UpdateExplorationResult(ExplorationResult explorationResult)
		{
			EventHandler<ExplorationResultEventArgs> explorationResultUpdated = this.ExplorationResultUpdated;
			if (explorationResultUpdated != null)
			{
				explorationResultUpdated(this, new ExplorationResultEventArgs(explorationResult));
			}
		}
	}
}
