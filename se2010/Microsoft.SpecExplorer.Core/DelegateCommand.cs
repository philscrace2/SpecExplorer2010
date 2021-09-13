// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.DelegateCommand
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Windows.Input;

namespace Microsoft.SpecExplorer
{
  public class DelegateCommand : ICommand
  {
    public Func<bool> CanExecuteMethod { get; private set; }

    public Action<object> ExecutedMethod { get; private set; }

    public DelegateCommand(Action<object> executedMethod) => this.ExecutedMethod = executedMethod;

    public DelegateCommand(Action<object> executedMethod, Func<bool> canExecuteMethod)
      : this(executedMethod)
    {
      this.CanExecuteMethod = canExecuteMethod;
    }

    public void InvokeCanExecuteChangedEvent()
    {
      if (this.CanExecuteChanged == null)
        return;
      this.CanExecuteChanged((object) this, (EventArgs) null);
    }

    public bool CanExecute(object parameter) => this.CanExecuteMethod == null || this.CanExecuteMethod();

    public event EventHandler CanExecuteChanged;

    public void Execute(object parameter)
    {
      if (this.ExecutedMethod == null)
        return;
      this.ExecutedMethod(parameter);
    }
  }
}
