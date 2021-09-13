// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.IHost
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Windows.Forms;

namespace Microsoft.SpecExplorer
{
  public interface IHost
  {
    Exception FatalError(string message, params Exception[] exceptions);

    void RecoverFromFatalError(Exception exception);

    void RunProtected(ProtectedAction action);

    EventHandler Protect(EventHandler handler);

    void Log(string line);

    bool Logging { get; }

    void ProgressMessage(VerbosityLevel verbosity, string message);

    VerbosityLevel Verbosity { get; }

    void DiagMessage(DiagnosisKind kind, string message, object location);

    void NotificationDialog(string title, string message);

    MessageResult DecisionDialog(
      string title,
      string message,
      MessageButton messageButton);

    DialogResult ModalDialog(Form form);

    object GetService(System.Type type);

    IWin32Window DialogOwner { get; }

    bool TryFindLocation(MemberInfo member, out TextLocation location);

    bool TryGetExtensionData(string key, object inputValue, out object outputValue);

    void NavigateTo(string fileName, int line, int column);
  }
}
