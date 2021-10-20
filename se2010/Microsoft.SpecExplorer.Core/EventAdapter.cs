using System;
using System.Runtime.Serialization;
using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	internal class EventAdapter
	{
		[Serializable]
		public class SerializableFatalErrorException : Exception
		{
			public SerializableFatalErrorException()
			{
			}

			public SerializableFatalErrorException(string message)
				: base(message)
			{
			}

			public SerializableFatalErrorException(string message, Exception innerException)
				: base(message, innerException)
			{
			}

			protected SerializableFatalErrorException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}

		private EventManager eventMgr;

		private IRemoteExplorer explorer;

		internal EventAdapter(EventManager eventMgr, IRemoteExplorer explorer)
		{
			this.eventMgr = eventMgr;
			this.explorer = explorer;
		}

		internal void SwitchState(ExplorationState state)
		{
			explorer.State = state;
			eventMgr.HandleEvent(new SwitchStateEvent(state));
		}

		internal void UpdateExplorationResult(ExplorationResult explorationResult)
		{
			eventMgr.HandleEvent(new UpdateExplorationResultEvent(explorationResult));
		}

		internal void DiagMessage(DiagnosisKind kind, string message, object location)
		{
			eventMgr.HandleEvent(new DiagMessageEvent(kind, message, location));
		}

		internal void DiagMessage(DiagnosisKind kind, string message)
		{
			eventMgr.HandleEvent(new DiagMessageEvent(kind, message, null));
		}

		internal void Log(string message)
		{
			eventMgr.HandleEvent(new LogEvent(message));
		}

		internal void RecoverFromFatalError(Exception exception)
		{
			try
			{
				try
				{
					eventMgr.HandleEvent(new RecoverFromFatalErrorEvent(exception));
				}
				catch (SerializationException)
				{
					eventMgr.HandleEvent(new RecoverFromFatalErrorEvent(new SerializableFatalErrorException(string.Format("Non-serializable exception:\r\nMessage:{0}\r\nStack Trace:\r\n{1}", exception.Message, exception.StackTrace))));
				}
			}
			catch (Exception exception2)
			{
				eventMgr.HandleEvent(new RecoverFromFatalErrorEvent(exception2));
			}
		}

		internal void ProgressMessage(VerbosityLevel verbosity, string message)
		{
			eventMgr.HandleEvent(new ProgressMessageEvent(verbosity, message));
		}

		internal void ShowStatistics(ExplorationStatistics statistics)
		{
			eventMgr.HandleEvent(new ShowExplorationStatistics(statistics));
		}

		internal void ShowStatistics(TestingStatistics statistics)
		{
			eventMgr.HandleEvent(new ShowTestingStatistics(statistics));
		}

		internal void ShowTestCaseFinishedProgress(TestCaseFinishedEventArgs progress)
		{
			eventMgr.HandleEvent(new ShowTestCaseFinishedProgress(progress));
		}
	}
}
