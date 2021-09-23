// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.CommandLine.ConsoleHost
// Assembly: SpecExplorer, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7507FD4E-0ABD-4C37-958B-5FF2521D030B
// Assembly location: C:\Users\pls2\OneDrive\source code\SourceCode\spec_explorer\original_files\Spec Explorer 2010\SpecExplorer.exe

using Microsoft.Xrt;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.SpecExplorer.CommandLine
{
  public class ConsoleHost : IHost
  {
    private TextWriter consoleOut;
    private TextWriter consoleError;

    public ConsoleHost()
    {
      this.consoleOut = Console.Out;
      this.consoleError = Console.Error;
    }

    public MessageResult DecisionDialog(
      string title,
      string message,
      MessageButton messageButton)
    {
      this.Log(message);
      return MessageResult.None;
    }

    public void DiagMessage(DiagnosisKind kind, string message, object location)
    {
      string str1;
      switch (kind)
      {
        case DiagnosisKind.Error:
          str1 = "ERROR";
          break;
        case DiagnosisKind.Warning:
          str1 = "WARNING";
          break;
        default:
          str1 = "INFO";
          break;
      }
      string str2 = str1;
      string message1;
      if (location != null)
        message1 = str2 + ":\t" + message + "\n at " + location.ToString();
      else
        message1 = str2 + ":\t" + message;
      this.PrintToConsole(kind, message1);
    }

    public IWin32Window DialogOwner => (IWin32Window) null;

    public Exception FatalError(string message, params Exception[] exceptions)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("Fatal Error: {0} \nOptional exceptions causing this fatal error: \r\n", (object) message);
      if (exceptions != null)
      {
        foreach (Exception exception in exceptions)
          stringBuilder.AppendFormat("Exception: {0} \r\n", (object) exception.Message);
      }
      return (Exception) new RecoverFromFatalErrorException(stringBuilder.ToString());
    }

    public object GetService(Type type) => (object) null;

    public DialogResult ModalDialog(Form form) => DialogResult.None;

    public void NotificationDialog(string title, string message)
    {
    }

    public void RecoverFromFatalError(Exception exception)
    {
      if (exception == null || exception is RecoverFromFatalErrorException)
        return;
      this.DiagMessage(DiagnosisKind.Error, exception.ToString(), (object) null);
    }

    public void RunProtected(ProtectedAction action)
    {
    }

    public EventHandler Protect(EventHandler handler) => handler;

    public void Log(string log) => this.consoleOut.WriteLine(log);

    public bool Logging => true;

    public void ProgressMessage(VerbosityLevel verbosity, string message)
    {
      if (verbosity > VerbosityLevel.Medium)
        return;
      this.consoleOut.WriteLine(message);
    }

    public bool TryFindLocation(MemberInfo member, out TextLocation location)
    {
      location = new TextLocation();
      return false;
    }

    public bool TryGetExtensionData(string key, object inputValue, out object outputValue)
    {
      outputValue = (object) null;
      return true;
    }

    public void NavigateTo(string fileName, int line, int column) => this.consoleOut.WriteLine("Navigate to {0}#{1},{2}", (object) fileName, (object) line, (object) column);

    public VerbosityLevel Verbosity => VerbosityLevel.Medium;

    private void PrintToConsole(DiagnosisKind kind, string message)
    {
      ConsoleColor foregroundColor = Console.ForegroundColor;
      switch (kind)
      {
        case DiagnosisKind.Error:
          Console.ForegroundColor = ConsoleColor.Red;
          break;
        case DiagnosisKind.Warning:
          Console.ForegroundColor = ConsoleColor.Yellow;
          break;
      }
      this.consoleError.WriteLine(message);
      Console.ForegroundColor = foregroundColor;
    }
  }
}
