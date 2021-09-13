// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.EventAdapter
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.Runtime.Serialization;

namespace Microsoft.SpecExplorer
{
  internal class EventAdapter
  {
    private EventManager eventMgr;
    private IRemoteExplorer explorer;

    internal EventAdapter(EventManager eventMgr, IRemoteExplorer explorer)
    {
      this.eventMgr = eventMgr;
      this.explorer = explorer;
    }

    internal void SwitchState(ExplorationState state)
    {
      this.explorer.State = state;
      this.eventMgr.HandleEvent((ExplorerEvent) new SwitchStateEvent(state));
    }

    internal void UpdateExplorationResult(ExplorationResult explorationResult) => this.eventMgr.HandleEvent((ExplorerEvent) new UpdateExplorationResultEvent(explorationResult));

    internal void DiagMessage(DiagnosisKind kind, string message, object location) => this.eventMgr.HandleEvent((ExplorerEvent) new DiagMessageEvent(kind, message, location));

    internal void DiagMessage(DiagnosisKind kind, string message) => this.eventMgr.HandleEvent((ExplorerEvent) new DiagMessageEvent(kind, message, (object) null));

    internal void Log(string message) => this.eventMgr.HandleEvent((ExplorerEvent) new LogEvent(message));

    internal void RecoverFromFatalError(Exception exception)
    {
      try
      {
        try
        {
          this.eventMgr.HandleEvent((ExplorerEvent) new RecoverFromFatalErrorEvent(exception));
        }
        catch (SerializationException ex)
        {
          this.eventMgr.HandleEvent((ExplorerEvent) new RecoverFromFatalErrorEvent((Exception) new EventAdapter.SerializableFatalErrorException(string.Format("Non-serializable exception:\r\nMessage:{0}\r\nStack Trace:\r\n{1}", (object) exception.Message, (object) exception.StackTrace))));
        }
      }
      catch (Exception ex)
      {
        this.eventMgr.HandleEvent((ExplorerEvent) new RecoverFromFatalErrorEvent(ex));
      }
    }

    internal void ProgressMessage(VerbosityLevel verbosity, string message) => this.eventMgr.HandleEvent((ExplorerEvent) new ProgressMessageEvent(verbosity, message));

    internal void ShowStatistics(ExplorationStatistics statistics) => this.eventMgr.HandleEvent((ExplorerEvent) new ShowExplorationStatistics(statistics));

    internal void ShowStatistics(TestingStatistics statistics) => this.eventMgr.HandleEvent((ExplorerEvent) new ShowTestingStatistics(statistics));

    internal void ShowTestCaseFinishedProgress(TestCaseFinishedEventArgs progress) => this.eventMgr.HandleEvent((ExplorerEvent) new Microsoft.SpecExplorer.ShowTestCaseFinishedProgress(progress));

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
  }
}
