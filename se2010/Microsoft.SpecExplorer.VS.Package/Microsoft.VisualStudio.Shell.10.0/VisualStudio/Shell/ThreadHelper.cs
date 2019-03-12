// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ThreadHelper
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.Internal.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Microsoft.VisualStudio.Shell
{
  public abstract class ThreadHelper
  {
    private static ThreadHelper _generic;

    public static ThreadHelper Generic
    {
      get
      {
        if (ThreadHelper._generic == null)
          ThreadHelper._generic = (ThreadHelper) new GenericThreadHelper();
        return ThreadHelper._generic;
      }
    }

    protected abstract IDisposable GetInvocationWrapper();

    private IVsInvokerPrivate Invoker
    {
      get
      {
        return ServiceProvider.GlobalProvider.GetService(typeof (SVsUIThreadInvokerPrivate)) as IVsInvokerPrivate;
      }
    }

    private bool CheckAccess()
    {
      if (Application.Current == null)
        return ServiceProvider.CheckServiceProviderThreadAccess();
      return Application.Current.CheckAccess();
    }

    private void InvokeOnUIThread(InvokableBase invokable)
    {
      // ISSUE: reference to a compiler-generated method
      this.Invoker.Invoke((IVsInvokablePrivate) invokable);
      if (invokable.Exception != null)
        throw ThreadHelper.WrapException(invokable.Exception);
    }

    private static Exception WrapException(Exception inner)
    {
      try
      {
        return (Exception) Activator.CreateInstance(inner.GetType(), (object) inner.Message, (object) inner);
      }
      catch
      {
        try
        {
          return (Exception) Activator.CreateInstance(inner.GetType(), (object) inner);
        }
        catch
        {
          return (Exception) new TargetInvocationException(inner);
        }
      }
    }

    [DebuggerStepThrough]
    public void Invoke(Action action)
    {
      using (this.GetInvocationWrapper())
      {
        if (this.CheckAccess())
          action();
        else
          this.InvokeOnUIThread((InvokableBase) new InvokableAction(action));
      }
    }

    [DebuggerStepThrough]
    public TResult Invoke<TResult>(Func<TResult> method)
    {
      using (this.GetInvocationWrapper())
      {
        if (this.CheckAccess())
          return method();
        InvokableFunction<TResult> invokableFunction = new InvokableFunction<TResult>(method);
        this.InvokeOnUIThread((InvokableBase) invokableFunction);
        return invokableFunction.Result;
      }
    }
  }
}
