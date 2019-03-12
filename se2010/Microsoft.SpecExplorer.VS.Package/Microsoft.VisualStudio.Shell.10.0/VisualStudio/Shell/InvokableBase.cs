// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.InvokableBase
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.Internal.VisualStudio.Shell.Interop;
using System;
using System.Windows;

namespace Microsoft.VisualStudio.Shell
{
  internal abstract class InvokableBase : IVsInvokablePrivate
  {
    protected abstract void InvokeMethod();

    public int Invoke()
    {
      this.VerifyAccess();
      try
      {
        this.InvokeMethod();
      }
      catch (Exception ex)
      {
        this.Exception = ex;
      }
      return 0;
    }

    public Exception Exception { get; private set; }

    private void VerifyAccess()
    {
      if (Application.Current == null)
      {
        if (!ServiceProvider.CheckServiceProviderThreadAccess())
          throw new InvalidOperationException(Resources.Services_InvokedOnWrongThread);
      }
      else
        Application.Current.Dispatcher.VerifyAccess();
    }
  }
}
